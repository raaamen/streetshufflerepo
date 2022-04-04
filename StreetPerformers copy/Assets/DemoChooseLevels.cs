using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class DemoChooseLevels : MonoBehaviour
    {
        [SerializeField] private List<Image> _characterImages = new List<Image>();
        [SerializeField] private List<Button> _levelUpButtons = new List<Button>();
        [SerializeField] private List<Button> _levelDownButtons = new List<Button>();
        [SerializeField] private List<TextMeshProUGUI> _characterLevelTexts = new List<TextMeshProUGUI>();

        [SerializeField] private TextMeshProUGUI _levelsLeftText = null;

        [SerializeField] private Material _highlightMaterial = null;

        private string _levelText = "";

        private int _characterIndex = 0;
        private Vector2Int _lastDirection = Vector2Int.zero;
        private float _bufferTime = .3f;
        private float _timer = 0f;

        private List<int> _characterLevels = new List<int>();

        private int _startLevels = 10;
        private int _levelsLeft = 0;

        private void OnEnable()
        {
            _characterIndex = 0;

            _levelsLeft = _startLevels;
            _levelText = $"{_levelsLeft} levels left!";
            _levelsLeftText.text = _levelText;

            _characterImages[_characterIndex].material = _highlightMaterial;

            _characterLevels = new List<int>();

            for(int i = 0; i < _characterImages.Count; i++)
            {
                _characterLevels.Add(1);
                _characterLevelTexts[i].text = "Lvl" + _characterLevels[i];

                _levelUpButtons[i].interactable = _characterLevels[i] != 10;
                _levelDownButtons[i].interactable = _characterLevels[i] != 1;
            }
        }

        public void OnButtonClicked(string button)
        {
            switch(button)
            {
                case "Start":
                    //Start the battle
                    PartyHandler party = PartyHandler.Instance;
                    int aceLevel = _characterLevels[0];

                    List<string> partyNames = new List<string>();
                    partyNames.Add("Mascot");
                    partyNames.Add("Contortionist");
                    partyNames.Add("Mime");

                    List<int> partyLevels = new List<int>();
                    partyLevels.Add(_characterLevels[1]);
                    partyLevels.Add(_characterLevels[2]);
                    partyLevels.Add(_characterLevels[3]);

                    party.SetParty(aceLevel, partyNames, partyLevels);

                    ProgressionHandler.Instance.LoadNextDemoBattle();
                    break;
                case "LevelUp":
                    LevelUp();
                    break;
                case "LevelDown":
                    LevelDown();
                    break;
            }
        }

        private void LevelUp()
        {
            if (_characterLevels[_characterIndex] >= 10 || _levelsLeft <= 0)
            { return; }

            _characterLevels[_characterIndex]++;
            _characterLevelTexts[_characterIndex].text = $"Lvl {_characterLevels[_characterIndex]}";

            _levelsLeft--;

            UpdateLevels();
        }

        private void LevelDown()
        {
            if (_characterLevels[_characterIndex] <= 1 || _levelsLeft >= _startLevels)
            { return; }

            _characterLevels[_characterIndex]--;
            _characterLevelTexts[_characterIndex].text = $"Lvl {_characterLevels[_characterIndex]}";

            _levelsLeft++;

            UpdateLevels();
        }

        private void UpdateLevels()
        {
            _levelText = $"{_levelsLeft} levels left!";
            _levelsLeftText.text = _levelText;

            _levelUpButtons[_characterIndex].interactable = _characterLevels[_characterIndex] != 10;
            _levelDownButtons[_characterIndex].interactable = _characterLevels[_characterIndex] != 1;
        }

        private void Update()
        {
            Vector2Int input = new Vector2Int(Mathf.RoundToInt(Input.GetAxis("Horizontal")), Mathf.RoundToInt(Input.GetAxis("Vertical")));
            int index = _characterIndex;

            if (input[1] != 0)
            {
                if(input[1] != _lastDirection[1] || _timer <= 0f)
                {
                    _lastDirection[1] = input[1];
                    _timer = _bufferTime;

                    index -= input[1];
                    if(index > _characterLevels.Count - 1)
                    {
                        index = 0;
                    }
                    else if(index < 0)
                    {
                        index = _characterLevels.Count - 1;
                    }

                    if(index != _characterIndex)
                    {
                        _characterImages[_characterIndex].material = null;

                        _characterIndex = index;
                        _characterImages[_characterIndex].material = _highlightMaterial;
                    }
                }
            }
            else if(input[0] != 0)
            {
                if(input[0] != _lastDirection[0] || _timer <= 0f)
                {
                    _lastDirection[0] = input[0];
                    _timer = _bufferTime;

                    if(input[0] == 1)
                    {
                        LevelUp();
                    }
                    else
                    {
                        LevelDown();
                    }
                }
            }
            else
            {
                _lastDirection = input;
            }

            if(_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
        }
    }
}
