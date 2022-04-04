using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace StreetPerformers
{
    public class PlayerTurn : CharacterTurn
    {
        [SerializeField]
        private Transform _handTrans;
        [SerializeField]
        private Transform _deckCardTrans;
        [SerializeField]
        private Transform _battlePosition;
        [SerializeField]
        private Transform _discardTrans;
        [SerializeField]
        private Transform _magicianTrans;

        private List<CardScriptable> _scriptablesInHand;
        private List<GameObject> _cardsInHand;

        private static List<CardScriptable> _scriptablesDiscarded;

        private int _maxCardsInHand = 2;
        private int _numCardsLeft;
        private bool _firstCard = true;
        private CardScriptable.CardClass _previousCard = CardScriptable.CardClass.NONE;
        private GameObject _chosenCard;

        private PlayerStats _stats;
        private StatusEffectManager _statusManager;
        private CombatEffectManager _combatEffManager;
        private PlayerHand _hand;

        private List<int> _disabledIndex;
        private List<GameObject> _disabledCards;

        private int _additionalAceCardsAssigned = 0;
        private int _aceCardsAssigned = 2;

        private bool _ended = false;

        [HideInInspector]
        public System.Action<int> OnDiscard;

        private List<GameObject> _partyMembers;

        [HideInInspector] public bool _isTurn = false;

        protected override void Awake()
        {
            base.Awake();

            _stats = GetComponent<PlayerStats>();
            _hand = GetComponent<PlayerHand>();
            _disabledIndex = new List<int>();
            _disabledCards = new List<GameObject>();
            _scriptablesDiscarded = new List<CardScriptable>();

#if UNITY_EDITOR
            if(SceneManager.GetActiveScene().name.Equals("TestBattleScene"))
            {
                _maxCardsInHand = Resources.LoadAll("ScriptableObjects/TestAce").Length;
                _aceCardsAssigned = _maxCardsInHand;
            }
#endif
        }

        protected void Start()
        {
            _statusManager = GetComponent<StatusEffectManager>();
            _combatEffManager = GetComponent<CombatEffectManager>();

            _cardsInHand = new List<GameObject>();

            _scriptablesInHand = new List<CardScriptable>();

            _partyMembers = new List<GameObject>();
            foreach (GameObject partyMem in GameObject.FindGameObjectsWithTag("PartyMember"))
            {
                _partyMembers.Add(partyMem);
                _maxCardsInHand++;
                string name = partyMem.GetComponent<CharacterTurn>()._characterName;
                int level = PartyHandler.Instance._characterLevels[name];
                if(level >= 5)
                {
                    _maxCardsInHand++;
                }
                if (level >= 6)
                {
                    _stats.UpgradeMana();
                }

                if(level >= 10)
                {
                    switch(name)
                    {
                        case "Contortionist":
                            _aceCardsAssigned++;
                            _maxCardsInHand++;
                            break;
                        case "Mascot":
                            _statusManager.AddStatusEffect(StatusEffectEnum.RAGE, 1);
                            break;
                        case "Mime":
                            _statusManager.AddStatusEffect(StatusEffectEnum.FORTIFY, 1);
                            break;
                        default:
                            break;
                    }
                }
            }

            _characterLevel = PartyHandler.Instance._characterLevels[_characterName];
            GenerateDeck();

            if(!BattleHandler.Instance._preBattleAnimation)
            {
                Invoke("PlayerStartAnimation", .5f);
            }
        }

        public void PlayerStartAnimation()
        {
            transform.DOMove(_battlePosition.position, .75f).OnComplete(delegate
            {
                _turnManager.GetComponent<BattleAgents>().BattleStartAnimation();
            });
        }

        public override void StartTurn(bool tutorial = false)
        {
            _isTurn = true;
            _ended = false;

            if (_scriptablesDiscarded != null)
                _scriptablesDiscarded.Clear();

            _numCardsLeft = _scriptablesInHand.Count;
            _firstCard = true;
            _previousCard = CardScriptable.CardClass.NONE;

            //_stats.StartTurn();

            int cardsAdded = 0;

            if(tutorial)
            {
                cardsAdded = 2;
                _numCardsLeft = 2;

                _scriptablesInHand.Add(_deck[0]);
                _deck.RemoveAt(0);

                _scriptablesInHand.Add(_deck[0]);
                _deck.RemoveAt(0);
            }
            else
            {
                for (int i = _scriptablesInHand.Count; i < (_maxCardsInHand + _additionalAceCardsAssigned) && cardsAdded < (_aceCardsAssigned + _additionalAceCardsAssigned); i++, cardsAdded++)
                {
                    if (_deck.Count == 0)
                    {
                        if (_discards.Count == 0)
                        {
                            //_numCardsLeft = _scriptablesDiscarded.Count;
                            break;
                        }
                        Shuffle();
                    }

                    _numCardsLeft++;
                    int index = Random.Range(0, _deck.Count);
                    _scriptablesInHand.Add(_deck[index]);
                    _deck.RemoveAt(index);
                }
            }

            _additionalAceCardsAssigned = 0;

            SpawnCards();

            UpdateVulnerableDiscardVisual();
            UpdatePoisonDiscardVisual();
            UpdateDamageDiscardVisual();

            _disabledCards.Clear();
            for (int i = 0; i < _disabledIndex.Count; i++)
            {
                _disabledCards.Add(_cardsInHand[_disabledIndex[i]]);
            }

            CheckAvailableMana();
        }

        private void SpawnCards()
        {
            for (int i = 0; i < _cardsInHand.Count; i++)
            {
                GameObject card = _cardsInHand[i];
                card.transform.SetParent(_handTrans);
                card.AddComponent<DraggableCard>().AddActivateListener(
                    delegate {

                        ActivateCard(card);
                    });
                card.GetComponent<DraggableCard>().Initialize(_scriptablesInHand[i], this.gameObject, "Enemy");
                card.GetComponent<Card>().Initialize(_scriptablesInHand[i], this.gameObject, "Enemy");
                if (_combatEffManager.GetCombatEffect(CombatEffectEnum.BLIND))
                {
                    card.GetComponent<Card>().Blind();
                }
            }

            for (int i = _cardsInHand.Count; i < _scriptablesInHand.Count; i++)
            {
                _cardsInHand.Add(Instantiate(_scriptablesInHand[i]._cardPrefab, _handTrans));

                GameObject card = _cardsInHand[i];
                card.name = "Card" + i;
                card.AddComponent<DraggableCard>().AddActivateListener(
                    delegate {

                        ActivateCard(card);
                    });
                card.GetComponent<DraggableCard>().Initialize(_scriptablesInHand[i], this.gameObject, "Enemy");
                card.GetComponent<Card>().Initialize(_scriptablesInHand[i], this.gameObject, "Enemy");
                if (_combatEffManager.GetCombatEffect(CombatEffectEnum.BLIND))
                {
                    card.GetComponent<Card>().Blind();
                }
            }

            _hand.Initialize(_cardsInHand);
        }

        protected void DraggableUp(GameObject target)
        {

        }

        protected override void ActivateCard(GameObject card)
        {
            _chosenCard = card;

            Card cardScript = card.GetComponent<Card>();
            AddHistory(cardScript._scriptable);

            //Do not uncomment this analytics
            /*AnalyticsEvent.Custom("Activate Card", new Dictionary<string, object>
            {
                { "Card ID", cardScript._scriptable._id }
            });*/

            BattleHandler.Instance.UseCard(cardScript._scriptable);

            if(_doubleAttack && cardScript._scriptable._class == CardScriptable.CardClass.ATTACK)
            {
                _doubleAttack = false;
                if (_combatEffManager.GetCombatEffect(CombatEffectEnum.BURN_ALL))
                {
                    cardScript.Activate(true, true);
                }
                else
                {
                    cardScript.Activate(true, _combatEffManager.GetCombatEffect(CombatEffectEnum.BURN_NEXT));
                }
            }
            else
            {

                if (_combatEffManager.GetCombatEffect(CombatEffectEnum.BURN_ALL))
                {
                    cardScript.Activate(false, true);
                }
                else
                {
                    cardScript.Activate(false, _combatEffManager.GetCombatEffect(CombatEffectEnum.BURN_NEXT));
                }

            }
            _combatEffManager.SetCombatEffect(CombatEffectEnum.BURN_NEXT, false);
            _stats.SetAnimation("attackAnim");

            _previousCard = card.GetComponent<Card>()._scriptable._class;

            _cardsInHand.Remove(card);
            _scriptablesInHand.Remove(cardScript._scriptable);
            //cardScript.Destroy();

            ApplyPerCardEffects();
            UpdateVulnerableDiscardVisual();
            UpdatePoisonDiscardVisual();
            UpdateDamageDiscardVisual();

            CheckAvailableMana();

            _numCardsLeft--;

            if (_numCardsLeft <= 0)
            {
                EndTurn();
            }

            _firstCard = false;
        }

        private void CheckAvailableMana()
        {
            float curMana = _stats._mana;
            for (int i = 0; i < _scriptablesInHand.Count; i++)
            {
                if (_cardsInHand[i] != null)
                {
                    bool available = true;
                    switch (_scriptablesInHand[i]._manaType)
                    {
                        case CardScriptable.ManaType.VALUE:
                            available = curMana >= _scriptablesInHand[i]._manaCost;
                            break;
                        case CardScriptable.ManaType.MAX:
                            available = curMana >= _stats._maxMana;
                            break;
                        case CardScriptable.ManaType.X:
                            available = curMana > 0;
                            break;
                        default:
                            available = false;
                            break;
                    }

                    if (_disabledCards.Contains(_cardsInHand[i]))
                    {
                        available = false;
                    }

                    _cardsInHand[i].GetComponent<DraggableCard>().ToggleInteractive(available);
                    Image image = _cardsInHand[i].GetComponent<Card>()._cardImage;
                    if (available)
                    {
                        Color c = image.color;
                        c.a = 1f;
                        image.color = c;
                    }
                    else
                    {
                        Color c = image.color;
                        if (_disabledCards.Contains(_cardsInHand[i]))
                        {
                            c = Color.red;
                        }
                        c.a = .5f;
                        image.color = c;
                    }
                }
            }
        }

        public void EndTurnButton()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.End_Turn_Button);
            EndTurn();
        }

        public override void EndTurn()
        {
            if (_ended)
            { return; }

            _isTurn = false;
            _ended = true;

            OnDiscard?.Invoke(_cardsInHand.Count);
            OnDiscard = null;

            if (_vulnerablePerDiscard > 0)
            {
                _statusManager.AddStatusEffect(StatusEffectEnum.VULNERABLE, _vulnerablePerDiscard * _cardsInHand.Count);
            }

            _scriptablesDiscarded = new List<CardScriptable>(_scriptablesInHand);

            _stats.EndTurn();

            base.EndTurn();

            for (int i = 0; i < _cardsInHand.Count; i++)
            {
                GameObject card = _cardsInHand[i];
                BattleHandler.Instance.DiscardCard(card.GetComponent<Card>()._scriptable);
                Destroy(card.GetComponent<DraggableCard>());
                card.GetComponent<Card>().DiscardCard(true, "Player");
                object[] magicians = FindObjectsOfType<MagicianTurn>();
                if (magicians.Length > 0)
                {
                    card.transform.DOMove(_magicianTrans.position, .5f).OnComplete(delegate { Destroy(card); });
                } else
                {
                    card.transform.DOMove(_discardTrans.position, .5f).OnComplete(delegate { Destroy(card); });
                }
                card.transform.DOScale(.45f, .5f);
            }

            _disabledIndex.Clear();
            _scriptablesInHand.Clear();
            _cardsInHand.Clear();
            _turnManager.EndTurn();

            _combatEffManager.EndTurn();
        }

        public override bool IsLastCard()
        {
            return _numCardsLeft == 1;
        }

        public override bool IsFirstCard()
        {
            return _firstCard;
        }

        public override CardScriptable.CardClass GetPreviousCard()
        {
            return _previousCard;
        }

        public override void Discard(int discardAmount)
        {
            for (int i = 0; i < discardAmount; i++)
            {
                GameObject card = _cardsInHand[Random.Range(0, _cardsInHand.Count)];
                
                if(card != _chosenCard)
                {
                    _numCardsLeft--;
                    Card cardScript = card.GetComponent<Card>();
                    BattleHandler.Instance.DiscardCard(cardScript._scriptable);
                    cardScript.DiscardCard(false, "Player");
                    _scriptablesInHand.Remove(cardScript._scriptable);
                    _cardsInHand.Remove(card);
                    Destroy(card.GetComponent<DraggableCard>());

                    card.transform.DOMove(_discardTrans.position, .5f).OnComplete(delegate { Destroy(card); });
                    card.transform.DOScale(.45f, .5f);

                    if(_numCardsLeft <= 0)
                    {
                        EndTurn();
                        return;
                    }
                }
            }

            UpdateVulnerableDiscardVisual();
            UpdatePoisonDiscardVisual();
            UpdateDamageDiscardVisual();
        }

        public void AddCardToHand(GameObject card)
        {
            _cardsInHand.Add(card);
            _scriptablesInHand.Add(card.GetComponent<Card>()._scriptable);
        }

        public void AddDirectlyToHand(CardScriptable cardScriptable)
        {
            StartCoroutine(DelayAddToHand(cardScriptable));
        }

        public void AddCardScriptableToHand(CardScriptable cardScriptable) {
            _scriptablesInHand.Add(cardScriptable);
        }

        public void AddCardToDeck(CardScriptable scriptable)
        {
            _deck.Add(scriptable);
        }

        private IEnumerator DelayAddToHand(CardScriptable scriptable)
        {
            yield return new WaitForSeconds(.1f);

            _numCardsLeft++;
            GameObject card = Instantiate(scriptable._cardPrefab, _handTrans);
            _cardsInHand.Add(card);
            _scriptablesInHand.Add(scriptable);

            card.name = "Card" + _cardsInHand.Count;
            card.AddComponent<DraggableCard>().AddActivateListener(
                delegate {

                    ActivateCard(card);
                });
            card.GetComponent<DraggableCard>().Initialize(scriptable, this.gameObject, "Enemy");
            card.GetComponent<Card>().Initialize(scriptable, this.gameObject, "Enemy");
            _hand.Initialize(card);

            CheckAvailableMana();
            _hand.FanOutCards();
        }

        public void ResetTurnValues()
        {
            _stats.StartTurn();
        }

        public int GetDeckCount()
        {
            return _cardsInHand.Count;
        }

        public int GetCardsOfType(CardScriptable.CardClass cardType)
        {
            if (cardType == CardScriptable.CardClass.NONE)
            { return _cardsInHand.Count; }

            int counter = 0;
            foreach (GameObject card in _cardsInHand)
            {
                if (card.GetComponent<Card>()._scriptable._class == cardType)
                {
                    counter++;
                }
            }
            return counter;
        }

        public static List<CardScriptable> GetDiscarded() {
            return _scriptablesDiscarded;
        }

        public void DisableCard()
        {
            List<int> availableCards = new List<int>();
            for (int i = 0; i < _maxCardsInHand; i++)
            {
                if (_disabledIndex.Contains(i))
                { continue; }

                availableCards.Add(i);
            }

            int index = Random.Range(0, availableCards.Count);
            _disabledIndex.Add(availableCards[index]);
        }

        public bool PartyBlock()
        {
            foreach (GameObject partyMem in _partyMembers)
            {
                PartyTurn turn = partyMem.GetComponent<PartyTurn>();
                if (turn._characterLevel >= 9 && !turn.IsExhausted())
                {
                    turn.PermaExhaust();
                    return true;
                }
            }
            return false;
        }

        public PartyTurn CanPartyBlock()
        {
            foreach(GameObject partyMem in _partyMembers)
            {
                PartyTurn turn = partyMem.GetComponent<PartyTurn>();
                if(turn._characterLevel >= 9 && !turn.IsExhausted())
                {
                    return turn;
                }
            }
            return null;
        }

        public void AddAceCard()
        {
            _additionalAceCardsAssigned++;
        }

        public Vector3 GetBattlePosition()
        {
            return _battlePosition.position;
        }

        private void ApplyPerCardEffects() 
        {
            /*if (_poisonPerCardUse > 0)
            {
                _statusManager.AddStatusEffect(StatusEffectEnum.POISON, _poisonPerCardUse);
            }
            if (_damagePerCardUse > 0)
            {
                _stats.TakeDamage(_damagePerCardUse, null, true, true, true);
            }*/
        }

        public override int GetNumCardsInHand()
        {
            return _cardsInHand.Count;
        }

        public void RemoveProjectionView(bool cardPlayed = true)
        {
            foreach(GameObject party in _partyMembers)
            {
                party.GetComponent<PartyTurn>().RemoveProjectionView(cardPlayed);
            }
        }

        public override int GetDamagePerDiscard()
        {
            int totalDamagePerDiscard = 0;
            foreach(CardScriptable scriptable in _scriptablesInHand)
            {
                totalDamagePerDiscard += scriptable._discardDamage;
            }
            return totalDamagePerDiscard;
        }

        public override int GetPoisonPerDiscard()
        {
            int totalPoisonPerDiscard = 0;
            foreach(CardScriptable scriptable in _scriptablesInHand)
            {
                totalPoisonPerDiscard += scriptable._discardPoison;
            }
            return totalPoisonPerDiscard;
        }
    }

}
