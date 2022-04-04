using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace StreetPerformers
{
    public class DialogueManager : MonoBehaviour
    {
        private bool _displayOn; //bool to pass to ToggleConvo() to turn the conversation UI on or off.
        public GameObject[] convoElements;

        [SerializeField] private Transform _offScreenPos = null;
        [SerializeField] private Transform _onScreenPos = null;

        [SerializeField] private BattleManager _battleManager;

        bool canProgress;
        public TextMeshProUGUI dialogueText;
        [SerializeField]
        private GameObject _allyNameBox = null;
        [SerializeField]
        private Image _allyName = null;
        [SerializeField]
        private GameObject _enemyNameBox = null;
        [SerializeField]
        private Image _enemyName = null;

        public Image _characterSprite;
        public Image _enemySprite;
        public Image _screenOverlay;
        public Conversation _currentConvo;
        GameObject _convo;

        [SerializeField]
        private Image _autoProgressButton;
        private Color _autoInitColor;

        [SerializeField] private Transform _enemyOffScreen;
        [SerializeField] private Transform _enemyOnScreen;
        [SerializeField] private Transform _enemyInactive;
        [SerializeField] private Transform _allyOffScreen;
        [SerializeField] private Transform _allyOnScreen;
        [SerializeField] private Transform _allyInactive;

        private string _curEnemy = "";
        private string _curAlly = "";
        private bool _enemySpeaking = false;

        private bool _typing = false;
        private int _typingIndex = 0;
        private string _typingLine = "";
        private bool _autoProgress = false;

        [SerializeField]
        private List<Sprite> _nameSprites = new List<Sprite>();
        private Dictionary<string, int> _nameToSpriteIndex = new Dictionary<string, int>();

        [Header("Disable During Dialogue")]
        [SerializeField] private List<GameObject> _disableList = new List<GameObject>();
        [SerializeField] private BattleAgents _agents = null;
        [SerializeField] private Transform _menuButtonTrans = null;
        private int _menuButtonSiblingIndex = 0;

        private Action _completeCallback = null;

        private void Awake()
        {
            _battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();

            _autoProgress = PlayerPrefs.GetInt("Auto Progress") != 0;
            _autoInitColor = _autoProgressButton.color;

            SetAutoProgressColor();

            _nameToSpriteIndex["Ace"] = 0;
            _nameToSpriteIndex["Contortionist"] = 1;
            _nameToSpriteIndex["Mascot"] = 2;
            _nameToSpriteIndex["Mime"] = 3;
            _nameToSpriteIndex["Gem"] = 4;
            _nameToSpriteIndex["Cop"] = 5;
            _nameToSpriteIndex["Wild Guy"] = 6;
            _nameToSpriteIndex["Statue"] = 7;
            _nameToSpriteIndex["StatueName"] = 8;
            _nameToSpriteIndex["Magician"] = 9;
            _nameToSpriteIndex["???"] = 10;
        }

        private void SetupConversation()
        {
            this.gameObject.SetActive(true);
            convoElements[0].transform.position = _offScreenPos.position;
            _enemySprite.gameObject.SetActive(false);
            _characterSprite.gameObject.SetActive(false);
        }

        public void StartConversation(GameObject conversation, Action completeCallback = null)
        {
            _completeCallback = completeCallback;

            SetupConversation();

            if(conversation != null)
            {
                _convo = Instantiate(conversation);
                _currentConvo = _convo.GetComponent<Conversation>();
                _currentConvo.StartConvo(this);
            }
            else
            {
                //_battleManager
                this.gameObject.SetActive(false);
            }
        }

        public void StartConversation(bool first)
        {
            SetupConversation();

            if(first)
            {
                if(BattleHandler.Instance._beforeConvo != null)
                {
                    _convo = Instantiate(BattleHandler.Instance._beforeConvo);
                    _currentConvo = _convo.GetComponent<Conversation>();
                    _currentConvo.StartConvo(this);
                }
                else
                {
                    _battleManager.StartBattle();
                    this.gameObject.SetActive(false);
                }
            }
            else
            {
                if(BattleHandler.Instance._afterConvo != null)
                {
                    _convo = Instantiate(BattleHandler.Instance._afterConvo);
                    _currentConvo = _convo.GetComponent<Conversation>();
                    _currentConvo.StartConvo(this);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

        public void ToggleDisabledObjects(bool toggle)
        {
            foreach (GameObject obj in _disableList)
            {
                obj.SetActive(toggle);
            }
            _agents.ToggleHealthCanvas(toggle);

            if(toggle)
            {
                _menuButtonTrans.SetSiblingIndex(_menuButtonSiblingIndex);
            }
            else
            {
                _menuButtonSiblingIndex = _menuButtonTrans.GetSiblingIndex();
                _menuButtonTrans.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            }
        }

        public void NextConvoSentence()
        {
            if(!canProgress)
            { return; }

            CancelInvoke("NextConvoSentence");

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Advance_Text);

            if(_typing)
            {
                _typingIndex = _typingLine.Length;
            }
            else
            {
                _currentConvo.Next();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetButtonDown("Select"))
            {
                NextConvoSentence();
            }
        }

        public void BeginConvo(Conversation convo)
        {
            DarkenScreen();
            canProgress = true;
            _displayOn = true;
            _curAlly = "";
            _curEnemy = "";
            ToggleDisplay(_displayOn);
        }

        public void DarkenScreen()
        {
            Color dark = new Color(0, 0, 0, .8f);

            _screenOverlay.DOColor(dark, .2f).SetEase(Ease.InCirc) ;
        }

        public void ClearScreen()
        {
            Color clear = new Color(0, 0, 0, 0);

            _screenOverlay.DOColor(clear, .2f);
        }

        public void EndConvo()
        {
            ClearScreen();
            _displayOn = false;
            canProgress = false;
            _curAlly = "";
            _curEnemy = "";
            ToggleDisplay(_displayOn);
        }
        
        public void ToggleDisplay(bool on)
        {
            if (on == true)
            {
                convoElements[0].SetActive(on);
                convoElements[0].transform.DOMove(_onScreenPos.position, .3f).SetEase(Ease.InSine).OnComplete(canClick);
            }
            else{

                StartCoroutine(HideUI(.4f));  //wait unitl the text box is offscreen, then disable it's text component
            }
        }

        public void canClick()
        {
            canProgress = true;
        }

        public void ToggleAutoProgress()
        {
            CancelInvoke("NextConvoSentence");
            _autoProgress = !_autoProgress;
            PlayerPrefs.SetInt("Auto Progress", _autoProgress ? 1 : 0);

            SetAutoProgressColor();

            if(_autoProgress && !_typing)
            {
                CancelInvoke("NextConvoSentence");
                Invoke("NextConvoSentence", 1f);
            }
        }

        private void SetAutoProgressColor()
        {
            if(_autoProgress)
            {
                _autoProgressButton.color = Color.green;
            }
            else
            {
                _autoProgressButton.color = _autoInitColor;
            }
        }

        public void NextLine(string textToShow, float textSpeed)
        {
            StartCoroutine(DisplayText(textToShow, textSpeed));
        }

        private IEnumerator DisplayText(string textToShow, float textSpeed)
        {
            _typingIndex = 0;
            _typingLine = textToShow;
            string curText = "";
            dialogueText.text = curText;
            WaitForSeconds wait = new WaitForSeconds(1f / textSpeed);
            string colorTag = "<color=#00000000>";
            _typing = true;

            yield return wait;

            _typingIndex++;
            _typingIndex = Mathf.Min(_typingIndex, _typingLine.Length);

            while(_typingIndex < _typingLine.Length)
            {
                curText = _typingLine.Substring(0, _typingIndex) + colorTag + _typingLine.Substring(_typingIndex) + "</color>";
                dialogueText.text = curText;
                _typingIndex++;
                _typingIndex = Mathf.Min(_typingIndex, _typingLine.Length);
                yield return wait;
            }

            dialogueText.text = _typingLine;

            yield return new WaitForSeconds(.2f);

            _typing = false;

            if(_autoProgress)
            {
                Invoke("NextConvoSentence", 2f);
            }

            yield return null;
        }

        public IEnumerator switchCharAnimation(float animTime, string personName, string displayName, Sprite sprite, bool enemySpeaking)
        {
            if(_enemySpeaking && enemySpeaking && personName == _curEnemy)
            {
                _enemySprite.sprite = sprite;
                yield break;
            }
            else if(!_enemySpeaking && !enemySpeaking && personName == _curAlly)
            {
                _characterSprite.sprite = sprite;
                yield break;
            }

            _enemySpeaking = enemySpeaking;

            if(!canProgress)
            { yield break; }

            _characterSprite.transform.DOComplete();
            _enemySprite.transform.DOComplete();

            //characterNameText.text = name;
            _allyNameBox.SetActive(!enemySpeaking && !displayName.Equals(""));
            _enemyNameBox.SetActive(enemySpeaking && !displayName.Equals(""));

            if(enemySpeaking)
            {

                if(!displayName.Equals(""))
                {
                    _enemyName.sprite = _nameSprites[_nameToSpriteIndex[displayName]];
                    
                }

                if(personName != _curEnemy)
                {
                    _enemySprite.transform.DOMove(_enemyOffScreen.position, animTime);
                }
            }
            else
            {
                if(!displayName.Equals(""))
                {
                    _allyName.sprite = _nameSprites[_nameToSpriteIndex[displayName]];
                }

                if(personName != _curAlly)
                {
                    _characterSprite.transform.DOMove(_allyOffScreen.position, animTime);
                }
            }
            
            yield return new WaitForSeconds(animTime);

            if(!canProgress)
            { yield break; }

            if(enemySpeaking)
            {
                _curEnemy = personName;

                _enemySprite.gameObject.SetActive(true);
                _enemySprite.transform.SetAsLastSibling();
                _enemySprite.sprite = sprite;
                if(sprite == null)
                {
                    _enemySprite.enabled = false;
                }
                else
                {
                    _enemySprite.enabled = true;
                    _enemySprite.SetNativeSize();
                }
                
                _enemySprite.transform.DOScale(_enemyOnScreen.localScale, animTime);
                //_enemySprite.transform.position = _enemyOffScreen.position;
                _enemySprite.transform.DOMove(_enemyOnScreen.position, animTime);
                _enemySprite.DOColor(Color.white, animTime);

                if(_characterSprite.gameObject.activeSelf)
                {
                    _characterSprite.transform.DOMove(_allyInactive.position, animTime);
                    _characterSprite.transform.DOScale(_allyInactive.localScale, animTime);
                    _characterSprite.DOColor(Color.grey, animTime);
                }
            }
            else
            {
                _curAlly = personName;

                _characterSprite.gameObject.SetActive(true);
                _characterSprite.transform.SetAsLastSibling();
                _characterSprite.sprite = sprite;
                if(sprite == null)
                {
                    _characterSprite.enabled = false;
                }
                else
                {
                    _characterSprite.enabled = true;
                    _characterSprite.SetNativeSize();
                }

                _characterSprite.transform.DOScale(_allyOnScreen.localScale, animTime);
                //characterSprite.transform.position = _allyOffScreen.position;
                _characterSprite.transform.DOMove(_allyOnScreen.position, animTime);
                _characterSprite.DOColor(Color.white, animTime);

                if(_enemySprite.gameObject.activeSelf)
                {
                    _enemySprite.transform.DOMove(_enemyInactive.position, animTime);
                    _enemySprite.transform.DOScale(_enemyInactive.localScale, animTime);
                    _enemySprite.DOColor(Color.grey, animTime);
                }
            }

            yield return new WaitForSeconds(animTime);
        }

        public void SwitchName(string personName, string displayName, Sprite _characterImage, bool enemySpeaking)
        {
            StartCoroutine(switchCharAnimation(.3f, personName, displayName, _characterImage, enemySpeaking));
        }

        IEnumerator HideUI(float duration)
        {
            if(_enemySprite.gameObject.activeSelf)
            {
                _enemySprite.transform.DOComplete();
                _enemySprite.transform.DOMove(_enemyOffScreen.position, .3f);
            }
            if(_characterSprite.gameObject.activeSelf)
            {
                _characterSprite.transform.DOComplete();
                _characterSprite.transform.DOMove(_allyOffScreen.position, .3f);
            }

            yield return new WaitForSeconds(.3f);

            _enemySprite.gameObject.SetActive(false);
            _characterSprite.gameObject.SetActive(false);
            convoElements[0].transform.DOMove(_offScreenPos.position, duration).SetEase(Ease.OutSine);

            yield return new WaitForSeconds(duration);

            foreach (GameObject convoPiece in convoElements)
            {
                _currentConvo = null;
                convoPiece.SetActive(false);
                this.gameObject.SetActive(false);
                //_turnManager.StartBattle();
            }

            if (_completeCallback != null)
            {
                _completeCallback.Invoke();
                _completeCallback = null;
            }
            else
            {
                ToggleDisabledObjects(true);
                _battleManager.DialogueFinished();
            }
        }
    }
}
