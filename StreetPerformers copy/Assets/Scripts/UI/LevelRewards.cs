using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StreetPerformers
{
    public class LevelRewards : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _displayRight;
        [SerializeField]
        private RectTransform _displayMid;
        [SerializeField] private GameObject _continueButton = null;

        private GameObject _displayCard;
        private GameObject _displayBack;

        private string _charName;
        private int _level;
        private int _endLevel;
        private RewardUI _rewardScript;

        private bool _canContinue = false;

        private bool _bonusBattleReward = false;

        public void Initialize(string name, int startLevel, int endLevel, RewardUI rewardScript, bool bonusBattleReward = false)
        {
            _charName = name;
            _level = startLevel;
            _endLevel = endLevel;
            _rewardScript = rewardScript;
            _bonusBattleReward = bonusBattleReward;

            _continueButton.SetActive(false);

            AddLevelRewards();
        }

        private void AddLevelRewards()
        {
            for(int i = 1; i <= 2; i++)
            {
                transform.Find("Reward" + i).gameObject.SetActive(false);
            }

            transform.Find("Title").GetComponent<TextMeshProUGUI>().text = _charName + " has reached level " + _level;

            string number = "";
            switch(_level)
            {
                case 2:
                    number = "6";
                    break;
                case 3:
                    number = "7";
                    break;
                case 4:
                    number = "8";
                    break;
                case 5:
                    number = "9";
                    break;
                case 6:
                    number = "10";
                    break;
                case 7:
                    number = "Jack";
                    break;
                case 8:
                    number = "Queen";
                    break;
                case 9:
                    number = "King";
                    break;
                case 10:
                    number = "Ace";
                    break;
            }


            if(_charName == "Ace")
            {
                int index = 1;
                if(_level <= 10)
                {
                    RewardAtIndex(index, "- Gained the " + number + " of Spades");
                    index++;
                }

                int healthGain = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>()._healthPerLevel;
                RewardAtIndex(index, "- Gained " + healthGain + " max health");
            }
            else
            {
                string suit = "";
                switch(_charName)
                {
                    case "Contortionist":
                        suit = "Diamonds";
                        break;
                    case "Mascot":
                        suit = "Hearts";
                        break;
                    case "Mime":
                        suit = "Clubs";
                        break;
                    default:
                        break;
                }
                
                int index = 1;
                if(_bonusBattleReward)
                {
                    RewardAtIndex(index, "- Upgraded a card");
                    return;
                }
                else if(_level <= 10)
                {
                    RewardAtIndex(index, "- Gained the " + number + " of " + suit);
                }
                index++;

                switch(_level)
                {
                    case 3:
                        RewardAtIndex(index, "- Displays 3 cards during drafting");
                        index++;
                        break;
                    case 5:
                        RewardAtIndex(index, "- Can choose 2 cards during drafting");
                        index++;
                        break;
                    case 6:
                        RewardAtIndex(index, "- +1 to total energy");
                        index++;
                        break;
                    case 8:
                        RewardAtIndex(index, "- Displays 4 cards during drafting");
                        index++;
                        break;
                    case 9:
                        RewardAtIndex(index, "- Protects from a fatal blow to Ace. Disabled for the rest of battle.");
                        index++;
                        break;
                    case 10:
                        switch(_charName)
                        {
                            case "Contortionist":
                                RewardAtIndex(index, "- Ace now draws an extra card each turn.");
                                break;
                            case "Mascot":
                                RewardAtIndex(index, "- Ace now starts battle with 1 rage.");
                                break;
                            case "Mime":
                                RewardAtIndex(index, "- Ace now starts battle with 1 fortify");
                                break;
                            default:
                                break;
                        }
                        index++;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Activate()
        {
            if(_level <= 10 || _bonusBattleReward)
            {
                PopulateCard();
            }
            else
            {
                _canContinue = true;
                _continueButton.SetActive(true);
            }
        }

        private void PopulateCard()
        {
            CardScriptable scriptable;
            if(_bonusBattleReward)
            {
                string upgradeCardId = "";
                switch(_charName)
                {
                    case "Contortionist":
                        upgradeCardId = "Contortionist-Ace";
                        break;
                    case "Mascot":
                        upgradeCardId = "Mascot-13";
                        break;
                    case "Mime":
                        upgradeCardId = "Mime-13";
                        break;
                }
                scriptable = Resources.Load("ScriptableObjects/" + _charName + "/Upgraded/" + upgradeCardId) as CardScriptable;
            }
            else if(_level == 10)
            {
                scriptable = Resources.Load("ScriptableObjects/" + _charName + "/Party/" + _charName + "-Ace") as CardScriptable;
            }
            else
            {
                scriptable = Resources.Load("ScriptableObjects/" + _charName + "/Party/" + _charName + "-" + (_level + 4)) as CardScriptable;
            } 

            _displayCard = Instantiate(scriptable._cardPrefab, _displayRight);
            Card cardScr = _displayCard.GetComponent<Card>();
            cardScr.Initialize(scriptable);
            _displayBack = cardScr._backsideImage.gameObject;
            cardScr._cardMask.enabled = false;

            float flips = 3f;
            float time = 3f;
            _displayCard.transform.DOMove(_displayMid.position, time);
            _displayCard.transform.DORotate(new Vector3(0, flips * -360f, 0f), time).OnComplete(
                delegate { 
                    _canContinue = true;
                    _continueButton.SetActive(true);
                });
        }

        private void RewardAtIndex(int index, string rewardText)
        {
            transform.Find("Reward" + index).gameObject.SetActive(true);
            transform.Find("Reward" + index).GetComponent<TextMeshProUGUI>().text = rewardText;
        }

        public void Continue()
        {
            if(!_canContinue)
            { return; }

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            _canContinue = false;

            _continueButton.SetActive(false);
            if(_displayCard != null)
            {
                _displayCard?.transform.DOScale(0f, .5f).OnComplete(delegate { Close(); });
            }
            else
            {
                Close();
            }
        }

        public void Close()
        {
            Destroy(_displayCard);

            if(_level >= _endLevel)
            {
                _rewardScript.CloseLevelRewards();
                //this.gameObject.SetActive(false);
                return;
            }

            for(int i = 1; i <= 2; i++)
            {
                transform.Find("Reward" + i).gameObject.SetActive(false);
            }
            _level++;
            AddLevelRewards();
        }

        private void Update()
        {
            if(_displayCard != null)
            {
                float angle = _displayCard.transform.eulerAngles.y;
                if(!_displayBack.activeSelf && angle <= 270f && angle >= 90f)
                {
                    _displayBack.SetActive(true);
                }
                else if(_displayBack.activeSelf && (angle < 90f || angle > 270f))
                {
                    _displayBack.SetActive(false);
                } 
            }

            if(Input.GetMouseButtonDown(0))
            {
                Continue();
            }
        }
    }

}
