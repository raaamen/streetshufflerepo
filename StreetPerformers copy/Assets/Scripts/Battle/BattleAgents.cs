using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the agents of battle: the player, the party members, and the enemies.
    /// This includes their position in battle, health status, and other miscellaneous functions
    /// </summary>
    public class BattleAgents : MonoBehaviour
    {
        [Header("References")]
        [SerializeField][Tooltip("Enemy position script that handles the enemy positions in battle")]
        private EnemyPosition _enemyPosition;
        [SerializeField]
        private List<Transform> _partySpawnPos;
        [SerializeField]
        private List<Transform> _partyAttackPos;
        [SerializeField] private Transform _aceOffscreenPosition = null;
        [SerializeField] private Transform _enemyOffscreenPosition = null;

        //Player object in game
        public GameObject _player
        {
            get;
            private set;
        }
        //Player's alive status
        public bool _playerAlive
        {
            get;
            private set;
        }

        //List of enemies for this battle
        public List<GameObject> _enemyList
        {
            get;
            private set;
        }
        //Number of enemies left alive
        public int _enemiesLeft
        {
            get;
            private set;
        }
        //Maximum health of all enemies combined at the beginning of battle (used for crowd placement)
        public float _enemyMaxHealth
        {
            get;
            private set;
        }

        //List of party members for this battle
        public List<GameObject> _partyMembers
        {
            get;
            private set;
        }

        //Reference to the TurnManager script on this object
        private TurnManager _turnManager;
        //Reference to the BattleManager script on this object
        private BattleManager _battleManager;

        /// <summary>
        /// Instantiate enemies and party members as well as set some of the member variables.
        /// </summary>
        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerAlive = true;

            _partyMembers = new List<GameObject>();
            _enemyList = new List<GameObject>();

            _turnManager = GetComponent<TurnManager>();
            _battleManager = GetComponent<BattleManager>();

            bool preBattleCutscene = BattleHandler.Instance._preBattleAnimation;
            if(preBattleCutscene)
            {
                _player.transform.position = _aceOffscreenPosition.position;
            }

            int enemyInd = 0;
            foreach(EnemyStruct enemyStr in BattleHandler.Instance._enemies)
            {
                GameObject enemy = Resources.Load("Enemies/" + enemyStr._enemyName) as GameObject;
                enemy.name = enemyStr._enemyName;
                enemy.GetComponent<EnemyTurn>().SetLevel(enemyStr._enemyLevel);

                Vector3 spawnPos;
                if(preBattleCutscene)
                {
                    spawnPos = _enemyOffscreenPosition.position;
                }
                else
                {
                    spawnPos = _enemyPosition.GetSpawnPos(enemyInd);
                }
                _enemyList.Add(Instantiate(enemy, spawnPos, Quaternion.identity));
                
#if UNITY_STANDALONE
                _enemyList[enemyInd].transform.SetParent(_enemyPosition._spawnPos.parent);
#elif UNITY_IOS || UNITY_ANDROID
                _enemyList[enemyInd].transform.SetParent(_enemyPosition._spawnPos);
#endif

                enemyInd++;
            }

            int partyInd = 0;
            foreach(string partyName in PartyHandler.Instance._partyMembers)
            {
                if(PartyHandler.Instance._excludedPartyMembers.Contains(partyName))
                {
                    continue;
                }

                GameObject partyMember = Resources.Load("PartyMembers/" + partyName) as GameObject;
                partyMember.name = partyName;

                Vector3 spawnPos = new Vector3(-10f, 0f, 0f);
#if UNITY_STANDALONE
                    spawnPos = _partySpawnPos[partyInd].position;
#elif UNITY_IPHONE || UNITY_ANDROID
                    spawnPos = new Vector3(-10f, 0f, 0f);
#endif
                _partyMembers.Add(Instantiate(partyMember, spawnPos, Quaternion.identity));
                partyInd++;
            }

            _battleManager.ToggleDialogueDisabledObjects(false);
        }

        /// <summary>
        /// Set the party member position, enemy max health, and character OnDeath action.
        /// </summary>
        private void Start()
        {
            int index = 1;
            foreach(GameObject g in _partyMembers)
            {
                PartyTurn party = g.GetComponent<PartyTurn>();

#if UNITY_STANDALONE
                party._onScreenPos = _partyAttackPos[index - 1].position;
#elif UNITY_IPHONE || UNITY_ANDROID
                g.transform.position = (new Vector3(-10, -1 * index, 0));
                party._onScreenPos = new Vector3(_player.GetComponent<PlayerTurn>().GetBattlePosition().x + 1f, g.transform.position.y, 0f);
#endif

                party._offScreenPos = g.transform.position;
                index++;
            }

            _enemiesLeft = _enemyList.Count;
            foreach(GameObject enemy in _enemyList)
            {
                CharacterStats enemyStat = enemy.GetComponent<CharacterStats>();
                _enemyMaxHealth += enemyStat._health;
            }

            for(int i = 0; i < _enemyList.Count; i++)
            {
                _enemyList[i].GetComponent<CharacterStats>().OnDeath += EnemyDied;

                _enemyList[i].GetComponent<EnemyTurn>().Initialize(_battleManager._blockPanel);
            }

            _player.GetComponent<CharacterStats>().OnDeath += PlayerDied;
        }

        /// <summary>
        /// Move the enemies into battle position from off screen at the start of battle.
        /// </summary>
        public void BattleStartAnimation()
        {
            _enemyPosition.SetSpawn(_enemyList);
            float time = _enemyPosition.MoveEnemies(_enemyList);
            _battleManager.Invoke("StartCutscene", time + 1f);
        }

        public void MoveEnemiesToStart(float time)
        {
            _enemyPosition.MoveSpawn(_enemyList, time);
        }

        /// <summary>
        /// Called when the player dies. Notify the TurnManager and BattleManager that the player died.
        /// </summary>
        /// <param name="player"></param>
        public void PlayerDied(GameObject player)
        {
            _playerAlive = false;

            _turnManager.PlayerDied();
            _battleManager.PlayerDied();
        }

        /// <summary>
        /// Called when an enemy dies. Remove the enemy from the enemy list, move it off screen, and notify the TurnManager 
        /// and BattleManager that the enemy died.
        /// </summary>
        /// <param name="enemy"></param> Reference to the enemy that died
        public void EnemyDied(GameObject enemy)
        {
            if(!_enemyList.Contains(enemy))
            { return; }

            _turnManager.EnemyDied(enemy);

            _enemyList[_enemyList.IndexOf(enemy)] = null;
            _enemiesLeft--;

            _enemyPosition.MoveEnemies(_enemyList);
            _enemyPosition.MoveOffScreen(enemy);

            Destroy(enemy.GetComponent<CombatEffectManager>());
            Destroy(enemy.GetComponent<ArmorManager>());
            Destroy(enemy.GetComponent<StatusEffectManager>());
            Destroy(enemy.GetComponent<CharacterStats>());
            Destroy(enemy.GetComponent<EnemyTurn>());
            Destroy(enemy.transform.Find("HealthCanvas").gameObject);

            _battleManager.EnemyDied();
        }

        /// <summary>
        /// Exhausts a random party member for the next turn. Cannot exhaust an already exhausted party member.
        /// </summary>
        public void ExhaustRandomParty()
        {
            List<PartyTurn> nonexhaustedParty = new List<PartyTurn>();
            foreach(GameObject party in _partyMembers)
            {
                PartyTurn partyTurn = party.GetComponent<PartyTurn>();
                if(!partyTurn.IsExhausted())
                {
                    nonexhaustedParty.Add(partyTurn);
                }
            }

            //Make sure there is a valid party member
            if(nonexhaustedParty.Count <= 0f)
            { return; }

            int index = Random.Range(0, nonexhaustedParty.Count);
            nonexhaustedParty[index].Exhaust();
        }

        /// <summary>
        /// Tells the party members to assign a random card to the player next turn.
        /// </summary>
        public void AssignRandomCardToAll()
        {
            foreach(GameObject partyMember in _partyMembers)
            {
                partyMember.GetComponent<PartyTurn>().RandomChooseCard();
            }
        }

        public void ToggleHealthCanvas(bool toggle)
        {
            _player.GetComponent<CharacterStats>().ToggleHealthCanvas(toggle);
            foreach(GameObject enemy in _enemyList)
            {
                if(enemy == null)
                { continue; }

                enemy.GetComponent<CharacterStats>().ToggleHealthCanvas(toggle);
            }
        }
    }
}
