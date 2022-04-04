using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace StreetPerformers
{
    /// <summary>
    /// Handles anything to do with the turns in battle.
    /// This includes handling the drafting panel toggling on/off.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        //Enum to keep track of whose turn it is
        [HideInInspector]
        public enum Turn { PLAYER, ENEMY, PARTY, GAMEOVER }

        [Header("References")]
        [SerializeField][Tooltip("Button that ends the turn")]
        private GameObject _endTurnButton;
        [SerializeField][Tooltip("Text displaying the turn number")]
        private TextMeshProUGUI _turnText;

        [SerializeField] private GameObject _draftingPanel = null;
        [SerializeField] private RectTransform _draftingImage = null;
        [SerializeField] private RectTransform _draftingTextbox = null;
        [SerializeField] private GameObject _draftingCardPanel = null;
        [SerializeField] private RectTransform _draftingButton = null;
        [SerializeField] private TextMeshProUGUI _draftingButtonText = null;
        [SerializeField] private GameObject _draftingScrim = null;
        [SerializeField] private RectTransform _deckPanel = null;
        [SerializeField] private Image _nameBox = null;

        //Keeps track of whose turn it is
        private Turn _curTurn;
        //Index of which enemy's turn it is
        private int _enemyIndex = 0;
        //Index of which party member's turn it is
        private int _partyIndex = 0;

        //Current turn number (starts at 1)
        public int _turnNumber
        {
            get;
            private set;
        }

        //Reference to the drafting panel for choosing cards from party members
        private Vector3 _draftingButtonOriginal = new Vector3();
        private Vector3 _draftingButtonOffscreen = new Vector3();
        private Vector3 _draftingTextboxOriginal = new Vector3();
        private Vector3 _draftingTextboxOffscreen = new Vector3();
        private Vector3 _draftingImageOriginal = new Vector3();
        private Vector3 _draftingImageOffscreen = new Vector3();
        private Vector3 _deckPanelOriginal = new Vector3();
        private Vector3 _deckPanelOffscreen = new Vector3();

        //Reference to the BattleAgents script on this game object
        [SerializeField] private BattleAgents _agents = null;

        [SerializeField] private BattleTutorialManager _tutorial = null;
        private bool _isTutorialBattle = false;
        private string _tutorialEnemyName = "";

        /// <summary>
        /// Initializes values.
        /// </summary>
        private void Awake()
        {
            Vector3[] draftingCorners = new Vector3[4];
            _draftingTextbox.GetWorldCorners(draftingCorners);
            float toggleDistance = Mathf.Abs(draftingCorners[1].y);

            _draftingButtonOriginal = _draftingButton.position;
            _draftingButtonOffscreen = new Vector3(_draftingButtonOriginal.x, _draftingButtonOriginal.y - toggleDistance);

            _draftingTextboxOriginal = _draftingTextbox.position;
            _draftingTextboxOffscreen = new Vector3(_draftingTextboxOriginal.x, _draftingTextboxOriginal.y - toggleDistance);

            _draftingImageOriginal = _draftingImage.position;
            _draftingImageOffscreen = new Vector3(_draftingImageOriginal.x, _draftingImageOriginal.y - toggleDistance);

            _deckPanelOriginal = _deckPanel.position;
            _deckPanelOffscreen = new Vector3(_deckPanelOriginal.x, _deckPanelOriginal.y - toggleDistance);

            _turnNumber = 1;
            _turnText.text = "Turn " + _turnNumber;
        }

        /// <summary>
        /// Initializes the curTurn variable and disables the end turn button
        /// </summary>
        private void Start()
        {
            _curTurn = Turn.PARTY;

            _endTurnButton.SetActive(false);

            BattleHandler battle = BattleHandler.Instance;
            if (battle._activeBattleIndex == 0)
            {
                string activeHubName = battle._activeHubName;
                if(activeHubName != null && (activeHubName.Equals("Corner Store") || activeHubName.Equals("Pier") || activeHubName.Equals("Subway")))
                {
                    _isTutorialBattle = true;
                    switch(activeHubName)
                    {
                        case "Corner Store":
                            _tutorialEnemyName = "Mascot";
                            break;
                        case "Pier":
                            _tutorialEnemyName = "Contortionist";
                            break;
                        case "Subway":
                            _tutorialEnemyName = "Mime";
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Starts the next turn in the sequence.
        /// The game starts with the party drafting phase (if there are party members).
        /// After cycling through each party member, the player turn starts.
        /// After the player turn, it cycles through each enemy turn next before starting up with the party members again.
        /// </summary>
        public void NextTurn()
        {
            switch(_curTurn)
            {
                case Turn.PARTY:
                    //If there are no party members, we reset turn values and go straight to the player turn
                    if(_agents._partyMembers.Count == 0)
                    {
                        ResetTurnValues();
                        EndTurn();
                        break;
                    }

                    //If this is the first party member, we open the drafting panel and start their turn.
                    if(_partyIndex == 0)
                    {
                        _draftingPanel.SetActive(true);
                        _deckPanel.gameObject.SetActive(true);
                        ToggleDraftingUI(true);

                        _draftingButton.gameObject.SetActive(!_agents._partyMembers[0].GetComponent<PartyTurn>()._randomChooseCard);

                        _draftingButtonText.text = "View Battle";

                        _draftingButton.position = _draftingButtonOriginal;
                        _draftingTextbox.position = _draftingTextboxOriginal;
                        _draftingImage.position = _draftingImageOriginal;
                        _deckPanel.position = _deckPanelOriginal;
                        ResetTurnValues();
                    }

                    PartyTurn party = _agents._partyMembers[_partyIndex].GetComponent<PartyTurn>();
                    _nameBox.sprite = party._nameSprite;

                    bool tutorial = _isTutorialBattle && _tutorialEnemyName.Equals("Contortionist");
                    party.StartTurn(tutorial);

                    break;
                //Start of player's turn we disable the drafting panel and enable the end turn button
                case Turn.PLAYER:
                    _draftingPanel.SetActive(false);
                    _endTurnButton.SetActive(true);
                    bool tutorialCards = _isTutorialBattle && _turnNumber == 1 && _tutorialEnemyName.Equals("Mascot");
                    _agents._player.GetComponent<PlayerTurn>().StartTurn(tutorialCards);

                    if(_isTutorialBattle)
                    {
                        switch(_turnNumber)
                        {
                            case 1:
                                if (_tutorialEnemyName.Equals("Mascot"))
                                {
                                    _endTurnButton.SetActive(false);
                                    _tutorial.Initialize(_tutorialEnemyName);
                                }
                                else if(_tutorialEnemyName.Equals("Contortionist"))
                                {
                                    _tutorial.Initialize(_tutorialEnemyName);
                                }
                                break;
                            case 2:
                                if(_tutorialEnemyName.Equals("Mime"))
                                {
                                    _tutorial.Initialize(_tutorialEnemyName);
                                }
                                break;
                        }
                    }
                    break;
                case Turn.ENEMY:
                    //If we have gone through all the enemies, end the enemy turn
                    if(_enemyIndex >= _agents._enemyList.Count)
                    {
                        EndTurn();
                        break;
                    }
                    
                    //If the enemy at the given index is dead, end the current turn
                    if(_agents._enemyList[_enemyIndex] == null)
                    {
                        EndTurn();
                        break;
                    }
                    _agents._enemyList[_enemyIndex].GetComponent<EnemyTurn>().StartTurn();
                    break;
                case Turn.GAMEOVER:
                    break;
                default:
                    break;
            }
        }
       
        /// <summary>
        /// Called at the end of a character's turn
        /// </summary>
        public void EndTurn()
        {
            CancelInvoke("EndTurn");
            switch(_curTurn)
            {
                case Turn.PARTY:
                    _partyIndex++;
                    //If we've gone through each party member, start the player turn
                    if(_partyIndex >= _agents._partyMembers.Count)
                    {
                        _partyIndex = 0;
                        _curTurn = Turn.PLAYER;
                    }
                    NextTurn();
                    break;
                case Turn.PLAYER:
                    _curTurn = Turn.ENEMY;
                    _enemyIndex = 0;
                    _endTurnButton.SetActive(false);
                    Invoke("NextTurn", 1f);
                    break;
                case Turn.ENEMY:
                    _enemyIndex++;
                    //If we've gone through each enemy, increment the turn number and start the party turn
                    if(_enemyIndex >= _agents._enemyList.Count)
                    {
                        _enemyIndex = 0;
                        _curTurn = Turn.PARTY;
                        _turnNumber++;
                        _turnText.text = "Turn " + _turnNumber;
                        Invoke("NextTurn", 1f);
                        break;
                    }
                    NextTurn();
                    break;
                case Turn.GAMEOVER:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Toggles the party member drafting panel on/off.
        /// </summary>
        public void ToggleDraftPanel()
        {
            //Toggle off
            //_draftingPanel.transform.DOKill();
            if(_draftingCardPanel.activeInHierarchy)
            {
                ToggleDraftingUI(false);

               _draftingButtonText.text = "View Cards";

                /*_draftingButton.DOMove(_draftingButtonOffscreen, .3f);
                _draftingTextbox.DOMove(_draftingTextboxOffscreen, .3f);
                _draftingImage.DOMove(_draftingImageOffscreen, .3f);
                _deckPanel.DOMove(_deckPanelOffscreen, .3f);*/
            }
            //Toggle on
            else
            {
                ToggleDraftingUI(true);

                _draftingButtonText.text = "View Battle";

                /*_draftingButton.DOMove(_draftingButtonOriginal, .3f);
                _draftingTextbox.DOMove(_draftingTextboxOriginal, .3f);
                _draftingImage.DOMove(_draftingImageOriginal, .3f);
                _deckPanel.DOMove(_deckPanelOriginal, .3f);*/
            }
        }

        private void ToggleDraftingUI(bool on)
        {
            _draftingScrim.SetActive(on);
            _draftingCardPanel.SetActive(on);
            _draftingImage.gameObject.SetActive(on);
            _draftingTextbox.gameObject.SetActive(on);
        }

        /// <summary>
        /// Called at the start of the player's turn. Resets player stats like mana, etc.
        /// </summary>
        private void ResetTurnValues()
        {
            _agents._player.GetComponent<PlayerTurn>().ResetTurnValues();
        }

        /// <summary>
        /// Called when the player dies. Ends the game so that no more turns play out.
        /// </summary>
        public void PlayerDied()
        {
            EndGame();
        }

        /// <summary>
        /// Called when an enemy dies. Checks if the enemy that dies was in the middle of it's turn, if so end their turn.
        /// </summary>
        /// <param name="enemy"></param>
        public void EnemyDied(GameObject enemy)
        {
            if(_curTurn == Turn.ENEMY && _agents._enemyList[_enemyIndex] == enemy)
            {
                Invoke("EndTurn", 1f);
            }
        }

        /// <summary>
        /// Called at the end of the game. Makes sure no more turns are taken.
        /// </summary>
        public void EndGame()
        {
            _curTurn = Turn.GAMEOVER;
            _endTurnButton.SetActive(false);
        }
    }
}
