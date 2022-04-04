using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    /// <summary>
    /// Handles playing cards during the enemy turn.
    /// </summary>
    public class EnemyTurn : CharacterTurn
    {
        [SerializeField]
        protected int _healthPerLevel;
        public int _experience;
        [SerializeField]
        protected int _expPerLevel;

        [SerializeField]
        protected List<Image> _nextIndicators;

        //Reference to the Transform where the enemy plays its card
        protected Transform _cardTrans;
        //Reference to this enemy's CharacterStats script
        protected CharacterStats _stats;
        //The card GameObject the enemy controls
        protected GameObject _cardObj;

        protected CardScriptable _previousCard;

        [SerializeField]
        protected int _cardsPerTurn = 1;
        protected int _cardIndex = 0;
        protected int _delayedCardsPerTurn;

        protected List<CardScriptable> _nextCards;
        protected int[] _classNums;

        protected BlockAnimation _blockPanel = null;

        private bool _turnIndicatorDisabled = false;

        protected override void Awake()
        {
            base.Awake();

            _stats = GetComponent<CharacterStats>();
            _stats.AddMaxHealth(_characterLevel * _healthPerLevel, true);
            if (_characterLevel >= 10 && (_characterName.Equals("Mascot") || _characterName.Equals("Mime") || _characterName.Equals("Contortionist")))
            {
                _cardsPerTurn = 2;
                _stats.SetMaxHealth(100);
            }

            _experience += (_characterLevel * _expPerLevel);

            BattleHandler.Instance.AddExperience(_experience);
            _cardTrans = GameObject.FindGameObjectWithTag("GameplayPanel").transform.Find("EnemyCardPosition");

            _nextCards = new List<CardScriptable>();
            _classNums = new int[3];

            if(!PartyHandler.Instance._partyMembers.Contains("Mascot"))
            {
                _turnIndicatorDisabled = true;
            }
        }

        protected void Start()
        {
            _delayedCardsPerTurn = _cardsPerTurn;

            GenerateDeck();

            ChooseCards();
        }

        public void Initialize(BlockAnimation blockPanel)
        {
            _blockPanel = blockPanel;
        }

        protected override void GetAvailableCards()
        {
            Object[] cardList;
            if(_characterName == "TestEnemy")
            {
                cardList = Resources.LoadAll("ScriptableObjects/" + _characterName);
            }
            else
            {
                cardList = Resources.LoadAll("ScriptableObjects/" + _characterName + "/Level" + _characterLevel);
            }
            _deck.Clear();
            foreach(Object card in cardList)
            {
                CardScriptable scriptable = (CardScriptable)card;
                if(_characterLevel >= scriptable._requiredLevel)
                {
                    scriptable._accumulatedArmor = 0;
                    scriptable._accumulatedDamage = 0;

                    if(scriptable._adjustable)
                    {
                        string desc = scriptable._cardDescUneditted;
                        desc = desc.Replace("<TargetDamage>", "" + scriptable._targetDamage);
                        desc = desc.Replace("<DamagePerUse>", "" + Mathf.Abs(scriptable._damagePerUse));
                        desc = desc.Replace("<ArmorAmount>", "" + scriptable._armor);
                        desc = desc.Replace("<ArmorPerUse>", "" + Mathf.Abs(scriptable._armorPerUse));
                        scriptable._cardDesc = desc;
                    }
                    _deck.Add((CardScriptable)card);
                }
            }
        }

        /// <summary>
        /// Called at the start of the enemy turn. Spawns the enemy card and plays it against the
        /// player.
        /// </summary>
        public override void StartTurn(bool tutorial = false)
        {
            base.StartTurn();

            _cardIndex = 0;

            _stats.StartTurn();

            StartCardAnimation();
        }

        private void StartCardAnimation()
        {
            _cardObj = Instantiate(_nextCards[_cardIndex]._cardPrefab, _cardTrans);

            Card card = _cardObj.GetComponent<Card>();
            card.Initialize(_nextCards[_cardIndex], this.gameObject, "Player");
            _cardObj.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);

            card.InitializeProjectionTargets();
            card.ProjectedActivate();
            card.UpdateDoubleUse();
            card.UpdateBurnImage();

            PartyTurn partyBlockCharacter = card.CheckPartyBlock();

            GameObject target = card.GetTarget();


            float animationLength = .3f;
            _cardObj.transform.DOScale(1.3f, animationLength).SetEase(Ease.InSine);
            _cardObj.transform.DOMove(Camera.main.WorldToScreenPoint((new Vector3(0, 0, 0))), animationLength)
                          .OnComplete(delegate
                          { StartCoroutine(CardAttackScale(animationLength, target, partyBlockCharacter)); });
        }

        /// <summary>
        /// Starts the scaling animation when attacking the player.
        /// </summary>
        /// <param name="timeOfAnim"></param>
        public IEnumerator CardAttackScale(float timeOfAnim, GameObject target, PartyTurn partyBlockCharacter)
        {
            Vector3 targetPosition = target.transform.position;
            Card cardScript = _cardObj.GetComponent<Card>();

            if(partyBlockCharacter != null)
            {
                yield return new WaitForSeconds(1f);
                _blockPanel.gameObject.SetActive(true);
                _blockPanel.Initialize(partyBlockCharacter._characterName);
                float animDuration = _blockPanel.GetTotalTime();
                yield return new WaitForSeconds(animDuration);

                cardScript.SetPartyBlock(partyBlockCharacter);
                targetPosition = partyBlockCharacter._onScreenPos;

                float moveTime = partyBlockCharacter.MoveToCenter();
                yield return new WaitForSeconds(moveTime);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
            }

            if(cardScript.WillBurn())
            {
                AddHistory(cardScript._scriptable);
                ActivateCard();

                CardBurn cardBurn = cardScript.GetComponent<CardBurn>();

                yield return new WaitUntil(delegate { return cardBurn.BurnComplete(); } );

                StartNextCard();
            }
            else
            {
                _cardObj.transform.DOMove(Camera.main.WorldToScreenPoint(targetPosition), timeOfAnim).SetEase(Ease.OutSine);
                _cardObj.transform.DOScale(.4f, timeOfAnim);
                yield return new WaitForSeconds(timeOfAnim);

                AddHistory(cardScript._scriptable);
                ActivateCard();
                if (partyBlockCharacter != null)
                {
                    partyBlockCharacter.MoveToSide();
                }

                cardScript.Destroy();

                StartNextCard();
            }
        }

        /// <summary>
        /// Activates the enemy's card.
        /// </summary>
        protected virtual void ActivateCard()
        {
            Card card = _cardObj.GetComponent<Card>();

            if(card._scriptable._class == CardScriptable.CardClass.ATTACK)
            {
                card.Activate(_doubleAttack, false, false);
                _doubleAttack = false;
            }
            else
            {
                card.Activate(false, false, false);
            }

            _classNums[(int)card._scriptable._class - 1]--;
            SetIndicators();

            _cardIndex++;
        }

        private void StartNextCard()
        {
            if (_cardIndex >= _cardsPerTurn)
            {
                Invoke("EndTurn", 1f);
            }
            else
            {
                Invoke("StartCardAnimation", 1f);
            }
        }

        public override void EndTurn()
        {
            base.EndTurn();

            _stats.EndTurn();

            _turnManager.EndTurn();

            _cardsPerTurn = _delayedCardsPerTurn;
            ChooseCards();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                _stats.TakeDamage(1000, this.gameObject, true, true, true);
            }
        }

        protected virtual void ChooseCards()
        {
            _nextCards.Clear();

            List<CardScriptable> possibleCards = new List<CardScriptable>(_deck);
            possibleCards.Remove(_previousCard);

            _classNums[0] = 0;
            _classNums[1] = 0;
            _classNums[2] = 0;

            for(int i = 0; i < _cardsPerTurn; i++)
            {
                int index = Random.Range(0, possibleCards.Count);
                CardScriptable card = possibleCards[index];

                if(_turnManager._turnNumber == 1)
                {
                    for (int j = 0; j < possibleCards.Count; j++)
                    {
                        if (possibleCards[j]._openingAct)
                        {
                            index = j;
                            card = possibleCards[j];
                        }
                    }
                }

                if (_previousCard != null)
                {
                    if(!(_previousCard._appliesEffect && _previousCard._burned))
                    {
                        possibleCards.Add(_previousCard);
                    }
                }
                _nextCards.Add(card);
                _classNums[(int)card._class - 1]++;

                _previousCard = card;
                possibleCards.RemoveAt(index);
            }

            SetIndicators();
        }

        protected void SetIndicators()
        {
            int indicatorIndex = 0;

            if (_turnIndicatorDisabled)
            {
                for (int i = indicatorIndex; i < _nextIndicators.Count; i++)
                {
                    _nextIndicators[i].gameObject.SetActive(false);
                }
                return;
            }

            int attackNum = _classNums[(int)CardScriptable.CardClass.ATTACK - 1];
            if(attackNum > 0)
            {
                _nextIndicators[indicatorIndex].sprite = _nextCards[0]._cardPrefab.GetComponent<Card>()._attackImage;
                SetIndicator(_nextIndicators[indicatorIndex].gameObject, attackNum);
                indicatorIndex++;
            }

            int defenseNum = _classNums[(int)CardScriptable.CardClass.DEFENSE - 1];
            if(defenseNum > 0)
            {
                _nextIndicators[indicatorIndex].sprite = _nextCards[0]._cardPrefab.GetComponent<Card>()._defenseImage;
                SetIndicator(_nextIndicators[indicatorIndex].gameObject, defenseNum);
                indicatorIndex++;
            }

            int supportNum = _classNums[(int)CardScriptable.CardClass.SUPPORT - 1];
            if(supportNum > 0)
            {
                _nextIndicators[indicatorIndex].sprite = _nextCards[0]._cardPrefab.GetComponent<Card>()._supportImage;
                SetIndicator(_nextIndicators[indicatorIndex].gameObject, supportNum);
                indicatorIndex++;
            }

            for(int i = indicatorIndex; i < _nextIndicators.Count; i++)
            {
                _nextIndicators[i].gameObject.SetActive(false);
            }
        }

        private void SetIndicator(GameObject indicator, int num)
        {
            indicator.gameObject.SetActive(true);
            TextMeshProUGUI numText = indicator.transform.Find("NumberText").GetComponent<TextMeshProUGUI>();
            if(num > 1)
            {
                numText.text = "" + num;
            }
            else
            {
                numText.text = "";
            }
        }

        public void Burn(CardScriptable scriptable)
        {
            if(!_deck.Contains(scriptable))
            { return; }

            _deck.Remove(scriptable);
        }

        public void SetLevel(int level)
        {
            _characterLevel = level;
        }

        public int GetLevel()
        {
            return _characterLevel;
        }

        public void AddCardPerTurn(int amount)
        {
            _delayedCardsPerTurn += amount;
        }

        public void AddAceCard()
        {
            GameObject tempCharacter = Card.ParseUser("Ace");
            CardScriptable card = tempCharacter.GetComponent<CharacterTurn>().DrawRandomCard(false);
            AddCardToList(card);
        }

        public virtual void AddCardToList(CardScriptable card)
        {
            _cardsPerTurn++;
            _nextCards.Insert(_cardIndex + 1, card);
        }

        public override bool IsLastCard()
        {
            return _cardIndex >= _cardsPerTurn - 1;
        }

        public virtual int GetCardsOfType(CardScriptable.CardClass cardType)
        {
            if(cardType == CardScriptable.CardClass.NONE)
            {
                return _nextCards.Count - _cardIndex;
            }

            int counter = 0;
            for(int i = _cardIndex; i < _nextCards.Count; i++)
            {
                if(_nextCards[i]._class == cardType)
                {
                    counter++;
                }
            }
            return counter;
        }

        public void DoubleAttack()
        {
            _doubleAttack = true;
        }

        public Sprite GetActiveIndicator()
        {
            Card cardScript = _nextCards[0]._cardPrefab.GetComponent<Card>();

            if(_classNums[(int)CardScriptable.CardClass.ATTACK - 1] > 0)
            {
                return cardScript._attackImage;
            }
            else if(_classNums[(int)CardScriptable.CardClass.DEFENSE - 1] > 0)
            {
                return cardScript._defenseImage;
            }
            else if(_classNums[(int)CardScriptable.CardClass.SUPPORT - 1] > 0)
            {
                return cardScript._supportImage;
            }

            return cardScript._attackImage;
        }

        public Vector3 GetActiveIndicatorPos()
        {
            return _nextIndicators[0].transform.position;
        }
    }
}