using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace StreetPerformers
{
    public class RewardUI : MonoBehaviour
    {
        [SerializeField]
        private bool _imagePanel;
        [Header("Positions")]
        [SerializeField]
        private Transform _leftPos;
        [SerializeField]
        private Transform _rightPos;
        [SerializeField]
        private Transform _endPos;

        [SerializeField]
        private DialogueManager _dialogue;

        private int _awardedExp;

        private bool _continue = false;

        private Vector3 _acePos;
        private float _posOffset;

        private bool _bonusRewarded = false;

        private bool _xpAwarded = false;

        private void OnEnable()
        {
            transform.Find("ContinueButton").gameObject.SetActive(false);
            BattleHandler.Instance.ApplyExperienceMult();
            _awardedExp = BattleHandler.Instance._experienceThisBattle / 2;

            if(_imagePanel)
            {
                string exp = "Awarded " + _awardedExp + " XP to Ace!";
                //exp += "\nChoose who to give the rest to.";
                transform.Find("ExperienceText").GetComponent<TextMeshProUGUI>().text = exp;

                CharacterRewardPanel("Ace", 0);

                foreach(string name in PartyHandler.Instance._partyMembers)
                {
                    transform.Find("Images").Find(name).gameObject.SetActive(true);
                }

                GetComponentInParent<DripMenu>().OnAnimationFinish += StartAceExperience;
            }
            else
            {
                string exp = "Award another " + _awardedExp + " XP!";
                //exp += "\nChoose who to give the rest to.";
                transform.Find("ExperienceText").GetComponent<TextMeshProUGUI>().text = exp;

                _acePos = transform.Find("Ace").position;
                _posOffset = transform.Find("Mime").position.y - _acePos.y;
                CharacterRewardPanel("Ace", 0);
                int index = 1;
                foreach(string name in PartyHandler.Instance._partyMembers)
                {
                    CharacterRewardPanel(name, index);
                    index++;
                }
            }
        }

        public void StartAceExperience()
        {
            StartCoroutine(AddExperience("Ace", _awardedExp));
        }

        private IEnumerator AddExperience(string charName, int expLeft)
        {
            Transform characterPanel = transform.Find(charName);
            Image expSlider = characterPanel.Find("ExpSlider").Find("FillArea").GetComponent<Image>();

            while(expLeft > 0)
            {
                int curExp = PartyHandler.Instance._characterExp[charName];
                int expToLevel = PartyHandler.Instance.ExperienceToLevel(PartyHandler.Instance._characterLevels[charName]) - curExp;

                if(expLeft >= expToLevel)
                {
                    PartyHandler.Instance.AddExperience(charName, expToLevel);
                    expLeft -= expToLevel;

                    expSlider.DOFillAmount(1f, 1f).OnComplete( 
                        delegate
                        {
                            LevelUp(charName);
                            UpdatePanelValues(charName);
                        });

                    yield return new WaitUntil(() => _continue);

                    _continue = false;
                }
                else
                {
                    break;
                }
            }

            if(expLeft == 0 || (charName != "Ace" && PartyHandler.Instance._characterLevels[charName] == 10))
            {
                EndLevel();
                UpdatePanelValues(charName);
            }
            else
            {
                PartyHandler.Instance.AddExperience(charName, expLeft);

                float maxValue = PartyHandler.Instance.ExperienceToLevel(PartyHandler.Instance._characterLevels[charName]);
                float value = ((float)PartyHandler.Instance._characterExp[charName]) / maxValue;
                expSlider.DOFillAmount(value, 1f).OnComplete(
                    delegate
                    {
                        EndLevel();
                        UpdatePanelValues(charName);
                    });

                expLeft = 0;
            }
            yield return null;
        }

        private void LevelUp(string charName, bool bonusBattleReward = false)
        {
            int level = PartyHandler.Instance._characterLevels[charName];

            GameObject levelReward = transform.parent.Find("LevelRewards").gameObject;
            levelReward.transform.position = _rightPos.position;
            levelReward.SetActive(true);
            levelReward.transform.DOMove(_endPos.position, .7f);

            LevelRewards rewardScript = levelReward.GetComponent<LevelRewards>();
            rewardScript.Initialize(charName, level, level, this, bonusBattleReward);

            transform.DOMove(_leftPos.position, .7f).OnComplete(
                delegate
                {
                    rewardScript.Activate();
                });

            if(_imagePanel)
            { return; }

            transform.Find("Ace").Find("LevelButton").gameObject.SetActive(false);
            foreach(string partyName in PartyHandler.Instance._partyMembers)
            {
                transform.Find(partyName).Find("LevelButton").gameObject.SetActive(false);
            }
        }

        private void CharacterRewardPanel(string name, int index)
        {
            UpdatePanelValues(name);

            if(!_imagePanel)
            {
                Transform trans = transform.Find(name);
                trans.position = _acePos + (index * new Vector3(0f, _posOffset, 0f));

                transform.Find(name).Find("LevelButton").GetComponent<Button>().onClick.AddListener(
                    delegate { LevelButtonClicked(name); });
            }
        }

        public void LevelButtonClicked(string name)
        {
            if(_xpAwarded)
            { return; }
            _xpAwarded = true;

            AnalyticsMessage(name);

            StartCoroutine(AddExperience(name, _awardedExp));

            string exp = "Awarded " + _awardedExp + " XP to " + name + "!";
            transform.Find("ExperienceText").GetComponent<TextMeshProUGUI>().text = exp;

            transform.Find("Ace").Find("LevelButton").gameObject.SetActive(false);
            foreach(string charName in PartyHandler.Instance._partyMembers)
            {
                transform.Find(charName).Find("LevelButton").gameObject.SetActive(false);
            }
        }

        private void AnalyticsMessage(string name)
        {
            //if(Application.isEditor)
            //{ return; }

            //Empty for now
        }

        private void EndLevel()
        {
            transform.Find("ContinueButton").gameObject.SetActive(true);
        }

        public void CloseLevelRewards()
        {
            GameObject levelReward = transform.parent.Find("LevelRewards").gameObject;
            levelReward.transform.DOMove(_leftPos.position, .7f).OnComplete(
                delegate
                {
                    levelReward.SetActive(false);
                });

            transform.position = _rightPos.position;
            transform.DOMove(_endPos.position, .7f).OnComplete(
                delegate
                {
                    _continue = true;
                });
        }

        private void UpdatePanelValues(string name)
        {
            Transform characterPanel = transform.Find(name);
            characterPanel.gameObject.SetActive(true);
            int level = PartyHandler.Instance._characterLevels[name];
            characterPanel.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + level;
            Image expSlider = characterPanel.Find("ExpSlider").Find("FillArea").GetComponent<Image>();
            float maxValue = PartyHandler.Instance.ExperienceToLevel(level);

            if(name != "Ace" && level == 10)
            {
                expSlider.fillAmount = 1f;
                characterPanel.Find("LevelButton").gameObject.SetActive(false);
            }
            else
            {
                expSlider.fillAmount = (float)PartyHandler.Instance._characterExp[name] / maxValue;
            }
        }

        public void Continue()
        {
            Save();
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            Time.timeScale = 1f;
            if(BattleHandler.Instance._afterConvo == null)
            {
                if(!_bonusRewarded)
                {
                    HubAreaData hub = ProgressionHandler.Instance._hubAreas[BattleHandler.Instance._activeHubName];
                    BattleData battle = hub._battles[BattleHandler.Instance._activeBattleIndex];
                    switch (battle._enemyList[0]._enemyName)
                    {
                        case "Contortionist":
                        case "Mascot":
                        case "Mime":
                            _bonusRewarded = true;
                            LevelUp(battle._enemyList[0]._enemyName, true);
                            return;
                    }
                }
                

#if UNITY_STANDALONE
                if(ProgressionHandler.Instance._isDemo)
                {
                    ProgressionHandler.Instance.LoadNextDemoBattle();
                }
                else
                {
                    SceneManager.LoadScene("HubWorldSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
                InitializeAds.AttemptShowAd();
                SceneManager.LoadScene("HubWorldScene");
#endif
                }

            }
            else
            {
                transform.Find("ContinueButton").gameObject.SetActive(false);
                _dialogue.StartConversation(false);
            }
        }

        public void ImagePanelContinue()
        {
            if (ProgressionHandler.Instance._isDemo && ProgressionHandler.Instance._demoType == DemoManager.DemoType.BATTLE)
            {
                SceneManager.LoadScene("EndSceneLandscape");
                return;
            }

            GameObject experiencePanel = transform.parent.Find("Experience").gameObject;
            experiencePanel.transform.position = _rightPos.position;
            experiencePanel.SetActive(true);
            experiencePanel.transform.DOMove(_endPos.position, .7f);

            transform.DOMove(_leftPos.position, .7f).OnComplete(
                delegate
                {
                    this.gameObject.SetActive(false);
                });
        }

        public void AddCharacterScreen(string name)
        {
            Transform memberPanel = transform.parent.Find("NewMember");
            memberPanel.gameObject.SetActive(true);
            memberPanel.position = _rightPos.position;
            memberPanel.GetComponent<NewMember>().Initialize(name);
            memberPanel.DOMove(_endPos.position, .7f);

            transform.DOMove(_leftPos.position, .7f).OnComplete(
                delegate
                {
                    this.gameObject.SetActive(false);
                });
        }

        private void Save()
        {
            BattleHandler.Instance.DefeatedEnemy();
            ProgressionHandler.Instance.SaveCurrentProgression();
        }
    }
}
