using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class MenuControls : MonoBehaviour
    {
        private enum SelectedObject
        {
            SOUND,
            MUSIC,
            PARTY,
            GLOSSARY,
            QUIT
        }

        [SerializeField] private Slider _soundSlider = null;
        [SerializeField] private TextMeshProUGUI _soundText = null;
        [SerializeField] private Slider _musicSlider = null;
        [SerializeField] private TextMeshProUGUI _musicText = null;
        [SerializeField] private Button _partyButton = null;
        [SerializeField] private Button _glossaryButton = null;
        [SerializeField] private Button _quitToTitleButton = null;
        [SerializeField] private Button _demoQuitButton = null;

        [SerializeField] private List<GameObject> _disabledIfActiveList = new List<GameObject>();

        private Image _partyImage = null;
        private Color _partyColor = Color.white;

        private Image _glossaryImage = null;
        private Color _glossaryColor = Color.white;

        private Image _quitImage = null;
        private Color _quitColor = Color.white;

        private Image _demoQuitImage = null;
        private Color _demoQuitColor = Color.white;

        private Vector2Int _lastDirection = new Vector2Int(0, 0);
        private float _bufferTime = .3f;
        private float _timer = 0f;

        private SelectedObject _currentSelectedObject = SelectedObject.SOUND;

        private void Awake()
        {
            _partyImage = _partyButton.GetComponent<Image>();
            _partyColor = _partyImage.color;

            _glossaryImage = _glossaryButton.GetComponent<Image>();
            _glossaryColor = _glossaryImage.color;

            _quitImage = _quitToTitleButton.GetComponent<Image>();
            _quitColor = _quitImage.color;

            if(_demoQuitButton != null)
            {
                _demoQuitImage = _demoQuitButton.GetComponent<Image>();
                _demoQuitColor = _demoQuitImage.color;
            }
        }

        public void OnEnable()
        {
            _soundSlider.Select();
            _soundText.color = Color.grey;
            _currentSelectedObject = SelectedObject.SOUND;

            _musicText.color = Color.white;

            _partyImage.color = _partyColor;
            _glossaryImage.color = _glossaryColor;
            _quitImage.color = _quitColor;
            _demoQuitImage.color = _demoQuitColor;

            if (!_glossaryButton.gameObject.activeInHierarchy)
            {
                Vector3 pos = _partyButton.transform.localPosition;
                pos.x = 0f;
                _partyButton.transform.localPosition = pos;
            }
        }

        private void Update()
        {
            foreach (GameObject obj in _disabledIfActiveList)
            {
                if(obj.activeInHierarchy)
                { return; }
            }

            Vector2Int inputVec = new Vector2Int(Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")), Mathf.RoundToInt(Input.GetAxisRaw("Vertical")));

            if (inputVec[0] == 1 && (_lastDirection[0] != 1 || _timer <= 0f))
            {
                _lastDirection[0] = 1;
                _timer = _bufferTime;

                //Move right
                switch (_currentSelectedObject)
                {
                    case SelectedObject.PARTY:
                        if(_glossaryButton == null || !_glossaryButton.gameObject.activeInHierarchy)
                        {
                            _partyButton.Select();
                            break; 
                        }

                        _partyImage.color = _partyColor;

                        _glossaryButton.Select();
                        _glossaryImage.color = Color.grey;
                        _currentSelectedObject = SelectedObject.GLOSSARY;
                        break;
                }
            }
            else if (inputVec[0] == -1 && (_lastDirection[0] != -1 || _timer <= 0f))
            {
                _lastDirection[0] = -1;
                _timer = _bufferTime;

                //Move left
                switch (_currentSelectedObject)
                {
                    case SelectedObject.GLOSSARY:
                        _glossaryImage.color = _glossaryColor;

                        _partyButton.Select();
                        _partyImage.color = Color.grey;
                        _currentSelectedObject = SelectedObject.PARTY;
                        break;
                }
            }
            else if (inputVec[1] == 1 && (_lastDirection[1] != 1 || _timer <= 0f))
            {
                _lastDirection[1] = 1;
                _timer = _bufferTime;

                //move up
                switch(_currentSelectedObject)
                {
                    case SelectedObject.SOUND:
                        _soundText.color = Color.white;

                        if(_demoQuitButton.gameObject.activeInHierarchy)
                        {
                            _demoQuitButton.Select();
                            _demoQuitImage.color = Color.grey;
                        }
                        else
                        {
                            _quitToTitleButton.Select();
                            _quitImage.color = Color.grey;
                        }
                        _currentSelectedObject = SelectedObject.QUIT;
                        break;
                    case SelectedObject.MUSIC:
                        _musicText.color = Color.white;

                        _soundSlider.Select();
                        _soundText.color = Color.grey;
                        _currentSelectedObject = SelectedObject.SOUND;
                        break;
                    case SelectedObject.PARTY:
                    case SelectedObject.GLOSSARY:
                        _partyImage.color = _partyColor;
                        _glossaryImage.color = _glossaryColor;

                        _musicSlider.Select();
                        _musicText.color = Color.grey;
                        _currentSelectedObject = SelectedObject.MUSIC;
                        break;
                    case SelectedObject.QUIT:
                        if(_demoQuitButton != null)
                        {
                            _demoQuitImage.color = _demoQuitColor;
                        }
                        else
                        {
                            _quitImage.color = _quitColor;
                        }

                        _partyButton.Select();
                        _partyImage.color = Color.grey;
                        _currentSelectedObject = SelectedObject.PARTY;
                        break;
                }
            }
            else if(inputVec[1] == -1 && (_lastDirection[1] != -1 || _timer <= 0f))
            {
                _lastDirection[1] = -1;
                _timer = _bufferTime;

                //move down
                switch (_currentSelectedObject)
                {
                    case SelectedObject.SOUND:
                        _soundText.color = Color.white;

                        _musicSlider.Select();
                        _musicText.color = Color.grey;
                        _currentSelectedObject = SelectedObject.MUSIC;
                        break;
                    case SelectedObject.MUSIC:
                        _musicText.color = Color.white;

                        _partyButton.Select();
                        _partyImage.color = Color.grey;
                        _currentSelectedObject = SelectedObject.PARTY;
                        break;
                    case SelectedObject.PARTY:
                    case SelectedObject.GLOSSARY:
                        _partyImage.color = _partyColor;
                        _glossaryImage.color = _glossaryColor;

                        if(_demoQuitButton != null)
                        {
                            _demoQuitButton.Select();
                            _demoQuitImage.color = Color.grey;
                        }
                        else
                        {
                            _quitToTitleButton.Select();
                            _quitImage.color = Color.grey;
                        }
                        _currentSelectedObject = SelectedObject.QUIT;
                        break;
                    case SelectedObject.QUIT:
                        if(_demoQuitButton != null)
                        {
                            _demoQuitImage.color = _demoQuitColor;
                        }
                        else
                        {
                            _quitImage.color = _quitColor;
                        }

                        _soundSlider.Select();
                        _soundText.color = Color.grey;
                        _currentSelectedObject = SelectedObject.SOUND;
                        break;
                }
            }
            else if(inputVec.magnitude == 0)
            {
                _lastDirection[0] = 0;
                _lastDirection[1] = 0;
            }

            if(_timer > 0f)
            {
                _timer -= Time.unscaledDeltaTime;
            }

            if(Input.GetButtonDown("Select"))
            {
                switch(_currentSelectedObject)
                {
                    case SelectedObject.PARTY:
                        _partyButton.onClick.Invoke();
                        break;
                    case SelectedObject.GLOSSARY:
                        _glossaryButton.onClick.Invoke();
                        break;
                    case SelectedObject.QUIT:
                        if(_demoQuitButton != null)
                        {
                            _demoQuitButton.onClick.Invoke();
                        }
                        else
                        {
                            _quitToTitleButton.onClick.Invoke();
                        }
                        break;
                }
            }
        }
    }
}