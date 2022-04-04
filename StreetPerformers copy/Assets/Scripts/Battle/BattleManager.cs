using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace StreetPerformers
{
    /// <summary>
    /// Handles everything in battle outside of the actual turn system.
    /// This includes handling win/loss states, setting the background, and updating the crowd.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField][Tooltip("Win screen panel")]
        private GameObject _winPanel;
        [SerializeField][Tooltip("Lose screen panel")]
        private GameObject _losePanel;
        [SerializeField][Tooltip("Dialogue/conversation panel")]
        private DialogueManager _dialogue;
        public BlockAnimation _blockPanel = null;

        [SerializeField][Tooltip("Background of the area - behind the crowd")]
        private SpriteRenderer _background;
        [SerializeField][Tooltip("Foreground of the area - in front of the crowd but behind player")]
        private SpriteRenderer _frontground;

        [SerializeField][Tooltip("Crowd script that controls crowd movement")]
        private Crowd _crowd;

        [SerializeField] private FMODUnity.StudioEventEmitter _fmodEmitter = null;

        //Set to false after the battle begins. This is used to make sure correct dialogue plays and keeps the order of the game
        //before and after dialogue in tact
        private bool beginningFight;

        //Reference to the BattleAgents script of this game object
        private BattleAgents _agents;
        //Reference to the TurnManager script of this game object
        private TurnManager _turnManager;

        /// <summary>
        /// Initialize value and set crowd to start in the middle of the screen.
        /// </summary>
        private void Awake()
        {
            _crowd.UpdateCrowd(.5f);

            _agents = GetComponent<BattleAgents>();
            _turnManager = GetComponent<TurnManager>();

            if(!BattleHandler.Instance._battleMusic.Equals(""))
            {
                _fmodEmitter.Event = BattleHandler.Instance._battleMusic;
            }

            if (BattleHandler.Instance._background != null)
            {
                _background.sprite = BattleHandler.Instance._background;
                _frontground.sprite = BattleHandler.Instance._frontground;
                if (BattleHandler.Instance._preBattleAnimation)
                {
                    _background.color = BattleHandler.Instance._backgroundColor;
                }
            }
        }

        /// <summary>
        /// Set the background and UpdateCrowd action triggers.
        /// </summary>
        private void Start()
        {
            beginningFight = true;

            _agents._player.GetComponent<PlayerStats>().OnHealthChange += UpdateCrowd;
            foreach(GameObject enemy in _agents._enemyList)
            {
                enemy.GetComponent<CharacterStats>().OnHealthChange += UpdateCrowd;
            }
        }

        public void ToggleDialogueDisabledObjects(bool toggle)
        {
            if(BattleHandler.Instance._beforeConvo != null)
            {
                _dialogue.ToggleDisabledObjects(toggle);
            }
        }

        /// <summary>
        /// Starts the cutscene at the beginning of the match
        /// </summary>
        private void StartCutscene()
        {
            _dialogue.gameObject.SetActive(true);
            _dialogue.StartConversation(beginningFight);
        }

        /// <summary>
        /// Called at the end of either the before or after dialogue.
        /// </summary>
        public void DialogueFinished()
        {
            //If it's the beginning dialogue, start the battle
            if(beginningFight)
            {
                StartBattle();
            }
            //Otherwise we handle the after dialogue differently depending on who we fought
            else
            {
                string enemyName = BattleHandler.Instance._enemies[0]._enemyName;
                switch(enemyName)
                {
                    //If a new party member, show party member screen
                    case "Contortionist":
                    case "Mascot":
                    case "Mime":
                        _winPanel.GetComponentInChildren<RewardUI>().AddCharacterScreen(enemyName);
                        break;
                    //If the final boss, go to the end scene
                    case "Finale":
#if UNITY_STANDALONE
                        SceneManager.LoadScene("EndSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
                        SceneManager.LoadScene("EndScene");
#endif
                        break;
                    //If it's anyone else, load up the hub world
                    default:
                        if(ProgressionHandler.Instance._isDemo)
                        {
                            ProgressionHandler.Instance.LoadNextDemoBattle();
                        }
                        else
                        {
#if UNITY_STANDALONE
                            SceneManager.LoadScene("HubWorldSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
                        SceneManager.LoadScene("HubWorldScene");
#endif
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Called either after the beginning dialogue or after the enemy move animations if no beginning dialogue exists.
        /// Tells the TurnManager to start the battle.
        /// </summary>
        public void StartBattle()
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("InDialogue", 1);
            _turnManager.NextTurn();
        }

        /// <summary>
        /// This method is invoked shortly after the last enemy died.
        /// We buffer this in case the player dies just after the enemy for some reason (like using Thrash) as we
        /// don't want the player to win while dead.
        /// </summary>
        private void BufferVictory()
        {
            if(!_agents._playerAlive)
            { return; }

            beginningFight = false;
            _winPanel.GetComponent<DripMenu>().OpenDripMenu();

            EndBattleAnaltyics("Battle Win", false);
        }

        /// <summary>
        /// Called on either win, loss, or quit.
        /// Sends an analytics message about the state of the battle including whether it was a win/loss/quit, what turn it ended on,
        /// which battle this was, and what the most used cards were.
        /// </summary>
        /// <param name="eventName"></param> Either Win/Loss/Quit
        /// <param name="quit"></param> Set to true if the user quit the battle so we don't send card data
        public void EndBattleAnaltyics(string eventName, bool quit)
        {
            if(Application.isEditor)
            { return; }

            //Check to see if this battle has already been defeated, if so don't send data
            string hub = BattleHandler.Instance._activeHubName;
            int index = BattleHandler.Instance._activeBattleIndex;

            if(ProgressionHandler.Instance._hubAreas[hub]._battles[index]._defeated)
            { return; }

            //Send info about the state of the battle
            Debug.Log(AnalyticsEvent.Custom(eventName, new Dictionary<string, object>
            {
                { "Level", BattleHandler.Instance._activeHubName + " " + BattleHandler.Instance._activeBattleIndex},
                { "Turn Number", _turnManager._turnNumber}
            }));

            if(quit)
            { return; }

            Dictionary<string, object> cardDict = BattleHandler.Instance.GetTopCards();

            if(cardDict == null)
            { return; }

            //Send info about the most used cards this battle
            Debug.Log(AnalyticsEvent.Custom("Cards Used", cardDict));
        }

        /// <summary>
        /// Called when the health state of any player or enemy changes in the battle. Tells the crowd to update its position
        /// based on the current health state ratios of characters in battle.
        /// 
        /// The ratio is based off of current health / max health. So if the player is at 20/20 while the enemy is at 80/100,
        /// the crowd will lean towards the player as they've dealt more percent damage overall since the start of the battle.
        /// </summary>
        public void UpdateCrowd()
        {
            float enemyCurHealth = 0f;
            foreach(GameObject enemy in _agents._enemyList)
            {
                if(enemy == null)
                { continue; }

                enemyCurHealth += enemy.GetComponent<CharacterStats>()._health;
            }
            float enemyPercentage = enemyCurHealth / _agents._enemyMaxHealth;

            PlayerStats pStats = _agents._player.GetComponent<PlayerStats>();
            float playerCurHealth = pStats._health;
            float playerPercentage = playerCurHealth / pStats._maxHealth;

            if(playerPercentage + enemyPercentage <= 0f)
            {
                _crowd.UpdateCrowd(1f);
                return;
            }

            float percent = playerPercentage / (playerPercentage + enemyPercentage);
            _crowd.UpdateCrowd(1f - percent);
        }

        /// <summary>
        /// Called upon player death. Plays the player lose animation and the player lose music as well
        /// as opens the loss screen.
        /// </summary>
        public void PlayerDied()
        {
            _agents._player.GetComponentInChildren<Animator>().SetTrigger("loseBattle");
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Player_Death);

            _losePanel.GetComponent<DripMenu>().OpenDripMenu();
            EndBattleAnaltyics("Battle Lose", false);
        }

        /// <summary>
        /// Called upon enemy death. Checks if all enemies are dead and if so, begins the victory process.
        /// </summary>
        public void EnemyDied()
        {
            if(_agents._enemiesLeft <= 0)
            {
                _turnManager.EndGame();

                _agents._player.GetComponentInChildren<Animator>().SetTrigger("wonBattle");

                CancelInvoke("BufferVictory");
                Invoke("BufferVictory", 2f);
            }
        }

        /// <summary>
        /// Used for killing characters quickly in the editor.
        /// D kills Ace.
        /// The 1,2,3 keys will kill the first/second/third enemy respectively.
        /// </summary>
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Cursor.visible = !Cursor.visible;
            }
            if(!Application.isEditor)
            { return; }

            if(Input.GetKeyDown(KeyCode.D))
            {
                _agents._player.GetComponent<CharacterStats>().TakeDamage(100000f, null, true, true, true);
            }

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                if(_agents._enemyList[0] != null)
                {
                    _agents._enemyList[0].GetComponent<CharacterStats>().TakeDamage(10000f, null, true, true, true);
                }
            }

            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                if(_agents._enemyList[1] != null)
                {
                    _agents._enemyList[1].GetComponent<CharacterStats>().TakeDamage(10000f, null, true, true, true);
                }
            }

            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                if(_agents._enemyList[2] != null)
                {
                    _agents._enemyList[2].GetComponent<CharacterStats>().TakeDamage(10000f, null, true, true, true);
                }
            }
        }
    }
}
