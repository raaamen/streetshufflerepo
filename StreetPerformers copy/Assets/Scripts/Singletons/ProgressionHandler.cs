using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SaveManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

namespace StreetPerformers
{
    /// <summary>
    /// Holds data for which set of enemies to fight next.
    /// </summary>
    public class ProgressionHandler : Singleton<ProgressionHandler>
    {
        public enum Area
        {
            SUBWAY,
            PIER,
            SHOP,
            PARK,
            CAPITOL,
            GRAFFITI,
            TENT
        }

        [HideInInspector]
        public Dictionary<string, HubAreaData> _hubAreas;

        //HubArea[] _hubAreaObjects;
        [HideInInspector]
        public List<string> _encounteredEnemies;

        [HideInInspector]
        public UnityAction OnLevelUp;

        private List<string> _completedAreas;
        //private int _areaCompletion;
        
        
        /// control the saveFile.Q 
        public LoadFile _saveFile;

        private List<KeyValuePair<string, int>> _defeatedEnemies;
        private List<KeyValuePair<string, int>> _attemptedBattles;
        /// 
        /// NEED TO SAVE _areaCompletion, _areaUnlocked, _areaLevel, _defeatedEnemies, as well as the current character information
        ///

        private bool _justStarted = true;

        [HideInInspector]
        public float _adCooldownTimer = 0f;
        [HideInInspector]
        public float _adSuccessCooldownTimer = 6f * 60f;

        [HideInInspector]
        public bool _tutorialFinished = false;
        [HideInInspector]
        public bool _officialTutorialFinished = false;

        [HideInInspector]
        public bool _isDemo = false;
        [HideInInspector]
        public DemoManager.DemoType _demoType = DemoManager.DemoType.FULL;
        private List<BattleConfigData> _demoFullBattles = new List<BattleConfigData>();
        private List<BattleConfigData> _demoTutorialBattles = new List<BattleConfigData>();
        private List<BattleConfigData> _demoBattleBattles = new List<BattleConfigData>();
        [HideInInspector] public int _demoIndex = 0;


        private void Awake()
        {
            //_hubAreaObjects = GameObject.FindObjectsOfType<HubArea>();
        }

        public void LoadData(LoadFile saveFile)
        {
            _saveFile = saveFile;

            _justStarted = true;
            _tutorialFinished = false;
            _officialTutorialFinished = saveFile.GetFinishedTutorial();

            _completedAreas = new List<string>();

            if(saveFile.GetDefeatedEnemies().Count > 0)
            {
                this._defeatedEnemies = saveFile.GetDefeatedEnemies();
            }
            else
            {
                this._defeatedEnemies = new List<KeyValuePair<string, int>>();
            }

            if(saveFile.GetAttemptedBattles().Count > 0)
            {
                _attemptedBattles = saveFile.GetAttemptedBattles();
            }
            else
            {
                _attemptedBattles = new List<KeyValuePair<string, int>>();
            }

            _encounteredEnemies = saveFile.GetEncounteredEnemies();

            SetHubAreas();

            foreach(KeyValuePair<string, int> attempt in _attemptedBattles)
            {
                _hubAreas[attempt.Key]._battles[attempt.Value]._attempted = true;
            }

            BattleHandler.Instance._activeHubName = saveFile.GetSpawnArea();
        }

        private void SetHubAreas()
        {

            _hubAreas = new Dictionary<string, HubAreaData>();

            HubAreaData subwayData = new HubAreaData(false, 0, 6);
            _hubAreas.Add("Subway", subwayData);
            subwayData._battles.Add(new BattleData(0, new EnemyStruct("Mime", 1)));
            subwayData.AddBeforeCutscene(0, "Conversations/Subway/mimeBefore");
            subwayData.AddAfterCutscene(0, "Conversations/Subway/mimeAfter");
            subwayData._battles.Add(new BattleData(1, new EnemyStruct("Clown", 1), new EnemyStruct("Clown", 1)));
            subwayData._battles.Add(new BattleData(2, new EnemyStruct("Clown", 4)));
            subwayData._battles.Add(new BattleData(3, new EnemyStruct("Statue", 1)));
            subwayData.AddBeforeCutscene(3, "Conversations/Subway/statueBefore");
            subwayData.AddAfterCutscene(3, "Conversations/Subway/statueAfter");
            subwayData._battles.Add(new BattleData(4, new EnemyStruct("Clown", 4), new EnemyStruct("Singer", 1), new EnemyStruct("Singer", 1)));
            subwayData._battles.Add(new BattleData(5, new EnemyStruct("Statue", 1), new EnemyStruct("Singer", 1), new EnemyStruct("Musician", 1)));
            //Bonus Battles
            subwayData._battles.Add(new BattleData(6, BonusLevelRequirement.MIME_MAX, new EnemyStruct("Mime", 10)));

            HubAreaData pierData = new HubAreaData(false, 0, 6);
            _hubAreas.Add("Pier", pierData);
            pierData._battles.Add(new BattleData(0, new EnemyStruct("Contortionist", 1)));
            pierData.AddBeforeCutscene(0, "Conversations/Pier/contortBefore");
            pierData.AddAfterCutscene(0, "Conversations/Pier/contortAfter");
            pierData._battles.Add(new BattleData(1, new EnemyStruct("Singer", 1)));
            pierData._battles.Add(new BattleData(2, new EnemyStruct("Wild Guy", 1)));
            pierData._battles[2].SetMusicTrack("event:/Music/WildGuy_Battle");
            pierData.AddBeforeCutscene(2, "Conversations/Pier/wildoneBefore");
            pierData.AddAfterCutscene(2, "Conversations/Pier/wildoneAfter");
            pierData._battles.Add(new BattleData(3, new EnemyStruct("Singer", 4), new EnemyStruct("Clown", 1)));
            pierData._battles.Add(new BattleData(4, new EnemyStruct("Singer", 4), new EnemyStruct("Musician", 1), new EnemyStruct("Musician", 1)));
            pierData._battles.Add(new BattleData(5, new EnemyStruct("Singer", 4), new EnemyStruct("Clown", 4)));
            //Bonus Battles
            pierData._battles.Add(new BattleData(6, BonusLevelRequirement.CONTORTIONIST_MAX, new EnemyStruct("Contortionist", 10)));

            HubAreaData shopData = new HubAreaData(true, 0, 6);
            _hubAreas.Add("Corner Store", shopData);
            shopData._battles.Add(new BattleData(0, new EnemyStruct("Mascot", 1)));
            shopData.AddBeforeCutscene(0, "Conversations/Shop/kokoBefore");
            shopData.AddAfterCutscene(0, "Conversations/Shop/kokoAfter");
            shopData._battles.Add(new BattleData(1, new EnemyStruct("Cop", 1)));
            shopData._battles[1].SetMusicTrack("event:/Music/Cop_Battle");
            shopData.AddBeforeCutscene(1, "Conversations/Shop/officerBefore");
            shopData.AddAfterCutscene(1, "Conversations/Shop/officerAfter");
            shopData._battles.Add(new BattleData(2, new EnemyStruct("Musician", 4)));
            shopData._battles.Add(new BattleData(3, new EnemyStruct("Singer", 1), new EnemyStruct("Musician", 1), new EnemyStruct("Clown", 1)));
            shopData._battles.Add(new BattleData(4, new EnemyStruct("Clown", 4), new EnemyStruct("Musician", 4)));
            shopData._battles.Add(new BattleData(5, new EnemyStruct("Musician", 7)));
            //Bonus Battles
            shopData._battles.Add(new BattleData(6, BonusLevelRequirement.MASCOT_MAX, new EnemyStruct("Mascot", 10)));

            HubAreaData parkData = new HubAreaData(false, 0, 8);
            _hubAreas.Add("Park", parkData);
            parkData._battles.Add(new BattleData(0, new EnemyStruct("Clown", 7), new EnemyStruct("Clown", 1)));
            parkData._battles.Add(new BattleData(1, new EnemyStruct("Statue", 4)));
            parkData.AddBeforeCutscene(1, "Conversations/Park/statue2Before");
            parkData.AddAfterCutscene(1, "Conversations/Park/statue2After");
            parkData._battles.Add(new BattleData(2, new EnemyStruct("Clown", 10)));
            parkData._battles.Add(new BattleData(3, new EnemyStruct("Clown", 7), new EnemyStruct("Singer", 4), new EnemyStruct("Musician", 1)));
            parkData._battles.Add(new BattleData(4, new EnemyStruct("Clown", 10), new EnemyStruct("Clown", 4), new EnemyStruct("Clown", 4)));
            parkData._battles.Add(new BattleData(5, new EnemyStruct("Clown", 10), new EnemyStruct("Musician", 7), new EnemyStruct("Singer", 4)));
            parkData._battles.Add(new BattleData(6, new EnemyStruct("Statue", 7)));
            parkData.AddBeforeCutscene(6, "Conversations/Park/statue3Before");
            parkData.AddAfterCutscene(6, "Conversations/Park/statue3After");
            parkData._battles.Add(new BattleData(7, new EnemyStruct("Singer", 7), new EnemyStruct("Musician", 4)));
            //Bonus Battles
            parkData._battles.Add(new BattleData(8, BonusLevelRequirement.MAGICIAN_DEFEATED, new EnemyStruct("Clown", 10), new EnemyStruct("Clown", 10), new EnemyStruct("Clown", 10)));

            HubAreaData capitolData = new HubAreaData(false, 0, 8);
            _hubAreas.Add("Capitol", capitolData);
            capitolData._battles.Add(new BattleData(0, new EnemyStruct("Musician", 7)));
            capitolData._battles.Add(new BattleData(1, new EnemyStruct("Cop", 4)));
            capitolData._battles[1].SetMusicTrack("event:/Music/Cop_Battle");
            capitolData.AddAfterCutscene(1, "Conversations/Capital/officer2After");
            capitolData.AddBeforeCutscene(1, "Conversations/Capital/officer2Before");
            capitolData._battles.Add(new BattleData(2, new EnemyStruct("Musician", 7), new EnemyStruct("Singer", 4)));
            capitolData._battles.Add(new BattleData(3, new EnemyStruct("Musician", 7), new EnemyStruct("Musician", 1), new EnemyStruct("Clown", 1)));
            capitolData._battles.Add(new BattleData(4, new EnemyStruct("Musician", 10)));
            capitolData._battles.Add(new BattleData(5, new EnemyStruct("Musician", 10), new EnemyStruct("Singer", 4), new EnemyStruct("Singer", 4)));
            capitolData._battles.Add(new BattleData(6, new EnemyStruct("Cop", 7)));
            capitolData._battles[6].SetMusicTrack("event:/Music/Cop_Battle");
            capitolData.AddAfterCutscene(6, "Conversations/Capital/officer3After");
            capitolData.AddBeforeCutscene(6, "Conversations/Capital/officer3Before");
            capitolData._battles.Add(new BattleData(7, new EnemyStruct("Musician", 7), new EnemyStruct("Singer", 7), new EnemyStruct("Clown", 7)));
            //Bonus Battles
            capitolData._battles.Add(new BattleData(8, BonusLevelRequirement.MAGICIAN_DEFEATED, new EnemyStruct("Musician", 10), new EnemyStruct("Musician", 10), new EnemyStruct("Musician", 10)));

            HubAreaData graffitiData = new HubAreaData(false, 0, 8);
            _hubAreas.Add("Graffiti", graffitiData);
            graffitiData._battles.Add(new BattleData(0, new EnemyStruct("Wild Guy", 4)));
            graffitiData._battles[0].SetMusicTrack("event:/Music/WildGuy_Battle");
            graffitiData.AddBeforeCutscene(0, "Conversations/Graffiti/wildone2Before");
            graffitiData.AddAfterCutscene(0, "Conversations/Graffiti/wildone2After");
            graffitiData._battles.Add(new BattleData(1, new EnemyStruct("Singer", 7)));
            graffitiData._battles.Add(new BattleData(2, new EnemyStruct("Singer", 7), new EnemyStruct("Clown", 1), new EnemyStruct("Musician", 1)));
            graffitiData._battles.Add(new BattleData(3, new EnemyStruct("Singer", 10)));
            graffitiData._battles.Add(new BattleData(4, new EnemyStruct("Singer", 10), new EnemyStruct("Musician", 7)));
            graffitiData._battles.Add(new BattleData(5, new EnemyStruct("Singer", 10), new EnemyStruct("Singer", 10)));
            graffitiData._battles.Add(new BattleData(6, new EnemyStruct("Wild Guy", 7)));
            graffitiData._battles[6].SetMusicTrack("event:/Music/WildGuy_Battle");
            graffitiData.AddBeforeCutscene(6, "Conversations/Graffiti/wildone3Before");
            graffitiData.AddAfterCutscene(6, "Conversations/Graffiti/wildone3After");
            graffitiData._battles.Add(new BattleData(7, new EnemyStruct("Singer", 10), new EnemyStruct("Musician", 7), new EnemyStruct("Musician", 7)));
            //Bonus Battles
            graffitiData._battles.Add(new BattleData(8, BonusLevelRequirement.MAGICIAN_DEFEATED, new EnemyStruct("Singer", 10), new EnemyStruct("Singer", 10), new EnemyStruct("Singer", 10)));

            HubAreaData tentData = new HubAreaData(false, 0, 6);
            _hubAreas.Add("Magician Tent", tentData);
            tentData._battles.Add(new BattleData(0, new EnemyStruct("Cop", 10)));
            tentData._battles[0].SetMusicTrack("event:/Music/Cop_Battle");
            tentData.AddBeforeCutscene(0, "Conversations/Tent/minibossesBefore");
            tentData._battles.Add(new BattleData(1, new EnemyStruct("Statue", 10)));
            tentData._battles.Add(new BattleData(2, new EnemyStruct("Wild Guy", 10)));
            tentData._battles[2].SetMusicTrack("event:/Music/WildGuy_Battle");
            tentData._battles.Add(new BattleData(3, new EnemyStruct("Magician", 5)));
            tentData.AddBeforeCutscene(3, "Conversations/Tent/magicianPhase1Before");
            tentData._battles.Add(new BattleData(4, new EnemyStruct("Wild Guy", 7), new EnemyStruct("Cop", 7), new EnemyStruct("Statue", 7)));
            tentData.AddBeforeCutscene(4, "Conversations/Tent/cursedMiniBossesBefore");
            tentData._battles.Add(new BattleData(5, new EnemyStruct("Finale", 10)));
            tentData.AddBeforeCutscene(5, "Conversations/Tent/magicianPhase2Before");
            tentData.AddAfterCutscene(5, "Conversations/Tent/magicianPhase2After");
            //Bonus Battles
            tentData._battles.Add(new BattleData(6, BonusLevelRequirement.MAGICIAN_DEFEATED, new EnemyStruct("Singer", 10), new EnemyStruct("Musician", 10), new EnemyStruct("Clown", 10)));
            tentData._battles.Add(new BattleData(7, BonusLevelRequirement.MAGICIAN_DEFEATED, new EnemyStruct("Wild Guy", 7), new EnemyStruct("Cop", 7), new EnemyStruct("Statue", 7)));
            tentData._battles.Add(new BattleData(8, BonusLevelRequirement.MAGICIAN_DEFEATED, new EnemyStruct("Wild Guy", 10), new EnemyStruct("Cop", 10), new EnemyStruct("Statue", 10)));


            subwayData.OnComplete += delegate { UnlockArea("Subway", parkData, Area.PARK); };
            pierData.OnComplete += delegate { UnlockArea("Pier", graffitiData, Area.GRAFFITI); };
            shopData.OnComplete += delegate { UnlockArea("Corner Store", capitolData, Area.CAPITOL); };

            parkData.OnComplete += delegate { UnlockTent("Park", tentData); };
            graffitiData.OnComplete += delegate { UnlockTent("Graffiti", tentData); };
            capitolData.OnComplete += delegate { UnlockTent("Magician Tent", tentData); };

            LoadDefeatedEnemy(this._defeatedEnemies);

            _justStarted = false;
        }

        public void ResetValues()
        {
            _defeatedEnemies.Clear();
            _attemptedBattles.Clear();
            _encounteredEnemies.Clear();
            _completedAreas.Clear();

            _demoIndex = 0;

            SetHubAreas();
        }

        private void AreaCompleted(string area)
        {
            if(_completedAreas.Contains(area))
            { return; }

            _completedAreas.Add(area);
        }

        public bool IsAreaCompleted(string area)
        {
            return _completedAreas.Contains(area);
        }

        public void UnlockTent(string completedArea, HubAreaData tent)
        {
            AreaCompleted(completedArea);
            if(_completedAreas.Count >= _hubAreas.Count - 1)
            {
                tent._justUnlocked = !_justStarted;
                tent._active = true;
            }
        }

        public void UnlockArea(string completedArea, HubAreaData area, Area areaIndex)
        {
            AreaCompleted(completedArea);
            area._justUnlocked = !_justStarted;
            area._active = true;
        }

        /// <summary>
        /// delete the savefile and quit when on phone
        /// </summary>
        public void DeleteCurrentSave()
        {
            SaveManager.WipeFile();

            BattleHandler.Instance.ResetValues();
            PartyHandler.Instance.ResetValues();
            ResetValues();

            Time.timeScale = 1f;

            SceneManager.LoadScene("TitleScene");

            Destroy(this);
        }

        public void LevelUp()
        {
            List<string> hubNames = new List<string>(_hubAreas.Keys);
            foreach(string hub in hubNames)
            {
                HubAreaData hubData = _hubAreas[hub];
                if(!hubData._active)
                {
                    continue;
                }
                hubData.LevelUp();
                _hubAreas[hub] = hubData;
            }

            OnLevelUp?.Invoke();
        }

        public void EnemyDefeated(string hubName, int battleIndex)
        {
            this._defeatedEnemies.Add(new KeyValuePair<string, int>(hubName, battleIndex));//writing to local saveLog;

            BattleData battle = _hubAreas[hubName]._battles[battleIndex];
            if (!battle._defeated)
            {
                _hubAreas[hubName].LevelUp();
                battle._defeated = true;
                //LevelUp();
            }

            string enemyName = battle._enemyList[0]._enemyName;
            int enemyLevel = battle._enemyList[0]._enemyLevel;
            switch(enemyName)
            {
                case "Contortionist":
                    if(enemyLevel == 1)
                    {
                        _hubAreas["Subway"]._active = true;
                        _hubAreas["Subway"]._justUnlocked = true;
                        PartyHandler.Instance.AddPartyMember(enemyName);
                        battle._playable = false;
                    }
                    else
                    {
                        if(!PartyHandler.Instance._upgradedCards.Contains("Nauseate"))
                        {
                            PartyHandler.Instance._upgradedCards.Add("Nauseate");
                        }
                    }
                    break;
                case "Mime":
                    if(enemyLevel == 1)
                    {
                        PartyHandler.Instance.AddPartyMember(enemyName);
                        _tutorialFinished = true;
                        battle._playable = false;
                    }
                    else
                    {
                        if (!PartyHandler.Instance._upgradedCards.Contains("IntenseFocus"))
                        {
                            PartyHandler.Instance._upgradedCards.Add("IntenseFocus");
                        }
                    }
                    break;
                case "Mascot":
                    if(enemyLevel == 1)
                    {
                        _hubAreas["Pier"]._active = true;
                        _hubAreas["Pier"]._justUnlocked = true;
                        PartyHandler.Instance.AddPartyMember(enemyName);
                        battle._playable = false;
                    }
                    else
                    {
                        if (!PartyHandler.Instance._upgradedCards.Contains("DoubleTeam"))
                        {
                            PartyHandler.Instance._upgradedCards.Add("DoubleTeam");
                        }
                    }
                    break;
                default:
                    break;
            }
            _hubAreas[hubName]._battles[battleIndex] = battle;
        }

        /// <summary>
        /// submit the local saveLog to the actual log file.
        /// </summary>
        public void SaveCurrentProgression() 
        {
            _saveFile.SetMembers(PartyHandler.Instance.GetMembers());
            _saveFile.SetLevel(PartyHandler.Instance.GetLevel());
            _saveFile.SetExp(PartyHandler.Instance.GetExp());

            _saveFile.SetAttemptedBattles(_attemptedBattles);
            _saveFile.SetEncounteredEnemies(_encounteredEnemies);
            _saveFile.SetDefeatedEnemies(_defeatedEnemies);
            _saveFile._addedMaxHealth = PartyHandler.Instance._addedMaxHealth;

            _saveFile.SetFinishedTutorial(_officialTutorialFinished);

            _saveFile.SetUpgradedCards(PartyHandler.Instance._upgradedCards);

            _saveFile.SetSpawnArea(BattleHandler.Instance._activeHubName);

            GameObject.FindGameObjectWithTag("Save").GetComponent<LoadSaveFiles>().Save(_saveFile);
        }

        /*private void UpdateActiveAreas(HubArea[] _hubAreas) {
            
            foreach (HubArea hubArea in _hubAreas) {
                hubArea.UpdateActiveStatus();
            }
        }*/

        /// <summary>
        /// set disable some fight according to the save file
        /// </summary>
        /// <param name="_defeatedEnemies"></param>
        private void LoadDefeatedEnemy(List<KeyValuePair<string, int>> defeatedEnemies) 
        {
            foreach (KeyValuePair<string, int> defeatedEnemy in defeatedEnemies) {
                BattleData battle = _hubAreas[defeatedEnemy.Key]._battles[defeatedEnemy.Value];
                if (!battle._defeated)
                {
                    _hubAreas[defeatedEnemy.Key].LevelUp();
                    battle._defeated = true;
                    //LevelUp();
                }

                if (battle._enemyList[0]._enemyLevel == 1)
                {
                    switch (battle._enemyList[0]._enemyName)
                    {
                        case "Contortionist":
                            _hubAreas["Subway"]._active = true;
                            battle._playable = false;
                            break;
                        case "Mime":
                            _tutorialFinished = true;
                            battle._playable = false;
                            break;
                        case "Mascot":
                            _hubAreas["Pier"]._active = true;
                            battle._playable = false;
                            break;
                        default:
                            break;
                    }
                }
                
            }
        }

        public void AddAttempt(string hubName, int battleIndex)
        {
            KeyValuePair<string, int> pair = new KeyValuePair<string, int>(hubName, battleIndex);
            if(_attemptedBattles.Contains(pair))
            {
                return;
            }

            _attemptedBattles.Add(pair);
        }

        private void OnDestroy()
        {
            Analytics.FlushEvents();
        }

        public void StartAdCooldown(float normalTimer, float successTimer)
        {
            _adCooldownTimer = normalTimer;
            _adSuccessCooldownTimer = successTimer;
        }

        private void Update()
        {
            if(_adCooldownTimer > 0f)
            {
                _adCooldownTimer -= Time.unscaledDeltaTime;
            }
            if(_adSuccessCooldownTimer > 0f)
            {
                _adSuccessCooldownTimer -= Time.unscaledDeltaTime;
            }
        }

        public void LoadDemoFullBattles(List<BattleConfigData> battles)
        {
            _demoFullBattles = battles;
        }

        public void LoadDemoTutorialBattles(List<BattleConfigData> battles)
        {
            _demoTutorialBattles = battles;
        }

        public void LoadDemoBattleBattles(List<BattleConfigData> battles)
        {
            _demoBattleBattles = battles;
        }

        public void SetDemoIndex(int index)
        {
            _demoIndex = index;
        }

        public void LoadNextDemoBattle()
        {
            List<BattleConfigData> battles;
            switch(_demoType)
            {
                case DemoManager.DemoType.FULL:
                    battles = _demoFullBattles;
                    break;
                case DemoManager.DemoType.BATTLE:
                    _demoIndex = 0;
                    battles = _demoBattleBattles;
                    break;
                case DemoManager.DemoType.TUTORIAL:
                default:
                    battles = _demoTutorialBattles;
                    break;
            }

            if(_demoIndex <= battles.Count - 1)
            {
                battles[_demoIndex].SetBattle();
                _demoIndex++;
                SceneManager.LoadScene("BattleSceneLandscape");
            }
            else
            {
                _demoIndex = battles.Count - 1;
                SaveCurrentProgression();
                SceneManager.LoadScene("EndSceneLandscape");
            }
        }

        public void ResetDemo()
        {
            BattleHandler.Instance.ResetValues();
            PartyHandler.Instance.ResetValues();
            ResetValues();

            Time.timeScale = 1f;

            SaveCurrentProgression();

            SceneManager.LoadScene("TitleSceneLandscape");
        }
    }
}
