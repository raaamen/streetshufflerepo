using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the individual turn for a character
    /// </summary>
    public abstract class CharacterTurn : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField][Tooltip("Name of this character")]
        public string _characterName;

        [SerializeField] private GameObject _vulnerableDiscard = null;
        [SerializeField] private TextMeshProUGUI _vulnerableDiscardText = null;

        [SerializeField] private GameObject _poisonDiscard = null;
        [SerializeField] private TextMeshProUGUI _poisonDiscardText = null;

        [SerializeField] private GameObject _damageDiscard = null;
        [SerializeField] private TextMeshProUGUI _damageDiscardText = null;

        [SerializeField]
        private bool _testBattle = false;

        //List of possible cards to draw this turn
        protected List<CardScriptable> _deck;
        protected List<CardScriptable> _discards;
        //Reference to the turn manager in the scene
        protected TurnManager _turnManager;
        protected BattleManager _battleManager;
        //This character's level
        public int _characterLevel = 1;

        protected HistoryList _cardHistory;

        protected int _vulnerablePerDiscard;
        public int _poisonPerCardUse
        { 
            get;
            protected set;
        }
        public int _damagePerCardUse
        {
            get;
            protected set;
        }

        [HideInInspector]
        public bool _doubleAttack = false;

        protected virtual void Awake()
        {
            _deck = new List<CardScriptable>();
            _discards = new List<CardScriptable>();
            _turnManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
            _battleManager = _turnManager.GetComponent<BattleManager>();
            _cardHistory = GameObject.FindGameObjectWithTag("Canvas").transform.Find("HistoryPanel").Find("HistoryViewer").
                Find("HistoryList").GetComponent<HistoryList>();

            GenerateDeck();
        }

        /// <summary>
        /// Called at the start of this character's turn
        /// </summary>
        public virtual void StartTurn(bool tutorial = false)
        {

        }

        /// <summary>
        /// Called at the end of this character's turn
        /// </summary>
        public virtual void EndTurn()
        {
            _vulnerablePerDiscard = 0;
            UpdateVulnerableDiscardVisual();
            UpdatePoisonDiscardVisual();
            UpdateDamageDiscardVisual();
            

            _poisonPerCardUse = 0;
            _damagePerCardUse = 0;
        }

        protected virtual void GenerateDeck()
        {
            GetAvailableCards();
            _discards = new List<CardScriptable>();
        }

        /// <summary>
        /// Populates the cardOptions list with all potential cards for this turn.
        /// </summary>
        protected virtual void GetAvailableCards()
        {
            Object[] cardList;
            if(_testBattle)
            {
                cardList = Resources.LoadAll("ScriptableObjects/TestAce");
            }
            else
            {
                cardList = Resources.LoadAll("ScriptableObjects/" + _characterName + "/Party");
            }
            
            _deck.Clear();
            foreach(Object card in cardList)
            {
                CardScriptable scriptable = (CardScriptable)card;
                if (PartyHandler.Instance._upgradedCards.Contains(scriptable._saveId))
                {
                    scriptable = (CardScriptable)Resources.Load("ScriptableObjects/" + _characterName + "/Upgraded/" + scriptable._id);
                }

                if(_characterLevel >= scriptable._requiredLevel)
                {
                    scriptable._accumulatedArmor = 0;
                    scriptable._accumulatedDamage = 0;

                    if(scriptable._adjustable)
                    {
                        string desc = scriptable._cardDescUneditted;
                        desc = desc.Replace("<TargetDamage>", "" + (scriptable._targetDamage + scriptable._accumulatedDamage));
                        desc = desc.Replace("<DamagePerUse>", "" + Mathf.Abs(scriptable._damagePerUse));
                        desc = desc.Replace("<ArmorAmount>", "" + (scriptable._armor + scriptable._accumulatedArmor));
                        desc = desc.Replace("<ArmorPerUse>", "" + Mathf.Abs(scriptable._armorPerUse));
                        scriptable._cardDesc = desc;
                    }
                    _deck.Add((CardScriptable)card);
                }
            }
        }

        /// <summary>
        /// Called when a card gets activated.
        /// </summary>
        /// <param name="card"></param> Card that is being activated
        protected virtual void ActivateCard(GameObject card)
        {

        }

        /// <summary>
        /// Returns true if there is only one card left in the character's hand.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLastCard()
        {
            return false;
        }

        /// <summary>
        /// Returns true if no card has been played yet this turn.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsFirstCard()
        {
            return false;
        }

        /// <summary>
        /// Returns the card class of the previously played card
        /// </summary>
        /// <returns></returns>
        public virtual CardScriptable.CardClass GetPreviousCard()
        {
            return CardScriptable.CardClass.NONE;
        }

        /// <summary>
        /// Discards the amount designated by the given parameter.
        /// </summary>
        /// <param name="discardAmount"></param> Number of cards to discard
        public virtual void Discard(int discardAmount)
        {
            
        }

        /// <summary>
        /// Called when this character dies
        /// </summary>
        public virtual void CharacterDied()
        {

        }

        public virtual void AddDiscard(CardScriptable scriptable)
        {
            _discards.Add(scriptable);
        }

        protected virtual void Shuffle()
        {
            _deck = new List<CardScriptable>(_discards);
            _discards.Clear();
        }

        public CardScriptable DrawRandomCard(bool removeCard = true)
        {
            int deckCount = _deck.Count;
            int discardCount = _discards.Count;
            int index = Random.Range(0, deckCount + discardCount);
            if(index < deckCount)
            {
                CardScriptable card = _deck[index];
                if(removeCard)
                {
                    _deck.RemoveAt(index);
                }
                return card;
            }
            else
            {
                index -= deckCount;
                CardScriptable card = _discards[index];
                if(removeCard)
                {
                    _discards.RemoveAt(index);
                }
                return card;
            }
        }

        protected void AddHistory(CardScriptable card)
        {
            _cardHistory.AddCard(card, _characterName);
        }

        public void AddVulnerablePerDiscard(int weaknessAmount)
        {
            _vulnerablePerDiscard += weaknessAmount;
            UpdateVulnerableDiscardVisual();
        }

        protected void UpdateVulnerableDiscardVisual()
        {
            if(_vulnerableDiscard == null)
            { return; }

            int vulnerableAmount = _vulnerablePerDiscard * GetNumCardsInHand() + GetVulnerablePerDiscard();
            
            if(vulnerableAmount == 0 || GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            {
                _vulnerableDiscard.SetActive(false);
            }
            else
            {
                
                _vulnerableDiscard.SetActive(true);
                _vulnerableDiscardText.text = vulnerableAmount.ToString();
            }
        }

        protected void UpdateDamageDiscardVisual()
        {
            if(_damageDiscard == null)
            { return; }

            int damagePerDiscard = GetDamagePerDiscard();

            if(damagePerDiscard == 0)
            {
                _damageDiscard.SetActive(false);
            }
            else
            {
                _damageDiscard.SetActive(true);
                _damageDiscardText.text = damagePerDiscard.ToString();
            }
        }

        protected void UpdatePoisonDiscardVisual()
        {
            if(_poisonDiscard == null)
            { return; }

            int poisonAmount = GetPoisonPerDiscard();

            if(poisonAmount == 0 || GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            {
                _poisonDiscard.SetActive(false);
            }
            else
            {
                _poisonDiscard.SetActive(true);
                _poisonDiscardText.text = poisonAmount.ToString();
            }
        }

        public void AddPoisonPerCardUsed(int poisonAmount)
        {
            _poisonPerCardUse += poisonAmount;
        }

        public void AddDamageTakenPerCardUsed(int damageAmount) 
        {
            _damagePerCardUse += damageAmount;
        }

        public virtual int GetNumCardsInHand()
        {
            return 0;
        }

        public virtual int GetVulnerablePerDiscard()
        {
            return 0;
        }

        public virtual int GetPoisonPerDiscard()
        {
            return 0;
        }

        public virtual int GetDamagePerDiscard()
        {
            return 0;
        }
    }
}