using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SaveManagement;
using DG.Tweening;

namespace StreetPerformers
{
    public class HubArea : MonoBehaviour
    {

        [SerializeField]
        private Sprite _backgroundSprite;
        [SerializeField]
        private Sprite _frontgroundSprite;

        [SerializeField]
        private GameObject _enemyPanel;
        [SerializeField]
        private HubBackground _hubBackground;
#if UNITY_STANDALONE
        [SerializeField] private NewEnemySelectPanel _newEnemySelectPanel = null;
        [SerializeField] private GameObject _mainMenu = null;
#endif

        [SerializeField]
        private Sprite _clearStamp;
        [SerializeField]
        private Sprite _newStamp;

        [SerializeField]
        private Sprite _titleSprite = null;

        public TextAsset _descriptionText = null;

        public string _idString;
        public Color _crowdColor;
        public Color _backgroundColor;

        private HubAreaData _data;
        private ProgressionHandler _progression;

        private Vector3 _startScale;
        private Vector3 _origCamPos;

        private RectTransform _rect = null;

        private bool _aceInitialized = false;
        private bool _containsAce = false;

        [SerializeField] private HubTutorialManager _hubTutorial = null;

        [SerializeField] private List<HubArea> _upAreas = new List<HubArea>();
        [SerializeField] private List<HubArea> _downAreas = new List<HubArea>();
        [SerializeField] private List<HubArea> _leftAreas = new List<HubArea>();
        [SerializeField] private List<HubArea> _rightAreas = new List<HubArea>();
        [SerializeField] private List<HubArea> _upRightAreas = new List<HubArea>();
        [SerializeField] private List<HubArea> _upLeftAreas = new List<HubArea>();
        [SerializeField] private List<HubArea> _downRightAreas = new List<HubArea>();
        [SerializeField] private List<HubArea> _downLeftAreas = new List<HubArea>();

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void Start()
        {
            _progression = ProgressionHandler.Instance;
            _data = _progression._hubAreas[_idString];
            _startScale = transform.localScale;

            if(!_data._active)
            {
                this.gameObject.SetActive(false);
            }
            _progression.OnLevelUp += LevelUp;

            GameObject stamp = transform.Find("Stamp").gameObject;
            if(_data._battleLevel >= _data._battles.Count)
            {
                stamp.GetComponent<Image>().sprite = _clearStamp;
                stamp.SetActive(true);
            }
            else
            {
                if(!_data._battles[_data._nonBonusBattleCount].BonusRequirementFulfilled())
                {
                    if(_data._battleLevel >= _data._nonBonusBattleCount)
                    {
                        stamp.GetComponent<Image>().sprite = _clearStamp;
                        stamp.SetActive(true);
                    }
                    else if(!_data._battles[_data._battleLevel]._attempted)
                    {
                        if(ProgressionHandler.Instance._tutorialFinished || _data._battleLevel == 0)
                        {
                            stamp.GetComponent<Image>().sprite = _newStamp;
                            stamp.SetActive(true);
                        }
                    }
                }
                else if(!_data._battles[_data._battleLevel]._attempted)
                {
                    stamp.GetComponent<Image>().sprite = _newStamp;
                    stamp.SetActive(true);
                }
            }
        }

        private void SetSpawn()
        {
            _hubBackground.SetAce(transform.position);
        }

        private void Initialize()
        {
            if(BattleHandler.Instance._activeHubName.Equals(_idString))
            {
                _containsAce = true;
                SetSpawn();
            }

            if (_data._justUnlocked)
            {
                UnlockAnimation();
            }
            else if (_idString.Equals("Pier"))
            {
                if (!_data._battles[0]._attempted)
                {
                    _hubTutorial.Initialize(1);
                }
                else if (ProgressionHandler.Instance._tutorialFinished && !ProgressionHandler.Instance._officialTutorialFinished)
                {
                    _hubTutorial.Initialize(2);
                }
            }
        }

        private void Update()
        {
            if (!_aceInitialized && transform.position.magnitude > .1f)
            {
                _aceInitialized = true;
                Initialize();
                //Vector3 centerOffset = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f) - transform.position;
                //_hubBackground.SetPosition(_hubBackground.transform.position + centerOffset);
               
            }
            if (this.gameObject.activeInHierarchy)
            {
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    _data.LevelUp();
                }

                if(_containsAce && !_newEnemySelectPanel.gameObject.activeInHierarchy && !_mainMenu.activeInHierarchy)
                {
                    Vector2Int movementVec = new Vector2Int(Mathf.RoundToInt(Input.GetAxis("Horizontal")), Mathf.RoundToInt(Input.GetAxis("Vertical")));
                    if(movementVec.magnitude > 0)
                    {
                        MoveAce(movementVec);
                    }
                    else
                    {
                        if(Input.GetButtonDown("Select"))
                        {
                            Clicked();
                        }
                    }
                }
            }
        }

        private void MoveAce(Vector2Int movementVec)
        {
            List<HubArea> hubsToCheck = new List<HubArea>();
            //up, up right, up left
            if(movementVec[1] == 1)
            {
                //up right
                if(movementVec[0] == 1)
                {
                    hubsToCheck = _upRightAreas;
                }
                //up left
                else if(movementVec[0] == -1)
                {
                    hubsToCheck = _upLeftAreas;
                }
                //up
                else
                {
                    hubsToCheck = _upAreas;
                }
            }
            //down, down right, down left
            else if(movementVec[1] == -1)
            {
                //down right
                if (movementVec[0] == 1)
                {
                    hubsToCheck = _downRightAreas;
                }
                //down left
                else if (movementVec[0] == -1)
                {
                    hubsToCheck = _downLeftAreas;
                }
                //down
                else
                {
                    hubsToCheck = _downAreas;
                }
            }
            //right
            else if(movementVec[0] == 1)
            {
                hubsToCheck = _rightAreas;
            }
            //left
            else if(movementVec[0] == -1)
            {
                hubsToCheck = _leftAreas;
            }

            if(hubsToCheck.Count > 0)
            {
                foreach(HubArea hub in hubsToCheck)
                {
                    if(hub.gameObject.activeInHierarchy)
                    {
                        _containsAce = false;
                        hub.MoveAceTowardsSelf(1.5f);
                        break;
                    }
                }
            }
        }

        private void UnlockAnimation()
        {
            _data._justUnlocked = false;

            float moveTime = 1.5f;

            Vector3 centerOffset = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f) - transform.position;
            //Vector3 moveVec = _hubBackground.MoveTowards(_hubBackground.transform.position + centerOffset, 2f);
            _hubBackground.SetActiveStatus(false);

            transform.localScale = new Vector3(0f, 0f, 0f);
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(new Vector3(0f, 0f, 0f), moveTime)).Append(transform.DOScale(_startScale, 1.5f).SetEase(Ease.OutBack))
                .OnComplete(
                delegate
                {
                    if (_idString.Equals("Pier") && !_data._battles[0]._attempted)
                    {
                        _hubTutorial.Initialize(1);
                    }
                    else
                    {
                        Invoke("AnimateBackToAce", 1f);
                    }
                });
        }

        private void AnimateBackToAce()
        {
            //_hubBackground.ResetPosition();
            Invoke("EndAnimation", 1f);
        }

        private void EndAnimation()
        {
            _hubBackground.SetActiveStatus(true);
        }

        private void LevelUp()
        {
            if(this == null)
            {
                ProgressionHandler.Instance.OnLevelUp -= LevelUp;
                return;
            }

            ProgressionHandler progression = ProgressionHandler.Instance;
            if(progression._hubAreas[_idString]._active)
            {
                this.gameObject.SetActive(true);

                if (_enemyPanel.activeInHierarchy)
                {
                    PopulateEnemyPanel();
                }
            }
        }

        public void MoveAceTowardsSelf(float time)
        {
            _hubBackground.MoveAceTowards(transform.position, time);
            Invoke("MoveAceTowardsSelfFinished", time);
        }

        private void MoveAceTowardsSelfFinished()
        {
            _containsAce = true;
        }

        public void Clicked()
        {
            if(_enemyPanel.activeInHierarchy)
            {
                FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);

                _enemyPanel.SetActive(false);

                _hubBackground._menuOpen = false;
            }
            else
            {
                if(_hubBackground._menuOpen || !_hubBackground.IsActive())
                { return; }

                _hubBackground._menuOpen = true;

                if(Vector2.Distance(transform.position, _hubBackground.GetAcePos()) < .1f)
                {
                    PopulateEnemyPanel();
                }
                else
                {
                    float moveTime = 1.5f;
                    //Vector3 centerOffset = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f) - transform.position;
                    // Vector3 moveVec = _hubBackground.MoveTowards(_hubBackground.transform.position + centerOffset, moveTime);
                    _hubBackground.ResetContainsAce();
                    MoveAceTowardsSelf(moveTime);

                    Invoke("PopulateEnemyPanel", moveTime);
                }
            }
        }

        private void PopulateEnemyPanel()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);
            

#if UNITY_STANDALONE
            _hubBackground._menuOpen = false;
            _newEnemySelectPanel.Initialize(_idString, GetComponent<Image>().sprite, _backgroundSprite, _crowdColor, _idString, _descriptionText.text, _titleSprite, _backgroundColor);
#else
            _enemyPanel.SetActive(true);
            _enemyPanel.transform.Find("HubName").GetComponent<TextMeshProUGUI>().text = _idString;
            _enemyPanel.transform.Find("Icon").GetComponent<Image>().sprite = GetComponent<Image>().sprite;
            _enemyPanel.transform.Find("EnemyPanel").GetComponent<EnemySelectPanel>().Populate(this);
#endif
        }

        public void BattleButton(int index)
        {
            foreach(EnemyStruct enemy in _progression._hubAreas[_idString]._battles[index]._enemyList)
            {
                if(!_progression._encounteredEnemies.Contains(enemy._enemyName))
                {
                    _progression._encounteredEnemies.Add(enemy._enemyName);
                }
            }

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Level_Select);

            BattleHandler.Instance.SetBattle(_idString, index, _backgroundSprite, _frontgroundSprite, _crowdColor);

#if UNITY_STANDALONE
                SceneManager.LoadScene("BattleSceneLandscape");
#elif UNITY_IPHONE || UNITY_ANDROID
                SceneManager.LoadScene("BattleScene");
#endif
        }

        public void UpdateActiveStatus()
        {
            ProgressionHandler progression = ProgressionHandler.Instance;
            if (progression._hubAreas[_idString]._active)
            {
                this.gameObject.SetActive(true);
            }
        }

        public void ResetContainsAce()
        {
            _containsAce = false;
        }
    }
}

