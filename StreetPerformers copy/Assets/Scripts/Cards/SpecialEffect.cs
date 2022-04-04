using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StreetPerformers
{
    public class SpecialEffect : MonoBehaviour
    {
        [System.Serializable]
        public enum Special
        {
            NONE,
            EXPERIENCE,
            DISCARD,
            DEBUFF_PROT,
            MISSING_HEALTH,
            PERCENTAGE,
            PER_ENEMY_DEBUFF,
            PER_CARD,
            PER_ARMOR,
            PER_MANA,
            EVERY_TURN,
            DEBUFF_MULT,
            BUFF_MULT,
            REMOVE_DEBUFF,
            TRANSFER_ARMOR,
            BLOCK_NEXT,
            DRAW_CARD,
            IGNORE_ARMOR,
            DOUBLE_ATTACK,
            PER_TURN,
            PERMANENT_ARMOR,
            BLOCK_ALL,
            CARD_DISABLE,
            ENEMY_REMAINING_HEALTH,
            EXHAUST_RANDOM,
            ADD_CARD_PER_TURN,
            REDUCE_STATUS,
            BURN_NEXT,
            ASSIGN_RANDOM_CARDS,
            BLIND,
            ADD_CARD_TO_DECK,
            PER_DISCARD,
            PER_CARD_USED,
            REDUCE_MAX_MANA,
            REDUCE_MANA,
            ADD_ACE_CARD,
            LEFTOVER_ARMOR,
            MARK_AS_BURNED,
            POISON_HIT,
            DEBUFF_PER_TURN
        }

        public static void ApplySpecial(CardScriptable scriptable, GameObject user, List<GameObject> targets, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            switch(scriptable._specialType)
            {
                case Special.EXPERIENCE:
                    Experience(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.DISCARD:
                    Discard(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.DEBUFF_PROT:
                    DebuffProtection(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.MISSING_HEALTH:
                    MissingHealth(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PERCENTAGE:
                    Percentage(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PER_ENEMY_DEBUFF:
                    PerEnemyDebuff(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PER_CARD:
                    PerCard(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PER_ARMOR:
                    PerArmor(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PER_MANA:
                    PerMana(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.EVERY_TURN:
                    EveryTurn(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.DEBUFF_MULT:
                    DebuffMult(targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.BUFF_MULT:
                    BuffMult(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.REMOVE_DEBUFF:
                    RemoveDebuffs(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.TRANSFER_ARMOR:
                    TransferArmor(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.BLOCK_NEXT:
                    BlockNext(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.DRAW_CARD:
                    DrawCard(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.IGNORE_ARMOR:
                    IgnoreArmor(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.DOUBLE_ATTACK:
                    DoubleAttack(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PER_TURN:
                    PerTurn(user, targets, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PERMANENT_ARMOR:
                    PermanentArmor(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.BLOCK_ALL:
                    BlockAll(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.CARD_DISABLE:
                    CardDisable(targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.ENEMY_REMAINING_HEALTH:
                    EnemyRemainingHealth(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.EXHAUST_RANDOM:
                    ExhaustRandom(scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.ADD_CARD_PER_TURN:
                    AddCardPerTurn(user, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.REDUCE_STATUS:
                    ReduceStatus(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.BURN_NEXT:
                    BurnNext(targets, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.ASSIGN_RANDOM_CARDS:
                    AssignRandomCards(ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.BLIND:
                    Blind(targets, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.ADD_CARD_TO_DECK:
                    AddCardToDeck(targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.REDUCE_MAX_MANA:
                    ReduceMaxMana(targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PER_DISCARD:
                    PerDiscard(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.PER_CARD_USED:
                    PerCardUsed(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.REDUCE_MANA:
                    ReduceMana(targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.ADD_ACE_CARD:
                    AddAceCard(user, targets, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.LEFTOVER_ARMOR:
                    LeftoverArmor(user, targets, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.MARK_AS_BURNED:
                    MarkAsBurned(targets, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.POISON_HIT:
                    PoisonHit(user, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.DEBUFF_PER_TURN:
                    DebuffPerTurn(user, targets, scriptable, ref projectedChanges, checkProjection, doubleActivate);
                    break;
                case Special.NONE:
                default:
                    break;
            }
        }

        private static void Experience(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(user.tag != "Player" || checkProjection)
            { return; }

            BattleHandler.Instance._experienceMultiplier *= scriptable._expMult;
            if(doubleActivate)
            {
                BattleHandler.Instance._experienceMultiplier *= scriptable._expMult;
            }
        }

        /// <summary>
        /// Handles the SloppyAttack ability that discards cards after dealing damage
        /// </summary>
        private static void Discard(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            user.GetComponent<CharacterTurn>().Discard(scriptable._discardAmount);
            if(doubleActivate)
            {
                user.GetComponent<CharacterTurn>().Discard(scriptable._discardAmount);
            }
        }

        private static void DebuffProtection(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            user.GetComponent<CombatEffectManager>().SetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION, true);
        }

        /// <summary>
        /// Handles the Heroics ability which deals damage based off missing health.
        /// </summary>
        private static void MissingHealth(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            CharacterStats userStats = user.GetComponent<CharacterStats>();
            float missingHealth = user.GetComponent<CharacterStats>().GetMissingHealth();
            float damage = scriptable._baseDamage + Mathf.Round(missingHealth * scriptable._missingHealthMult);

            bool ignoreArmor = user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR);
            foreach(GameObject target in targets)
            {
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                targetStats.CheckProjectedDamage(damage, user, ignoreArmor, false, false, ref projectedChanges);
            }

            if(scriptable._healPerMissingHealth > 0f)
            {
                projectedChanges[userStats]._healChange += Mathf.RoundToInt(missingHealth * scriptable._healPerMissingHealth);
            }
        }

        private static void Percentage(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            bool ignoreArmor = user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR);

            int damageOdds = scriptable._damageOdds;
            int armorOdds = scriptable._armorOdds;
            int selfOdds = scriptable._selfDamageOdds;
            int total = damageOdds + armorOdds + selfOdds;

            int randVal = Random.Range(0, total);
            if(randVal < damageOdds)
            {
                foreach(GameObject target in targets)
                {
                    CharacterStats targetStats = target.GetComponent<CharacterStats>();
                    targetStats.CheckProjectedDamage(scriptable._damageOddsAmount, user, ignoreArmor, false, false, ref projectedChanges, true);
                }
            }
            else if(randVal < damageOdds + armorOdds)
            {
                projectedChanges[user.GetComponent<CharacterStats>()]._hiddenArmorChange += scriptable._armorOddsAmount;
            }
            else
            {
                CharacterStats userStats = user.GetComponent<CharacterStats>();
                userStats.CheckProjectedDamage(scriptable._selfDamageOddsAmount, user, ignoreArmor, true, true, ref projectedChanges, true);
            }
        }

        private static void PerEnemyDebuff(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            bool ignoreArmor = user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR);
            foreach(GameObject target in targets)
            {
                float damage = scriptable._damagePerDebuff * target.GetComponent<StatusEffectManager>().GetDebuffCount(scriptable._perDebuffType);

                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                targetStats.CheckProjectedDamage(damage, user, ignoreArmor, false, false, ref projectedChanges);
            }
        }

        private static void PerCard(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            int cardCounter;
            if (user.tag == "Player")
            {
                cardCounter = user.GetComponent<PlayerTurn>().GetCardsOfType(scriptable._cardType);
            }
            else 
            {
                cardCounter = user.GetComponent<EnemyTurn>().GetCardsOfType(scriptable._cardType);
            }

            if(scriptable._armorPerCard > 0)
            {
                projectedChanges[user.GetComponent<CharacterStats>()]._armorChange += Mathf.RoundToInt(cardCounter * scriptable._armorPerCard);
            }

            if(scriptable._healthPerCard > 0)
            {
                CharacterStats userStats = user.GetComponent<CharacterStats>();
                projectedChanges[userStats]._healChange += Mathf.RoundToInt(cardCounter * scriptable._healthPerCard);
            }
        }

        private static void PerArmor(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            CharacterStats stat = user.GetComponent<CharacterStats>();
            ArmorManager armorMan = user.GetComponent<ArmorManager>();
            int armor = armorMan._armor;
            
            if(scriptable._damagePerArmor > 0)
            {
                bool ignoreArmor = user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR);
                float damage = armor * scriptable._damagePerArmor;
                foreach(GameObject target in targets)
                {
                    CharacterStats targetStats = target.GetComponent<CharacterStats>();
                    targetStats.CheckProjectedDamage(damage, user, ignoreArmor, false, false, ref projectedChanges);
                }
            }

            if(scriptable._healthPerArmor > 0)
            {
                projectedChanges[stat]._healChange += Mathf.RoundToInt(armor * scriptable._healthPerArmor);
                projectedChanges[stat]._armorChange -= armor;
            }
        }

        private static void PerMana(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            if(user.tag == "Player")
            {
                float armor = user.GetComponent<PlayerStats>()._mana * scriptable._armorPerMana;
                projectedChanges[user.GetComponent<CharacterStats>()]._armorChange += Mathf.RoundToInt(armor);
            }
            else
            {
                projectedChanges[user.GetComponent<CharacterStats>()]._armorChange += Mathf.RoundToInt(5 * scriptable._armorPerMana);
            }
        }

        private static void EveryTurn(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            int permaArmor = scriptable._permaArmor;
            if(doubleActivate)
            {
                permaArmor *= 2;
            }

            user.GetComponent<ArmorManager>().AddPersistentArmor(permaArmor);
        }

        private static void DebuffMult(List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            foreach(GameObject target in targets)
            {
                target.GetComponent<StatusEffectManager>().MultiplyStatusEffect(StatusEffectEnum.POISON, scriptable._allPoisonMult, ref projectedChanges, checkProjection);
            }
        }

        private static void BuffMult(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            if(scriptable._allArmorMult > 0)
            {
                user.GetComponent<ArmorManager>().Multiply(scriptable._allArmorMult, ref projectedChanges, checkProjection);
            }
        }

        private static void RemoveDebuffs(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            user.GetComponent<StatusEffectManager>().RemoveDebuffs(ref projectedChanges, checkProjection);
        }

        private static void TransferArmor(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            user.GetComponent<ArmorManager>().TransferArmor(ref projectedChanges, checkProjection);
        }

        /// <summary>
        /// Handles the substitute ability that reduces incoming damage.
        /// </summary>
        private static void BlockNext(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            user.GetComponent<CombatEffectManager>().SetCombatEffect(CombatEffectEnum.BLOCK_NEXT, true);
        }

        private static void DrawCard(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            GameObject character = Card.ParseUser(scriptable._id);

            if(character == null)
            { return; }

            if (user.tag == "Player")
            {
                CardScriptable card = character.GetComponent<CharacterTurn>().DrawRandomCard(true);
                user.GetComponent<PlayerTurn>().AddDirectlyToHand(card);
                if(doubleActivate)
                {
                    user.GetComponent<PlayerTurn>().AddDirectlyToHand(card);
                }
            } 
            else
            {
                CardScriptable card = character.GetComponent<CharacterTurn>().DrawRandomCard(false);
                user.GetComponent<EnemyTurn>().AddCardToList(card);
                if(doubleActivate)
                {
                    user.GetComponent<EnemyTurn>().AddCardToList(card);
                }
            }
        }

        private static void IgnoreArmor(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            user.GetComponent<CombatEffectManager>().SetCombatEffect(CombatEffectEnum.IGNORE_ARMOR, true);
        }

        private static void DoubleAttack(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            if(user.tag == "Player")
            {
                user.GetComponent<PlayerTurn>()._doubleAttack = true;
            }
            else
            {
                user.GetComponent<EnemyTurn>().DoubleAttack();
            }
            
        }

        /// <summary>
        /// Handles the RiseAndFall ability that deals damage based off the turn number.
        /// </summary>
        private static void PerTurn(GameObject user, List<GameObject> targets, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            bool ignoreArmor = user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR);
            float damage = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>()._turnNumber;
            foreach(GameObject target in targets)
            {
                CharacterStats targetStat = target.GetComponent<CharacterStats>();
                targetStat.CheckProjectedDamage(damage, user, ignoreArmor, false, false, ref projectedChanges);
            }
        }

        private static void PermanentArmor(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            user.GetComponent<ArmorManager>().PermanentArmor(scriptable._armorReduction);
        }

        private static void BlockAll(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            user.GetComponent<CombatEffectManager>().SetCombatEffect(CombatEffectEnum.BLOCK_ALL, true);
        }

        private static void CardDisable(List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            int cardDisables = scriptable._cardDisables;
            if(doubleActivate)
            {
                cardDisables *= 2;
            }

            foreach(GameObject target in targets)
            {
                for(int i = 0; i < cardDisables; i++)
                {
                    target.GetComponent<PlayerTurn>().DisableCard();
                }
            }
        }

        private static void EnemyRemainingHealth(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            foreach(GameObject target in targets)
            {
                CharacterStats stat = target.GetComponent<CharacterStats>();
                float health = stat._health;
                float damage = Mathf.Max(health * scriptable._remainingHealthPercentage, scriptable._remainingHealthMinimum);

                stat.CheckProjectedDamage(damage, user, false, false, false, ref projectedChanges);
            }
        }

        private static void AddCardPerTurn(GameObject user, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            int cardPerTurn = 1;
            if(doubleActivate)
            {
                cardPerTurn *= 2;
            }

            user.GetComponent<EnemyTurn>().AddCardPerTurn(cardPerTurn);
        }

        private static void ExhaustRandom(CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            int exhaustAmount = scriptable._exhaustAmount;
            if(doubleActivate)
            {
                exhaustAmount *= 2;
            }

            BattleAgents agent = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleAgents>();
            for(int i = 0; i < exhaustAmount; i++)
            {
                agent.ExhaustRandomParty();
            }
        }

        private static void ReduceStatus(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            user.GetComponent<StatusEffectManager>().ReduceStatus(scriptable._reduceAmount, ref projectedChanges, checkProjection);
            foreach(GameObject target in targets)
            {
                target.GetComponent<StatusEffectManager>().ReduceStatus(scriptable._reduceAmount, ref projectedChanges, checkProjection);
            }
        }

        private static void BurnNext(List<GameObject> targets, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            foreach(GameObject target in targets)
            {
                target.GetComponent<CombatEffectManager>().SetCombatEffect(CombatEffectEnum.BURN_NEXT, true);
            }
        }

        private static void AssignRandomCards(ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleAgents>().AssignRandomCardToAll();
        }

        private static void Blind(List<GameObject> targets, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            foreach(GameObject target in targets)
            {
                target.GetComponent<CombatEffectManager>().SetCombatEffect(CombatEffectEnum.BLIND, true);
            }
        }

        private static void AddCardToDeck(List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            foreach(GameObject target in targets)
            {
                target.GetComponent<PlayerTurn>().AddCardToDeck(scriptable._addedCard);
                if(doubleActivate)
                {
                    target.GetComponent<PlayerTurn>().AddCardToDeck(scriptable._addedCard);
                }
            }
        }

        private static void ReduceMaxMana(List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            foreach(GameObject target in targets)
            {
                projectedChanges[target.GetComponent<CharacterStats>()]._maxManaChange -= scriptable._maxManaReduction;
            }
        }

        private static void PerDiscard(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(scriptable._weaknessPerDiscard > 0)
            {
                foreach(GameObject target in targets)
                {
                    if(checkProjection)
                    {
                        projectedChanges[target.GetComponent<CharacterStats>()]._vulnerablePerDiscard += scriptable._weaknessPerDiscard;
                    }
                }
            }

            if(scriptable._healthPerDiscard > 0)
            {
                CharacterStats userStats = user.GetComponent<CharacterStats>();
                if(checkProjection)
                {
                    projectedChanges[userStats]._healthPerDiscard += scriptable._healthPerDiscard;
                }
                else
                {
                    foreach(GameObject target in targets)
                    {
                        target.GetComponent<PlayerTurn>().OnDiscard += userStats.HealthPerDiscard;
                    }
                }
            }
        }

        private static void ReduceMana(List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            int manaReduction = scriptable._manaReduction;
            if(doubleActivate)
            {
                manaReduction *= 2;
            }

            foreach(GameObject target in targets)
            {
                PlayerStats playerStats = target.GetComponent<PlayerStats>();
                if(playerStats != null)
                {
                    playerStats.SetReduceMana(manaReduction);
                }
            }
        }

        private static void AddAceCard(GameObject user, List<GameObject> targets, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false) 
        {
            if(checkProjection)
            { return; }

            EnemyTurn enemyTurn = user.GetComponent<EnemyTurn>();
            if(enemyTurn != null) 
            {
                enemyTurn.AddAceCard();
                if(doubleActivate)
                {
                    enemyTurn.AddAceCard();
                }
                return;
            }

            foreach (GameObject target in targets)
            {
                PlayerTurn playerTurn = target.GetComponent<PlayerTurn>();
                if (playerTurn != null)
                {
                    playerTurn.AddAceCard();
                    if(doubleActivate)
                    {
                        playerTurn.AddAceCard();
                    }
                }
            }
        }

        private static void PerCardUsed(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(!checkProjection)
            { return; }

            foreach(GameObject target in targets)
            {
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                projectedChanges[targetStats]._poisonPerCardUsed += scriptable._poisonPerCardUsed;
                projectedChanges[targetStats]._damagePerCardUsed += scriptable._damageTakenPerCardUsed;
            }
        }

        private static void LeftoverArmor(GameObject user, List<GameObject> targets, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            user.GetComponent<CharacterStats>().LeftoverArmorAttack(targets);
        }

        private static void MarkAsBurned(List<GameObject> targets, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false) 
        {
            if(checkProjection)
            { return; }

            foreach(GameObject target in targets) 
            {
                target.GetComponent<CombatEffectManager>().SetCombatEffect(CombatEffectEnum.BURN_ALL, true);    
            }
        }

        private static void PoisonHit(GameObject user, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if (checkProjection)
            { return; }

            CombatEffectManager combatEffectManager = user.GetComponent<CombatEffectManager>();
            combatEffectManager.SetCombatEffect(CombatEffectEnum.POISON_HIT, true);
            combatEffectManager.AddCombatEffectValue(CombatEffectEnum.POISON_HIT, scriptable._poisonPerHit);
        }

        private static void DebuffPerTurn(GameObject user, List<GameObject> targets, CardScriptable scriptable, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false, bool doubleActivate = false)
        {
            if(checkProjection)
            { return; }

            switch(scriptable._debuffPerTurnType)
            {
                case StatusEffectEnum.POISON:
                case StatusEffectEnum.VULNERABLE:
                    foreach (GameObject obj in targets)
                    {
                        obj.GetComponent<StatusEffectManager>()?.AddStatusEffectPerTurn(scriptable._debuffPerTurnType, scriptable._debuffPerTurn);
                    }
                    break;
                case StatusEffectEnum.FORTIFY:
                case StatusEffectEnum.RAGE:
                    user.GetComponent<StatusEffectManager>()?.AddStatusEffectPerTurn(scriptable._debuffPerTurnType, scriptable._debuffPerTurn);
                    break;
            }
        }
    }
}
