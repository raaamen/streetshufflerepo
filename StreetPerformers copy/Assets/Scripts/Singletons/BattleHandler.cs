using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace StreetPerformers
{
    /// <summary>
    /// Holds data for which set of enemies to fight next.
    /// </summary>
    public class BattleHandler : Singleton<BattleHandler>
    {
        //List of enemies to fight in the next battle
        [HideInInspector]
        public List<EnemyStruct> _enemies;
        [HideInInspector]
        public GameObject _beforeConvo;
        [HideInInspector]
        public GameObject _afterConvo;
        [HideInInspector]
        public Sprite _background;
        [HideInInspector]
        public Sprite _frontground;
        [HideInInspector]
        public Color _backgroundColor;
        public Color _crowdColor;
        private DialogueManager _dialogueManager;

        [HideInInspector]
        public string _activeHubName;

        [HideInInspector]
        public int _activeBattleIndex;

        public int _experienceThisBattle
        {
            get;
            private set;
        }
        public float _experienceMultiplier = 1f;

        public Dictionary<string, int> _cardsUsedThisBattle;
        public Dictionary<string, int> _cardsDiscardedThisBattle;

        private bool _testScene = false;

        public bool _preBattleAnimation
        {
            get;
            private set;
        }

        public string _battleMusic
        {
            get;
            private set;
        }

      
        private void Awake()
        {
            _enemies = new List<EnemyStruct>();
            _enemies.Add(new EnemyStruct("TestEnemy", 1));
            _enemies.Add(new EnemyStruct("TestEnemy", 1));
			_enemies.Add(new EnemyStruct("TestEnemyQ", 1));
            _experienceThisBattle = 0;

            _cardsUsedThisBattle = new Dictionary<string, int>();
            _cardsDiscardedThisBattle = new Dictionary<string, int>();
        }

        public void ResetValues()
        {
            _activeHubName = "";
        }

        public void SetBattle(string hubName, int battleIndex, Sprite background, Sprite frontground, Color colorOfCrowd, bool preBattleAnimation = false)
        {
            _activeHubName = hubName;
            _activeBattleIndex = battleIndex;

            BattleData battle = ProgressionHandler.Instance._hubAreas[hubName]._battles[battleIndex];
            battle._attempted = true;
            ProgressionHandler.Instance.AddAttempt(hubName, battleIndex);

            _battleMusic = battle._musicTrack;

            PartyHandler.Instance.ResetExcludedParty();
            _enemies = new List<EnemyStruct>(battle._enemyList);
            foreach(EnemyStruct enemy in _enemies)
            {
                switch(enemy._enemyName)
                {
                    case "Mascot":
                    case "Contortionist":
                    case "Mime":
                        PartyHandler.Instance.RemoveParty(enemy._enemyName);
                        break;
                }
            }

            _beforeConvo = Resources.Load(battle._beforeCutscene) as GameObject;
            _afterConvo = Resources.Load(battle._afterCutscene) as GameObject;
            _crowdColor = colorOfCrowd;
            _background = background;
            _frontground = frontground;
            _experienceThisBattle = 0;
            _cardsUsedThisBattle.Clear();
            _cardsDiscardedThisBattle.Clear();

            _preBattleAnimation = preBattleAnimation;
            if (_preBattleAnimation)
            {
                _backgroundColor = new Color(1f, 128f / 255f, 144f / 255f, 1f);
            }
        }

        public void SetBattle(List<EnemyStruct> enemies)
        {
            _enemies = enemies;

            _testScene = true;
        }
   
        public void ResetBattle()
        {
            _experienceThisBattle = 0;

            HubAreaData hubArea = ProgressionHandler.Instance._hubAreas[_activeHubName];
            BattleData battle;
            if(_activeBattleIndex > hubArea._battles.Count - 1)
            {
                battle = hubArea._battles[_activeBattleIndex - hubArea._battles.Count];
            }
            else
            {
                battle = hubArea._battles[_activeBattleIndex];
            }

            _enemies = new List<EnemyStruct>(battle._enemyList);
            _beforeConvo = Resources.Load(battle._beforeCutscene) as GameObject;
            _afterConvo = Resources.Load(battle._afterCutscene) as GameObject;

            _cardsUsedThisBattle.Clear();
            _cardsDiscardedThisBattle.Clear();
            
        }

        public void DefeatedEnemy()
        {
            if(_testScene)
            { return; }

            ProgressionHandler.Instance.EnemyDefeated(_activeHubName, _activeBattleIndex);
        }

        public void AddExperience(int experience)
        {
            if(_testScene)
            { return; }

            BattleData battle = ProgressionHandler.Instance._hubAreas[_activeHubName]._battles[_activeBattleIndex];
            if(battle._defeated)
            {
                experience /= 2;
            }
            _experienceThisBattle += experience;
        }
        
        public void ApplyExperienceMult()
        {
            if(_testScene)
            { return; }

            float exp = _experienceThisBattle;
            exp *= _experienceMultiplier;
            _experienceThisBattle = Mathf.RoundToInt(exp);
            _experienceMultiplier = 1f;
        }

        public void ResetExperience()
        {
            _experienceThisBattle = 0;
            _experienceMultiplier = 1f;
        }

        public void UseCard(CardScriptable scriptable)
        {
            if(_testScene)
            { return; }

            if(!_cardsUsedThisBattle.ContainsKey(scriptable._id))
            {
                _cardsUsedThisBattle.Add(scriptable._id, 1);
                _cardsDiscardedThisBattle.Add(scriptable._id, 0);
                return;
            }
            _cardsUsedThisBattle[scriptable._id]++;
        }

        public void DiscardCard(CardScriptable scriptable)
        {
            if(_testScene)
            { return; }

            if(!_cardsDiscardedThisBattle.ContainsKey(scriptable._id))
            {
                _cardsUsedThisBattle.Add(scriptable._id, 0);
                _cardsDiscardedThisBattle.Add(scriptable._id, 1);
                return;
            }
            _cardsDiscardedThisBattle[scriptable._id]++;
        }

        public Dictionary<string, object> GetTopCards()
        {
            if(_testScene)
            { return null; }

            if(_cardsUsedThisBattle.Count <= 0)
            { return null; }

            Dictionary<string, object> cardDict = new Dictionary<string, object>();
            List<KeyValuePair<string, int>> cardList = _cardsUsedThisBattle.ToList();

            cardList.Sort(
                delegate (KeyValuePair<string, int> pair1, KeyValuePair<string, int> pair2)
                {
                    return pair2.Value.CompareTo(pair1.Value);
                });
            for(int i = 0; i < cardList.Count && i < 10; i++)
            {
                cardDict.Add(cardList[i].Key, cardList[i].Value);
            }

            return cardDict;
        }
    }
}
