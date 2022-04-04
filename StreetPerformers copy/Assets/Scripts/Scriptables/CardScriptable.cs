using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Scriptable object containing all card data
    /// </summary>
    [CreateAssetMenu(fileName = "CardScriptable")]
    public class CardScriptable : ScriptableObject
    {
        [System.Serializable]
        public enum CardClass
        {
            NONE,
            ATTACK,
            SUPPORT,
            DEFENSE
        }

        [System.Serializable]
        public enum AttackType
        {
            NONE,
            SINGLE,
            MULTIPLE,
            X
        }

        [System.Serializable]
        public enum ManaType
        {
            NONE,
            VALUE,
            MAX,
            X
        }

        [Header("All Card Info")]
        public string _id;
        public string _saveId;
        public GameObject _cardPrefab;
        public string _cardName;
        public CardClass _class;
        public string _cardDesc;
        [HideInInspector]
        public string _cardDescUneditted;
        public int _requiredLevel;
        public bool _canTargetSelf;
        public bool _canTargetAllAllies;
        public bool _canTargetEnemy;
        public bool _hitSingleEnemy;
        public bool _hitAllEnemies;
        public bool _openingAct;

        [Header("Mana")]
        public ManaType _manaType;
        public int _manaCost;

        [Header("Condition")]
        public CardConditions.Conditions _condition;

        [Header("Attack Damage")]
        public bool _dealsDamage;
        public AttackType _attackType;
        public int _targetDamage;
        public int _allDamage;
        public int _damageInstances;
        public float _damageMultiplier;
        public int _bonusDamage;

        [Header("Armor")]
        public bool _givesArmor;
        public int _armor;
        public int _damageReduction;
        public float _armorMultiplier;

        [Header("Mana Restore")]
        public bool _restoresMana;
        public int _manaRestore;
        public float _manaMultiplier;

        [Header("Self Damage")]
        public bool _dealsSelfDamage;
        public int _selfDamage;
        public int _selfWeakness;
        public int _selfPoison;

        [Header("Healing")]
        public bool _heals;
        public int _healingAmount;
        public int _addedMaxHealth;

        [Header("Status Effects")]
        public bool _appliesEffect;
        public int _weakness;
        public int _poison;
        public int _defense;
        public int _rage;
        public bool _burned;
        public bool _exhausted;
        public int _addedPoison;

        [Header("On Hit")]
        public bool _effectOnHit;
        public int _hitPoison;
        public int _hitWeakness;
        public int _hitDamage;

        [Header("Adjustable Value")]
        public bool _adjustable;
        public bool _burnAtZero;
        public int _damagePerUse;
        [HideInInspector]
        public int _accumulatedDamage;
        public int _armorPerUse;
        [HideInInspector]
        public int _accumulatedArmor;

        [Header("On Discard")]
        public bool _discardEffect;
        public int _discardDamage;
        public int _discardPoison;

        [Header("Special")]
        public SpecialEffect.Special _specialType;
        //Experience
        public float _expMult;
        //Discard
        public int _discardAmount;
        //Debuff Protection
        public int _protLength;
        //Missing Health
        public int _baseDamage;
        public float _missingHealthMult;
        public float _healPerMissingHealth;
        //Percentage
        public int _damageOdds;
        public int _damageOddsAmount;
        public int _armorOdds;
        public int _armorOddsAmount;
        public int _selfDamageOdds;
        public int _selfDamageOddsAmount;
        //Per Enemy Debuff
        public StatusEffectEnum _perDebuffType;
        public int _damagePerDebuff;
        //Per Card
        public CardClass _cardType;
        public float _healthPerCard;
        public float _armorPerCard;
        //Per Armor
        public float _damagePerArmor;
        public float _healthPerArmor;
        //Per Mana
        public float _armorPerMana;
        //Every Turn
        public int _permaArmor;
        //Debuff Mult
        public float _allPoisonMult;
        //Buff Mult
        public float _allArmorMult;
        //Remove Debuff
        //Transfer Armor
        //Block Next
        //Draw Card
        //Ignore Armor
        //Double Attack
        //Card Disable
        public int _cardDisables;
        //Enemy Remaining Health
        public float _remainingHealthPercentage;
        public int _remainingHealthMinimum;
        //Reduce Status
        public int _reduceAmount;
        //Add Card to Hand
        public CardScriptable _addedCard;
        //Max mana reduction
        public int _maxManaReduction;
        //Permanent armor
        public float _armorReduction;
        //Exhaust amount
        public int _exhaustAmount;
        //Per Discard
        public int _weaknessPerDiscard;
        public int _healthPerDiscard;
        //Mana reduction
        public int _manaReduction;
        //Magician additional Card
        public AdditionalMagicianCardEffectType _additionalMagicianCardEffectType;
        public int _damagePerMagicianCard;
        public int _armorPerMagicianCard;
        public bool _magicianCardStillEffective;
        //Effect Per Unused Mana
        public int _damagePerUnusedMana;
        public int _poisonPerUnusedMana;
        //Poison Hit
        public int _poisonPerHit;
        //Debuff Per Turn
        public StatusEffectEnum _debuffPerTurnType;
        public int _debuffPerTurn;

        [System.Serializable]
        public enum AdditionalMagicianCardEffectType
        {
            Damage,
            Armor
        }
        //Per Card Used
        public int _poisonPerCardUsed;
        public int _damageTakenPerCardUsed;

        public void CopyCard(CardScriptable card)
        {
            _id = card._id;
            _cardPrefab = card._cardPrefab;
            _cardName = card._cardName;
            _class = card._class;
            _cardDesc = card._cardDesc;
            _cardDescUneditted = card._cardDescUneditted;

            _manaType = card._manaType;
            _manaCost = card._manaCost;
        }
    }
}

