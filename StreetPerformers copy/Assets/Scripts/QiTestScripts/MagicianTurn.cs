using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace StreetPerformers
{
    public class MagicianTurn : EnemyTurn
    {
        private List<CardScriptable> _cardsToUse;

        public override void StartTurn(bool tutorial = false)
        {
            _stats.StartTurn();
            StartCoroutine(StartTurnProcess());
        }

        private IEnumerator StartTurnProcess() 
        {
            //int index = ChooseCardFromPool();
            _cardsToUse = new List<CardScriptable>();
            _cardsToUse = PlayerTurn.GetDiscarded();
            _cardIndex = 0;
            _cardsToUse.Insert(0, _nextCards[_cardIndex]);

            yield return StartCoroutine(LoopDeck());

            base.EndTurn();
        }

        /// Use all cards in the list. When 1 card is done, it would start another
        private IEnumerator LoopDeck() 
        {
            for(_cardIndex = 0; _cardIndex < _cardsToUse.Count; _cardIndex++)
            {
                yield return StartCoroutine(UseCard(_cardsToUse[_cardIndex]));
            }
            yield return null;
        }



        /// deal with the effect and animation of the card use
        private IEnumerator UseCard(CardScriptable scriptable) 
        {
            _cardObj = Instantiate(scriptable._cardPrefab, _cardTrans);

            Card cardScript = _cardObj.GetComponent<Card>();
            cardScript.Initialize(scriptable, this.gameObject, "Player");
            _cardObj.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);

            cardScript.InitializeProjectionTargets();
            cardScript.ProjectedActivate();
            cardScript.UpdateDoubleUse();
            cardScript.UpdateBurnImage();

            PartyTurn partyBlockCharacter = cardScript.CheckPartyBlock();

            GameObject target = cardScript.GetTarget();
            if (!target)
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }

            float animationLength = .3f;
            _cardObj.transform.DOScale(1.3f, animationLength).SetEase(Ease.InSine);
            _cardObj.transform.DOMove(Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0)), animationLength);

            yield return new WaitForSeconds(animationLength);

            yield return StartCoroutine(CardAttackScale(animationLength, target, partyBlockCharacter));

            yield return new WaitForSeconds(1);
        }

        /// <summary>
        /// override the ActivateCard function in the enemy turn
        /// prevent it from endTurn right after card being used
        /// </summary>
        protected override void ActivateCard()
        {
            Card cardScript = _cardObj.GetComponent<Card>();

            if(_doubleAttack && _cardObj.GetComponent<Card>()._scriptable._class == CardScriptable.CardClass.ATTACK)
            {
                cardScript.Activate(_doubleAttack, false, false);
                _doubleAttack = false;
            }
            else
            {
                cardScript.Activate(false, false, false);
            }

            SetIndicators();
        }

        public void SetScriptablesDiscarded() {
            
        
        }

        public override void AddCardToList(CardScriptable scriptable) 
        {
            _cardsToUse.Insert(_cardIndex + 1, scriptable);
        }

        public override void Discard(int numToDiscard) 
        {
            for(int i = 0; i < numToDiscard; i++)
            {
                if(_cardIndex < _cardsToUse.Count - 1)
                {
                    _cardsToUse.RemoveAt(_cardsToUse.Count - 1);
                }
            }
        }

        public override int GetCardsOfType(CardScriptable.CardClass cardType)
        {
            if (cardType == CardScriptable.CardClass.NONE)
            { 
                return _cardsToUse.Count - _cardIndex; 
            }

            int counter = 0;
            for(int i = _cardIndex; i < _cardsToUse.Count; i++)
            {
                if(_cardsToUse[i]._class == cardType)
                {
                    counter++;
                }
            }
            return counter;
        }

        public override bool IsLastCard()
        {
            return _cardIndex == _cardsToUse.Count - 1;
        }
    }
}