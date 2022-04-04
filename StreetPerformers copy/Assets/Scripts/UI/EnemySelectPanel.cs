using StreetPerformers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class EnemySelectPanel : MonoBehaviour, IDragHandler
    {
        [SerializeField]
        private GameObject _buttonPrefab;
        [SerializeField]
        private Sprite _unplayableButtonImage;

        private List<GameObject> _buttons;

        private RectTransform _buttonPos1;
        private RectTransform _buttonPos2;
        private float _buttonOffset;
        private int _buttonIndex = 0;

        private HubArea _activeHub;

        private void Awake()
        {
            _buttons = new List<GameObject>();

            _buttonPos1 = transform.Find("Enemy1").GetComponent<RectTransform>();
            _buttonPos2 = transform.Find("Enemy2").GetComponent<RectTransform>();
            _buttonOffset = _buttonPos2.position.y - _buttonPos1.position.y;
        }

        public void Populate(HubArea hubArea)
        {
            _activeHub = hubArea;

            //Separate into 3 tiers,
            //  Not completed first in order of unlock
            //  Completed but repeatable second in any order
            //  Completed and not repeatable third

            HubAreaData hubData = ProgressionHandler.Instance._hubAreas[hubArea._idString];
            int progressionLevel = hubData._battleLevel;

            List<BattleData> repeatable = new List<BattleData>();
            List<BattleData> nonRepeatable = new List<BattleData>();
            for(int i = 0; i < hubData._battles.Count; i++)
            {
                BattleData battle = hubData._battles[i];
                battle._hubIndex = i;

                if(battle._requiredLevel > progressionLevel || (i > 0 && !ProgressionHandler.Instance._tutorialFinished))
                {
                    continue; 
                }

                if(!battle._playable)
                {
                    nonRepeatable.Add(battle);
                    continue;
                }

                if(battle._defeated)
                {
                    repeatable.Add(battle);
                    continue;
                }

                AddButton(battle);
            }

            foreach(BattleData battle in repeatable)
            {
                AddButton(battle);
            }

            foreach(BattleData battle in nonRepeatable)
            {
                AddButton(battle);
            }
        }

        private void AddButton(BattleData battle)
        {
            Vector3 pos = _buttonPos1.transform.position;
            pos.y += (_buttonIndex * _buttonOffset);
            GameObject buttonObj = Instantiate(_buttonPrefab, _buttonPos1);
            buttonObj.GetComponent<RectTransform>().position = pos;
            _buttons.Add(buttonObj);
            _buttonIndex++;

            string levelString = "";
            string nameString = "";
            for(int i = 0; i < battle._enemyList.Count; i++)
            {
                levelString += "Lv" + battle._enemyList[i]._enemyLevel;

                string enemyName = battle._enemyList[i]._enemyName;
                if(ProgressionHandler.Instance._encounteredEnemies.Contains(enemyName))
                {
                    nameString += battle._enemyList[i]._enemyName;
                }
                else
                {
                    nameString += "???";
                }

                if(i < battle._enemyList.Count)
                {
                    levelString += "\n";
                    nameString += "\n";
                }
            }

            TextMeshProUGUI level = buttonObj.transform.Find("EnemyLevels").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI names = buttonObj.transform.Find("EnemyNames").GetComponent<TextMeshProUGUI>();
            level.text = levelString;
            names.text = nameString;
            names.fontSize = level.fontSize;

            Button button = buttonObj.GetComponent<Button>();
            if(battle._playable)
            {
                button.onClick.AddListener(
                    delegate 
                    { 
                        _activeHub.BattleButton(battle._hubIndex); 
                    });
            }
            else
            {
                buttonObj.GetComponent<Image>().sprite = _unplayableButtonImage;
                button.interactable = false;
            }

            if(battle._defeated)
            {
                buttonObj.transform.Find("ClearStamp").gameObject.SetActive(true);
            }
            else if(!battle._attempted)
            {
                buttonObj.transform.Find("NewStamp").gameObject.SetActive(true);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            float dragAmount = eventData.delta.y;

            Vector3[] corners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(corners);

            Vector3[] topCorners = new Vector3[4];
            _buttons[0].GetComponent<RectTransform>().GetWorldCorners(topCorners);
            if(topCorners[1].y + dragAmount < corners[1].y)
            {
                return;
            }

            Vector3[] botCorners = new Vector3[4];
            _buttons[_buttons.Count - 1].GetComponent<RectTransform>().GetWorldCorners(botCorners);
            if(botCorners[0].y + dragAmount > corners[0].y)
            {
                return;
            }

            foreach(GameObject button in _buttons)
            {
                Vector2 pos = button.transform.position;
                pos.y += eventData.delta.y;
                button.transform.position = pos;
            }
        }

        public void ClosePanel()
        {
            foreach(GameObject button in _buttons)
            {
                Destroy(button);
            }
            _buttons.Clear();
            _buttonIndex = 0;
        }
    }
}

