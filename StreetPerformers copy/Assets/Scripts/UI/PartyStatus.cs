using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class PartyStatus : MonoBehaviour
    {
        private enum PartySelectedObject
        {
            RETURN,
            REORDER,
            PARTY
        }

        [SerializeField]
        private GameObject _deckPanel;
        [SerializeField] private GameObject _reorderButtonObj = null;
        [SerializeField] private Transform _mascotButtonTransform = null;
        [SerializeField] private Transform _contortionistButtonTransform = null;
        [SerializeField] private Transform _mimeButtonTransform = null;

        private int _partyCount = 0;
        private int _reorderNum;
        private List<string> _reorderedList;

        [SerializeField] private Button _reorderButton = null;
        [SerializeField] private Image _reorderImage = null;
        [SerializeField] private Button _returnButton = null;
        [SerializeField] private Image _returnImage = null;

        private PartySelectedObject _currentSelectedObject;
        private int _lastDirection = 0;
        private float _bufferTime = .3f;
        private float _timer = 0f;

        private List<Button> _partyButtons = new List<Button>();
        private List<Image> _partyButtonImages = new List<Image>();
        private int _partyIndex = 0;

        private Color _disabledColor = new Color(.1f, .1f, .1f);


        private void OnEnable()
        {
            Button aceButton = transform.Find("Ace").GetComponent<Button>();
            aceButton.onClick.RemoveAllListeners();
            aceButton.onClick.AddListener(delegate { OpenDeckViewer("Ace"); });
            UpdatePanelValues("Ace");

            _partyCount = PartyHandler.Instance._partyMembers.Count;
            if(_partyCount <= 2)
            {
                if(_reorderButtonObj != null)
                {
                    _reorderButtonObj.SetActive(false);
                }

                Vector3 pos = transform.Find("Return").GetComponent<RectTransform>().localPosition;
                pos.x = 0;
                transform.Find("Return").GetComponent<RectTransform>().localPosition = pos;
            }

            int index = 1;
            foreach(string name in PartyHandler.Instance._partyMembers)
            {
                Transform character = transform.Find(name);
                character.gameObject.SetActive(true);

                Button charButton = character.GetComponent<Button>();
                charButton.onClick.RemoveAllListeners();
                charButton.onClick.AddListener(delegate { OpenDeckViewer(name); });
                UpdatePanelValues(name);

                if(_partyCount >= 2)
                {
                    character.Find("OrderBackground").gameObject.SetActive(true);
                    character.Find("OrderBackground").Find("PartyOrder").GetComponent<TextMeshProUGUI>().text = "" + index;
                }
                index++;
            }

            _returnImage.color = Color.grey;
            if(_reorderImage != null)
            {
                _reorderImage.color = Color.white;
            }
            _currentSelectedObject = PartySelectedObject.RETURN;
        }

        private void UpdatePanelValues(string name)
        {
            Transform trans = transform.Find(name);

            trans.gameObject.SetActive(true);
            int level = PartyHandler.Instance._characterLevels[name];
            trans.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lv. " + level;

            Image expSlider = trans.Find("ExpSlider").Find("FillArea").GetComponent<Image>();
            if(name != "Ace" && level == 10)
            {
                expSlider.fillAmount = 1f;
            }
            else
            {
                expSlider.fillAmount = (float)PartyHandler.Instance._characterExp[name] / (float)PartyHandler.Instance.ExperienceToLevel(level);
            }
        }

        public void OpenDeckViewer(string name)
        {
            DeckViewer deck = _deckPanel.GetComponent<DeckViewer>();
            deck.Initialize(name);
            deck.ToggleStatus();

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

        }

        public void StartReorder()
        {
            _reorderedList = new List<string>();
            _reorderNum = 1;

            transform.Find("Return").gameObject.SetActive(false);
            _reorderButtonObj.gameObject.SetActive(false);

            transform.Find("Ace").Find("CharacterImage").GetComponent<Image>().color = _disabledColor;
            transform.Find("Ace").GetComponent<Button>().onClick.RemoveAllListeners();

            _partyButtons.Clear();
            _partyButtonImages.Clear();
            for(int i = 0; i < 3; i++)
            {
                _partyButtons.Add(null);
                _partyButtonImages.Add(null);
            }

            foreach(string name in PartyHandler.Instance._partyMembers)
            {
                transform.Find(name).Find("OrderBackground").Find("PartyOrder").GetComponent<TextMeshProUGUI>().text = "";

                Button button = transform.Find(name).GetComponent<Button>();
                button.enabled = true;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { ReorderClick(name); });

                int index = 0;
                switch(name)
                {
                    case "Mascot":
                        index = 0;
                        break;
                    case "Contortionist":
                        index = 1;
                        break;
                    case "Mime":
                        index = 2;
                        break;
                }
                _partyButtons[index] = button;
                _partyButtonImages[index] = button.transform.Find("CharacterImage").GetComponent<Image>();
            }

            _partyButtonImages[0].color = Color.grey;
            _currentSelectedObject = PartySelectedObject.PARTY;
        }

        public void ReorderClick(string name)
        {
            _reorderedList.Add(name);

            Transform character;
            switch(name)
            {
                case "Mascot":
                    character = _mascotButtonTransform;
                    break;
                case "Contortionist":
                    character = _contortionistButtonTransform;
                    break;
                default:
                case "Mime":
                    character = _mimeButtonTransform;
                    break;
            }

            character.GetComponent<Button>().onClick.RemoveAllListeners();
            character.Find("OrderBackground").gameObject.SetActive(true);
            character.Find("OrderBackground").Find("PartyOrder").GetComponent<TextMeshProUGUI>().text = "" + _reorderNum;
            character.Find("CharacterImage").GetComponent<Image>().color = _disabledColor;

            _partyButtons[_partyIndex] = null;
            _partyButtonImages[_partyIndex] = null;

            _reorderNum++;
            if(_reorderNum > _partyCount)
            {
                EndReorder();
            }
            else
            {
                while(_partyButtons[_partyIndex] == null)
                {
                    _partyIndex++;
                    if(_partyIndex >= _partyButtons.Count)
                    {
                        _partyIndex = 0;
                    }
                }

                _partyButtonImages[_partyIndex].color = Color.grey;
            }
        }

        private void EndReorder()
        {
            PartyHandler.Instance.Reorder(_reorderedList);

            transform.Find("Ace").Find("CharacterImage").GetComponent<Image>().color = Color.white;
#if UNITY_IOS || UNITY_ANDROID
            transform.Find("Ace").GetComponent<Button>().onClick.AddListener(delegate { OpenDeckViewer("Ace"); });
#elif UNITY_STANDALONE
            transform.Find("Ace").GetComponent<Button>().enabled = false;
#endif

            foreach(string name in PartyHandler.Instance._partyMembers)
            {
                Transform character;
                switch (name)
                {
                    case "Mascot":
                        character = _mascotButtonTransform;
                        break;
                    case "Contortionist":
                        character = _contortionistButtonTransform;
                        break;
                    default:
                    case "Mime":
                        character = _mimeButtonTransform;
                        break;
                }

                character.Find("CharacterImage").GetComponent<Image>().color = Color.white;

                Button button = character.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
#if UNITY_IOS || UNITY_ANDROID
                button.onClick.AddListener(delegate { OpenDeckViewer(name); });
#endif
            }

            transform.Find("Return").gameObject.SetActive(true);
            _reorderButton.gameObject.SetActive(true);

            _reorderImage.color = Color.white;
            _returnImage.color = Color.grey;
            _returnButton.Select();
            _currentSelectedObject = PartySelectedObject.RETURN;
        }

        private void Update()
        {
            int direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            if(direction == 1)
            {
                //Move right
                if (_lastDirection != 1 || _timer <= 0f)
                {
                    _lastDirection = 1;
                    _timer = _bufferTime;

                    switch (_currentSelectedObject)
                    {
                        case PartySelectedObject.RETURN:
                            if(_reorderButtonObj == null || !_reorderButtonObj.activeInHierarchy)
                            { break; }

                            _returnImage.color = Color.white;

                            _reorderImage.color = Color.grey;
                            _reorderButton.Select();
                            _currentSelectedObject = PartySelectedObject.REORDER;
                            break;
                        case PartySelectedObject.PARTY:
                            int partyIndex = _partyIndex;
                            do
                            {
                                partyIndex++;
                                if(partyIndex >= _partyButtons.Count)
                                {
                                    partyIndex = 0;
                                }
                            }
                            while (_partyButtons[partyIndex] == null);

                            if(_partyIndex != partyIndex)
                            {
                                _partyButtonImages[_partyIndex].color = Color.white;

                                _partyButtonImages[partyIndex].color = Color.grey;
                                _partyButtons[partyIndex].Select();

                                _partyIndex = partyIndex;
                            }
                            break;
                    }
                }
            }
            else if(direction == -1)
            {
                //Move left
                if(_lastDirection != -1 || _timer <= 0f)
                {
                    _lastDirection = -1;
                    _timer = _bufferTime;

                    switch (_currentSelectedObject)
                    {
                        case PartySelectedObject.REORDER:
                            _reorderImage.color = Color.white;

                            _returnImage.color = Color.grey;
                            _returnButton.Select();
                            _currentSelectedObject = PartySelectedObject.RETURN;
                            break;
                        case PartySelectedObject.PARTY:
                            int partyIndex = _partyIndex;
                            do
                            {
                                partyIndex--;
                                if (partyIndex < 0)
                                {
                                    partyIndex = _partyButtons.Count - 1;
                                }
                            }
                            while (_partyButtons[partyIndex] == null);

                            if (_partyIndex != partyIndex)
                            {
                                _partyButtonImages[_partyIndex].color = Color.white;

                                _partyButtonImages[partyIndex].color = Color.grey;
                                _partyButtons[partyIndex].Select();

                                _partyIndex = partyIndex;
                            }
                            break;
                    }
                }
            }
            else
            {
                _lastDirection = 0;
            }

            if(_timer > 0f)
            {
                _timer -= Time.unscaledDeltaTime;
            }

            if(Input.GetButtonDown("Select"))
            {
                switch(_currentSelectedObject)
                {
                    case PartySelectedObject.RETURN:
                        _returnButton.onClick.Invoke();
                        break;
                    case PartySelectedObject.REORDER:
                        _reorderButton.onClick.Invoke();
                        break;
                    case PartySelectedObject.PARTY:
                        _partyButtons[_partyIndex].onClick.Invoke();
                        break;
                }
            }
        }
    }
}
