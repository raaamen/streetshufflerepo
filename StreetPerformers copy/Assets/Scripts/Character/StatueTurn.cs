using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Handles playing cards during the enemy turn.
    /// </summary>
    public class StatueTurn : EnemyTurn
    {
        private CardScriptable _exhaustCard;

        protected override void GetAvailableCards()
        {
            Object[] cardList = Resources.LoadAll("ScriptableObjects/" + _characterName + "/Level" + _characterLevel);
            _deck.Clear();

            foreach(Object card in cardList)
            {
                CardScriptable scriptable = (CardScriptable)card;
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

                if(_exhaustCard == null)
                {
                    //_exhaustCard = (CardScriptable)card;
                    //continue;
                }
                _deck.Add((CardScriptable)card);
            }
        }

        protected override void ChooseCards()
        {
            base.ChooseCards();

            /*_nextCards.Clear();

            List<CardScriptable> possibleCards = new List<CardScriptable>(_deck);
            possibleCards.Remove(_previousCard);
            if(possibleCards.Contains(_exhaustCard))
            {
                possibleCards.Remove(_exhaustCard);
            }

            _classNums[0] = 0;
            _classNums[1] = 0;
            _classNums[2] = 0;

            _nextCards.Add(_exhaustCard);
            _classNums[(int)_exhaustCard._class - 1]++;

            for(int i = 1; i < _cardsPerTurn; i++)
            {
                int index = Random.Range(0, possibleCards.Count);
                CardScriptable card = possibleCards[index];
                if(_previousCard != null)
                {
                    if(!(_previousCard._appliesEffect && _previousCard._exhausted))
                    {
                        possibleCards.Add(_previousCard);
                    }
                }
                _nextCards.Add(card);
                _classNums[(int)card._class - 1]++;

                _previousCard = card;
                possibleCards.RemoveAt(index);
            }

            SetIndicators();*/
        }
    }
}