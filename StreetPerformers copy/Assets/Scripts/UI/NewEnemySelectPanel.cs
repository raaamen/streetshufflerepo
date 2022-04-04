using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

namespace StreetPerformers
{
    public class NewEnemySelectPanel : MonoBehaviour
    {
        //Struct that holds a rig and its associated level
        [Serializable]
        private struct SpriteLevel
        {
            public int _level;
            public Sprite _sprite;
        }
        [Serializable]
        private struct CharacterSprites
        {
            public string _characterName;
            public List<SpriteLevel> _spriteLevels;
        }

        [SerializeField] private Image _icon = null;
        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private TextMeshProUGUI _descriptionText = null;
        [SerializeField] private Image _background = null;

        [SerializeField] private List<Button> _enemyButtons = null;
        [SerializeField] private List<Image> _enemyButtonImages = null;
        [SerializeField] private List<TextMeshProUGUI> _enemyButtonTexts = null;

        [SerializeField] private List<EnemyDisplay> _enemyDisplays = null;
        [SerializeField] private GameObject _startButton = null;

        [SerializeField] private Image _topBackground = null;
        [SerializeField] private Image _titleImage = null;

        [SerializeField] private List<CharacterSprites> _characterSprites;

        private List<BattleData> _battles = null;
        private List<BattleData> _bonusBattles = null;
        private BattleData _currentBattle = null;
        private Image _currentBattleButtonImage = null;

        private string _hubWorld = "";
        private Sprite _backgroundSpr = null;
        private Color _crowdColor = Color.white;

        private int _currentBattleIndex = 0;
        private int _lastAvailableBattleIndex = 0;

        private int _buttonsPerRow = 3;

        private float _bufferTime = .3f;
        private float _timer = 0f;
        private Vector2Int _lastDirection = new Vector2Int(0, 0);


        public void Initialize(string hubWorld, Sprite icon, Sprite background, Color crowdColor, string titleText, string descriptionText, Sprite titleImage, Color backgroundColor)
        {
            _currentBattleButtonImage = null;

            _hubWorld = hubWorld;
            _backgroundSpr = background;
            _crowdColor = crowdColor;

            _topBackground.color = backgroundColor;

            _titleImage.sprite = titleImage;
            _titleImage.SetNativeSize();

            _icon.sprite = icon;
            if(hubWorld.Equals("Magician Tent"))
            {
                _icon.transform.localScale = new Vector3(.4f, .4f);
            }
            else
            {
                _icon.transform.localScale = new Vector3(.8f, .8f);
            }
            _icon.SetNativeSize();

            _background.sprite = background;
            _titleText.text = titleText;
            _descriptionText.text = descriptionText;

            Button startButton = _startButton.GetComponent<Button>();

            HubAreaData hub = ProgressionHandler.Instance._hubAreas[hubWorld];
            int hubLevel = hub._battleLevel;
            _battles = hub._battles;
            int i = 0;
            int nextAvailableBattle = 0;
            for(; i < _battles.Count; i++)
            {
                _enemyButtons[i].gameObject.SetActive(true);
                _enemyButtonImages[i].color = Color.white;

                BattleData battle = _battles[i];
                battle._hubIndex = i;

                if(!battle.BonusRequirementFulfilled())
                {
                    break;
                }

                if(!battle._playable)
                {
                    _enemyButtons[i].enabled = true;
                    _enemyButtons[i].interactable = false;
                    _enemyButtonImages[i].color = Color.white;
                    _enemyButtonTexts[i].gameObject.SetActive(true);
                    startButton.interactable = false;
                }
                else if(battle._requiredLevel > hubLevel || (i > 0 && !ProgressionHandler.Instance._tutorialFinished))
                {
                    _enemyButtons[i].enabled = false;
                    _enemyButtons[i].interactable = true;
                    _enemyButtonImages[i].color = Color.black;
                    _enemyButtonTexts[i].gameObject.SetActive(false);
                }
                else
                {
                    nextAvailableBattle = i;
                    _enemyButtons[i].enabled = true;
                    _enemyButtons[i].interactable = true;
                    _enemyButtonImages[i].color = Color.white;
                    _enemyButtonTexts[i].gameObject.SetActive(true);
                    startButton.interactable = true;

                    _lastAvailableBattleIndex = i;
                }
            }
            for(; i < _enemyButtons.Count; i++)
            {
                _enemyButtons[i].gameObject.SetActive(false);
            }

            BattleButtonClicked(nextAvailableBattle);

            this.gameObject.SetActive(true);
        }

        public void ButtonClicked(string id)
        {
            switch(id)
            {
                case "Close":
                    this.gameObject.SetActive(false);
                    break;
                case "Start":
                    StartBattle();
                    break;
            }
        }

        public void BattleButtonClicked(int index)
        {
            _currentBattleIndex = index;

            if(_currentBattleButtonImage != null)
            {
                _currentBattleButtonImage.color = Color.white;
            }
            _currentBattleButtonImage = _enemyButtonImages[index];
            _currentBattleButtonImage.color = Color.grey;

            _currentBattle = _battles[index];

            List<EnemyStruct> enemies = _currentBattle._enemyList;
            switch(enemies.Count)
            {
                case 1:
                    _enemyDisplays[0].gameObject.SetActive(true);
                    SetEnemyDisplay(enemies[0], 0);

                    _enemyDisplays[1].gameObject.SetActive(false);
                    _enemyDisplays[2].gameObject.SetActive(false);
                    break;
                case 2:
                    _enemyDisplays[0].gameObject.SetActive(false);

                    _enemyDisplays[1].gameObject.SetActive(true);
                    SetEnemyDisplay(enemies[0], 1);

                    _enemyDisplays[2].gameObject.SetActive(true);
                    SetEnemyDisplay(enemies[1], 2);
                    break;
                case 3:
                    _enemyDisplays[0].gameObject.SetActive(true);
                    SetEnemyDisplay(enemies[0], 0);

                    _enemyDisplays[1].gameObject.SetActive(true);
                    SetEnemyDisplay(enemies[1], 1);

                    _enemyDisplays[2].gameObject.SetActive(true);
                    SetEnemyDisplay(enemies[2], 2);
                    break;
                default:
                    break;
            }
        }

        private void SetEnemyDisplay(EnemyStruct enemy, int index)
        {
            _enemyDisplays[index].Initialize(GetEnemyDisplaySprite(enemy), enemy._enemyLevel, ProgressionHandler.Instance._encounteredEnemies.Contains(enemy._enemyName));
        }

        private Sprite GetEnemyDisplaySprite(EnemyStruct enemy)
        {
            CharacterSprites enemySpriteStruct = new CharacterSprites();
            foreach(CharacterSprites sprite in _characterSprites)
            {
                if(sprite._characterName.Equals(enemy._enemyName))
                {
                    enemySpriteStruct = sprite;
                    break;
                }

                if(sprite._characterName.Equals("Magician") && enemy._enemyName.Equals("Finale"))
                {
                    enemySpriteStruct = sprite;
                    break;
                }
            }

            switch(enemy._enemyName)
            {
                case "Singer":
                case "Musician":
                case "Clown":
                    foreach(SpriteLevel spriteLevel in enemySpriteStruct._spriteLevels)
                    {
                        if(spriteLevel._level == enemy._enemyLevel)
                        {
                            return spriteLevel._sprite;
                        }
                    }
                    break;
                default:
                    return enemySpriteStruct._spriteLevels[0]._sprite;
            }

            return null;
        }

        private void StartBattle()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Level_Select);

            ProgressionHandler progression = ProgressionHandler.Instance;

            BattleData battle = progression._hubAreas[_hubWorld]._battles[_currentBattle._hubIndex];

            foreach(EnemyStruct enemy in battle._enemyList)
            {
                if(!progression._encounteredEnemies.Contains(enemy._enemyName))
                {
                    progression._encounteredEnemies.Add(enemy._enemyName);
                }
            }

            BattleHandler.Instance.SetBattle(_hubWorld, _currentBattle._hubIndex, _backgroundSpr, null, _crowdColor);

#if UNITY_STANDALONE
            SceneManager.LoadScene("BattleSceneLandscape");
#elif UNITY_IPHONE || UNITY_ANDROID
                SceneManager.LoadScene("BattleScene");
#endif
        }

        private void Update()
        {
            int index = _currentBattleIndex;
            Vector2Int moveVec = new Vector2Int(Mathf.RoundToInt(Input.GetAxis("Horizontal")), Mathf.RoundToInt(Input.GetAxis("Vertical")));
            //Move right
            if(moveVec[0] == 1)
            {
                if(_lastDirection[0] != 1 || _timer <= 0f)
                {
                    _lastDirection[0] = 1;
                    /*if (index % _buttonsPerRow == _buttonsPerRow - 1 || index == _lastAvailableBattleIndex)
                    {
                        //wrap to the first in row
                        index -= (index % _buttonsPerRow);
                        if (index == 0 && !_battles[0]._playable)
                        {
                            index++;
                        }
                    }
                    else
                    {
                        index++;
                    }*/
                    index++;
                    if(index > _lastAvailableBattleIndex)
                    {
                        index = 0;
                    }
                    
                    if(!_battles[index]._playable)
                    {
                        index++;
                    }
                }
            }
            //Move left
            else if(moveVec[0] == -1)
            {
                if(_lastDirection[0] != -1 || _timer <= 0f)
                {
                    _lastDirection[0] = -1;
                    /*if (index % _buttonsPerRow == 0 || (index == 1 && !_battles[0]._playable))
                    {
                        //wrap to the last in row
                        index += (_buttonsPerRow - 1) - (index % _buttonsPerRow);
                        if (index > _lastAvailableBattleIndex)
                        {
                            index = _lastAvailableBattleIndex;
                        }
                    }
                    else
                    {
                        index--;
                    }*/
                    index--;
                    if(index < 0 || !_battles[index]._playable)
                    {
                        index = _lastAvailableBattleIndex;
                    }
                }
            }
            //Move up
            else if(moveVec[1] == 1)
            {
                if(_lastDirection[1] != 1 || _timer <= 0f)
                {
                    _lastDirection[1] = 1;
                    if (index / _buttonsPerRow < 1)
                    {
                        if(_lastAvailableBattleIndex >= _buttonsPerRow)
                        {
                            index = (_lastAvailableBattleIndex / _buttonsPerRow) * _buttonsPerRow + (index % _buttonsPerRow);
                            index = Mathf.Min(index, _lastAvailableBattleIndex);
                        }
                    }
                    else
                    {
                        index -= _buttonsPerRow;
                        if(!_battles[index]._playable)
                        {
                            index++;
                        }
                    }
                }
            }
            //Move down
            else if(moveVec[1] == -1)
            {
                if(_lastDirection[1] != -1 || _timer <= 0f)
                {
                    _lastDirection[1] = -1;
                    if(index >= _lastAvailableBattleIndex - (_lastAvailableBattleIndex % _buttonsPerRow))
                    {
                        index = index % _buttonsPerRow;
                        if(!_battles[index]._playable)
                        {
                            index++;
                        }
                    }
                    else
                    {
                        index += _buttonsPerRow;
                        index = Mathf.Min(index, _lastAvailableBattleIndex);
                    }
                }
            }
            else
            {
                _lastDirection[0] = 0;
                _lastDirection[1] = 0;
            }

            if(index != _currentBattleIndex)
            {
                _timer = _bufferTime;
                BattleButtonClicked(index);
            }

            if(_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
        }
    }
}

