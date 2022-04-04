using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Handles checking for different conditions for card activations. This includes conditions
    /// like checking if the card was the first/last one used in the turn or if a certain type of
    /// card was used previously
    /// </summary>
    public class CardConditions : MonoBehaviour
    {
        //Enum representing which condition to check
        [System.Serializable]
        public enum Conditions
        {
            NO_CONDITION,
            LAST_CARD,
            FIRST_CARD,
            FATAL,
            ENEMY_DEBUFF,
            ENEMY_ARMOR,
            CROWD_FAVOR
        }

        /// <summary>
        /// Parses out and checks the condition of the card and returns true/false based off
        /// the condition evaluatoin.
        /// </summary>
        /// <param name="condition"></param> Condition to check
        /// <param name="user"></param> User of the card
        /// <param name="target"></param> Target of the card
        /// <returns></returns>
        public static bool Evaluate(Conditions condition, GameObject user, GameObject target, CardScriptable scriptable)
        {
            switch(condition)
            {
                //True if card being played is last in hand
                case Conditions.LAST_CARD:
                    return user.GetComponent<CharacterTurn>().IsLastCard();
                //True if card being played is first card this turn
                case Conditions.FIRST_CARD:
                    return user.GetComponent<CharacterTurn>().IsFirstCard();
                //True if damage dealt by this card would kill the target
                case Conditions.FATAL:
                    return target.GetComponent<CharacterStats>().CheckFatal(scriptable._targetDamage);
                //True if the target has a debuff on them
                case Conditions.ENEMY_DEBUFF:
                    return target.GetComponent<StatusEffectManager>().GetDebuffCount() > 0;
                //True if the target has any armor
                case Conditions.ENEMY_ARMOR:
                    return target.GetComponent<ArmorManager>()._armor > 0;
                //Unimplemented - true if the crowd is leaning towards the user
                case Conditions.CROWD_FAVOR:
                    return false;
                default:
                    return false;
            }
        }
    }

}
