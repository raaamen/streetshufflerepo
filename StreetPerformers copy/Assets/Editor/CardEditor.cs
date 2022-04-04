using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

namespace StreetPerformers
{
    [CustomEditor(typeof(CardScriptable))]
    public class CardEditor : Editor
    {
        [SerializeField]
        private CardScriptable _scriptable;

        private string[] _specialNames;
        private string[] _classNames;
        private string[] _attackNames;
        private string[] _conditionNames;
        private string[] _manaNames;
        private string[] _magicianCardEffectTypeNames;
        private string[] _perDebuffNames;

        private void OnEnable()
        {
            _scriptable = (CardScriptable)this.target;

            _specialNames = Enum.GetNames(typeof(SpecialEffect.Special));
            _classNames = Enum.GetNames(typeof(CardScriptable.CardClass));
            _attackNames = Enum.GetNames(typeof(CardScriptable.AttackType));
            _conditionNames = Enum.GetNames(typeof(CardConditions.Conditions));
            _manaNames = Enum.GetNames(typeof(CardScriptable.ManaType));
            _magicianCardEffectTypeNames = Enum.GetNames(typeof(CardScriptable.AdditionalMagicianCardEffectType));
            _perDebuffNames = Enum.GetNames(typeof(StatusEffectEnum));
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();

            //All card information section
            AllCardInfo();

            //Condition Section
            EditorGUILayout.LabelField("");
            _scriptable._condition = (CardConditions.Conditions)EditorGUILayout.Popup("Condition", (int)_scriptable._condition, _conditionNames);

            //Attack damage section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Attack Damage", EditorStyles.boldLabel);
            if(_scriptable._dealsDamage = EditorGUILayout.Toggle("Deals Damage", _scriptable._dealsDamage))
            {
                AttackDamage();
            }

            //Armor section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Armor", EditorStyles.boldLabel);
            if(_scriptable._givesArmor = EditorGUILayout.Toggle("Gives Armor", _scriptable._givesArmor))
            {
                Armor();
            }

            //Mana restore section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Mana Restore", EditorStyles.boldLabel);
            if(_scriptable._restoresMana = EditorGUILayout.Toggle("Restores Mana", _scriptable._restoresMana))
            {
                Mana();
            }

            //Self damage section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Self Damage", EditorStyles.boldLabel);
            if(_scriptable._dealsSelfDamage = EditorGUILayout.Toggle("Deals Self Damage", _scriptable._dealsSelfDamage))
            {
                SelfDamage();
            }

            //Heal section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Healing", EditorStyles.boldLabel);
            if(_scriptable._heals = EditorGUILayout.Toggle("Heals", _scriptable._heals))
            {
                Healing();
            }

            //Status effect section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Status Effects", EditorStyles.boldLabel);
            if(_scriptable._appliesEffect = EditorGUILayout.Toggle("Applies Status Effect", _scriptable._appliesEffect))
            {
                StatusEffects();
            }
            else
            {
                _scriptable._burned = false;
                _scriptable._exhausted = false;
            }

            //On hit section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("On Hit", EditorStyles.boldLabel);
            if(_scriptable._effectOnHit = EditorGUILayout.Toggle("Effect On Hit", _scriptable._effectOnHit))
            {
                OnHit();
            }

            //Adjustable value section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Adjustable", EditorStyles.boldLabel);
            if(_scriptable._adjustable = EditorGUILayout.Toggle("Adjustable", _scriptable._adjustable))
            {
                Adjustable();
            }

            //On Discard section
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("On Discard", EditorStyles.boldLabel);
            if(_scriptable._discardEffect = EditorGUILayout.Toggle("Discard Effect", _scriptable._discardEffect))
            {
                OnDiscard();
            }

            EditorGUILayout.LabelField("");
            SpecialEffects();

            UpdateDescription();

            EditorGUILayout.EndVertical();

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_scriptable);
                EditorSceneManager.MarkAllScenesDirty();
                AssetDatabase.SaveAssets();
            }
        }

        private void AllCardInfo()
        {
            EditorGUILayout.LabelField("All Card Info", EditorStyles.boldLabel);
            _scriptable._id = EditorGUILayout.TextField("Card ID", _scriptable._id);
            _scriptable._saveId = EditorGUILayout.TextField("Save ID", _scriptable._saveId);
            _scriptable._cardPrefab = (GameObject)EditorGUILayout.ObjectField("Card Prefab", _scriptable._cardPrefab, typeof(GameObject), true);
            _scriptable._cardName = EditorGUILayout.TextField("Card Name", _scriptable._cardName);
            
            _scriptable._class = (CardScriptable.CardClass)EditorGUILayout.Popup("Card Class", (int)_scriptable._class, _classNames);

            _scriptable._cardDescUneditted = EditorGUILayout.TextField("Card Description", _scriptable._cardDescUneditted);
            _scriptable._requiredLevel = EditorGUILayout.IntSlider("Required Level", _scriptable._requiredLevel, 0, 10);
            
            _scriptable._canTargetSelf = EditorGUILayout.Toggle("Can Target Self", _scriptable._canTargetSelf);
            if(_scriptable._canTargetSelf)
            {
                _scriptable._canTargetAllAllies = EditorGUILayout.Toggle("Can Target All Allies", _scriptable._canTargetAllAllies);
            }
            else
            {
                _scriptable._canTargetAllAllies = false;
            }

            _scriptable._canTargetEnemy = EditorGUILayout.Toggle("Can Target Enemy", _scriptable._canTargetEnemy);
            if(_scriptable._canTargetEnemy)
            {
                _scriptable._hitSingleEnemy = EditorGUILayout.Toggle("Hit Single Enemy", _scriptable._hitSingleEnemy);
                _scriptable._hitAllEnemies = EditorGUILayout.Toggle("Hit All Enemies", _scriptable._hitAllEnemies);
            }
            else
            {
                _scriptable._hitSingleEnemy = false;
                _scriptable._hitAllEnemies = false;
            }

            _scriptable._openingAct = EditorGUILayout.Toggle("Opening Act", _scriptable._openingAct);

            _scriptable._manaType = (CardScriptable.ManaType)EditorGUILayout.Popup("Mana Type", (int)_scriptable._manaType, _manaNames);
            if(_scriptable._manaType == CardScriptable.ManaType.VALUE)
            {
                _scriptable._manaCost = EditorGUILayout.IntField("Mana Cost", _scriptable._manaCost);
            }
            else
            {
                _scriptable._manaCost = 0;
            }
        }

        private void AttackDamage()
        {
            _scriptable._attackType = (CardScriptable.AttackType)EditorGUILayout.Popup("Attack Type", (int)_scriptable._attackType, _attackNames);
            if(_scriptable._attackType != CardScriptable.AttackType.NONE)
            {
                if(_scriptable._hitSingleEnemy)
                {
                    _scriptable._targetDamage = EditorGUILayout.IntField("Target Damage", _scriptable._targetDamage);
                }
                else
                {
                    _scriptable._targetDamage = 0;
                }

                if(_scriptable._hitAllEnemies)
                {
                    _scriptable._allDamage = EditorGUILayout.IntField("All Damage", _scriptable._allDamage);
                }
                else
                {
                    _scriptable._allDamage = 0;
                }
                
                if(_scriptable._attackType == CardScriptable.AttackType.MULTIPLE)
                {
                    _scriptable._damageInstances = EditorGUILayout.IntField("Damage Instances", _scriptable._damageInstances);
                }
                else
                {
                    _scriptable._damageInstances = 0;
                }

                if(_scriptable._condition != CardConditions.Conditions.NO_CONDITION)
                {
                    _scriptable._damageMultiplier = EditorGUILayout.FloatField("Damage Multiplier", _scriptable._damageMultiplier);
                    _scriptable._bonusDamage = EditorGUILayout.IntField("Bonus Damage", _scriptable._bonusDamage);
                }
                else
                {
                    _scriptable._damageMultiplier = 0;
                    _scriptable._bonusDamage = 0;
                }

            }
            else
            {
                _scriptable._targetDamage = 0;
                _scriptable._allDamage = 0;
                _scriptable._damageInstances = 0;
                _scriptable._damageMultiplier = 0;
                _scriptable._bonusDamage = 0;
            }
        }

        private void Armor()
        {
            _scriptable._armor = EditorGUILayout.IntField("Armor Amount", _scriptable._armor);
            _scriptable._damageReduction = EditorGUILayout.IntField("Damage Reduction", _scriptable._damageReduction);
            if(_scriptable._condition != CardConditions.Conditions.NO_CONDITION)
            {
                _scriptable._armorMultiplier = EditorGUILayout.FloatField("Armor Multiplier", _scriptable._armorMultiplier);
            }
            else
            {
                _scriptable._armorMultiplier = 0;
            }
        }

        private void Mana()
        {
            _scriptable._manaRestore = EditorGUILayout.IntField("Mana Restore", _scriptable._manaRestore);

            if(_scriptable._condition != CardConditions.Conditions.NO_CONDITION)
            {
                _scriptable._manaMultiplier = EditorGUILayout.FloatField("Mana Multiplier", _scriptable._manaMultiplier);
            }
            else
            {
                _scriptable._manaMultiplier = 0;
            }
        }

        private void SelfDamage()
        {
            _scriptable._selfDamage = EditorGUILayout.IntField("Self Damage", _scriptable._selfDamage);
            _scriptable._selfPoison = EditorGUILayout.IntField("Self Poison", _scriptable._selfPoison);
            _scriptable._selfWeakness = EditorGUILayout.IntField("Self Weakness", _scriptable._selfWeakness);
        }

        private void Healing()
        {
            _scriptable._healingAmount = EditorGUILayout.IntField("Healing Amount", _scriptable._healingAmount);
            if(_scriptable._condition != CardConditions.Conditions.NO_CONDITION)
            {
                _scriptable._addedMaxHealth = EditorGUILayout.IntField("Added Max Health", _scriptable._addedMaxHealth);
            }
            else
            {
                _scriptable._addedMaxHealth = 0;
            }
        }

        private void StatusEffects()
        {
            _scriptable._weakness = EditorGUILayout.IntField("Weakness", _scriptable._weakness);
            _scriptable._poison = EditorGUILayout.IntField("Poison", _scriptable._poison);
            _scriptable._defense = EditorGUILayout.IntField("Defense", _scriptable._defense);
            _scriptable._rage = EditorGUILayout.IntField("Rage", _scriptable._rage);
            _scriptable._burned = EditorGUILayout.Toggle("Burned", _scriptable._burned);
            _scriptable._exhausted = EditorGUILayout.Toggle("Exhausted", _scriptable._exhausted);

            if(_scriptable._condition != CardConditions.Conditions.NO_CONDITION)
            {
                _scriptable._addedPoison = EditorGUILayout.IntField("Added Poison", _scriptable._addedPoison);
            }
            else
            {
                _scriptable._addedPoison = 0;
            }
        }

        private void OnHit()
        {
            _scriptable._hitPoison = EditorGUILayout.IntField("Hit Poison", _scriptable._hitPoison);
            _scriptable._hitWeakness = EditorGUILayout.IntField("Hit Weakness", _scriptable._hitWeakness);
            _scriptable._hitDamage = EditorGUILayout.IntField("Hit Damage", _scriptable._hitDamage);
        }

        private void Adjustable()
        {
            _scriptable._burnAtZero = EditorGUILayout.Toggle("Burn At Zero", _scriptable._burnAtZero);
            _scriptable._damagePerUse = EditorGUILayout.IntField("Damage Per Use", _scriptable._damagePerUse);
            _scriptable._armorPerUse = EditorGUILayout.IntField("Armor Per Use", _scriptable._armorPerUse);
        }

        private void OnDiscard()
        {
            _scriptable._discardDamage = EditorGUILayout.IntField("Discard Damage", _scriptable._discardDamage);
            _scriptable._discardPoison = EditorGUILayout.IntField("Discard Poison", _scriptable._discardPoison);
        }

        private void SpecialEffects()
        {
            EditorGUILayout.LabelField("Special Cards", EditorStyles.boldLabel);

            _scriptable._specialType = (SpecialEffect.Special)EditorGUILayout.Popup("Special Card", (int)_scriptable._specialType, _specialNames);

            switch(_scriptable._specialType)
            {
                case SpecialEffect.Special.EXPERIENCE:
                    _scriptable._expMult = EditorGUILayout.FloatField("Experience Multiplier", _scriptable._expMult);
                    break;
                case SpecialEffect.Special.DISCARD:
                    _scriptable._discardAmount = EditorGUILayout.IntField("Discard Amount", _scriptable._discardAmount);
                    break;
                case SpecialEffect.Special.DEBUFF_PROT:
                    _scriptable._protLength = EditorGUILayout.IntField("Protection Length", _scriptable._protLength);
                    break;
                case SpecialEffect.Special.MISSING_HEALTH:
                    _scriptable._baseDamage = EditorGUILayout.IntField("Base Damage", _scriptable._baseDamage);
                    _scriptable._missingHealthMult = EditorGUILayout.FloatField("Missing Health Multiplier", _scriptable._missingHealthMult);
                    _scriptable._healPerMissingHealth = EditorGUILayout.FloatField("HealPerMissingHealth", _scriptable._healPerMissingHealth);
                    break;
                case SpecialEffect.Special.PERCENTAGE:
                    _scriptable._damageOdds = EditorGUILayout.IntField("Damage Odds", _scriptable._damageOdds);
                    _scriptable._armorOdds = EditorGUILayout.IntField("Armor Odds", _scriptable._armorOdds);
                    _scriptable._selfDamageOdds = EditorGUILayout.IntField("Self Damage Odds", _scriptable._selfDamageOdds);
                    _scriptable._damageOddsAmount = EditorGUILayout.IntField("Damage Odds Amount", _scriptable._damageOddsAmount);
                    _scriptable._armorOddsAmount = EditorGUILayout.IntField("Armor Odds Amount", _scriptable._armorOddsAmount);
                    _scriptable._selfDamageOddsAmount = EditorGUILayout.IntField("Self Damage Odds Amount", _scriptable._selfDamageOddsAmount);
                    break;
                case SpecialEffect.Special.PER_ENEMY_DEBUFF:
                    _scriptable._perDebuffType = (StatusEffectEnum)EditorGUILayout.Popup("Status Effect", (int)_scriptable._perDebuffType, _perDebuffNames);
                    _scriptable._damagePerDebuff = EditorGUILayout.IntField("Damage Per Debuff", _scriptable._damagePerDebuff);
                    break;
                case SpecialEffect.Special.PER_CARD:
                    _scriptable._cardType = (CardScriptable.CardClass)EditorGUILayout.Popup("Card Type", (int)_scriptable._cardType, _classNames);
                    _scriptable._healthPerCard = EditorGUILayout.FloatField("Health Per Card", _scriptable._healthPerCard);
                    _scriptable._armorPerCard = EditorGUILayout.FloatField("Armor Per Card", _scriptable._armorPerCard);
                    break;
                case SpecialEffect.Special.PER_ARMOR:
                    _scriptable._damagePerArmor = EditorGUILayout.FloatField("Damage Per Armor", _scriptable._damagePerArmor);
                    _scriptable._healthPerArmor = EditorGUILayout.FloatField("Health Per Armor", _scriptable._healthPerArmor);
                    break;
                case SpecialEffect.Special.PER_MANA:
                    _scriptable._armorPerMana = EditorGUILayout.FloatField("Armor Per Mana", _scriptable._armorPerMana);
                    break;
                case SpecialEffect.Special.EVERY_TURN:
                    _scriptable._permaArmor = EditorGUILayout.IntField("Permanent Armor",_scriptable._permaArmor);
                    break;
                case SpecialEffect.Special.DEBUFF_MULT:
                    _scriptable._allPoisonMult = EditorGUILayout.FloatField("All Poison Multiplier", _scriptable._allPoisonMult);
                    break;
                case SpecialEffect.Special.BUFF_MULT:
                    _scriptable._allArmorMult = EditorGUILayout.FloatField("All Armor Multiplier", _scriptable._allArmorMult);
                    break;
                case SpecialEffect.Special.REMOVE_DEBUFF:
                    break;
                case SpecialEffect.Special.TRANSFER_ARMOR:
                    break;
                case SpecialEffect.Special.BLOCK_NEXT:
                    break;
                case SpecialEffect.Special.DRAW_CARD:
                    break;
                case SpecialEffect.Special.IGNORE_ARMOR:
                    break;
                case SpecialEffect.Special.DOUBLE_ATTACK:
                    break;
                case SpecialEffect.Special.CARD_DISABLE:
                    _scriptable._cardDisables = EditorGUILayout.IntField("Card Disables", _scriptable._cardDisables);
                    break;
                case SpecialEffect.Special.ENEMY_REMAINING_HEALTH:
                    _scriptable._remainingHealthPercentage = EditorGUILayout.FloatField("Remaining Health Percentage", _scriptable._remainingHealthPercentage);
                    _scriptable._remainingHealthMinimum = EditorGUILayout.IntField("Remaining Health Minimun", _scriptable._remainingHealthMinimum);
                    break;
                case SpecialEffect.Special.REDUCE_STATUS:
                    _scriptable._reduceAmount = EditorGUILayout.IntField("Reduce Amount", _scriptable._reduceAmount);
                    break;
                case SpecialEffect.Special.ADD_CARD_TO_DECK:
                    _scriptable._addedCard = (CardScriptable)EditorGUILayout.ObjectField("Added Card", _scriptable._addedCard, typeof(CardScriptable), false);
                    break;
                case SpecialEffect.Special.REDUCE_MAX_MANA:
                    _scriptable._maxManaReduction = EditorGUILayout.IntField("Max Mana Reduction", _scriptable._maxManaReduction);
                    break;
                case SpecialEffect.Special.PERMANENT_ARMOR:
                    _scriptable._armorReduction = EditorGUILayout.FloatField("Perma Armor Rate", _scriptable._armorReduction);
                    break;
                case SpecialEffect.Special.EXHAUST_RANDOM:
                    _scriptable._exhaustAmount = EditorGUILayout.IntField("Exhaust Amount", _scriptable._exhaustAmount);
                    break;
                case SpecialEffect.Special.PER_DISCARD:
                    _scriptable._weaknessPerDiscard = EditorGUILayout.IntField("Weakness Per Discard", _scriptable._weaknessPerDiscard);
                    _scriptable._healthPerDiscard = EditorGUILayout.IntField("Health Per Discard", _scriptable._healthPerDiscard);
                    break;
                case SpecialEffect.Special.PER_CARD_USED:
                    _scriptable._poisonPerCardUsed = EditorGUILayout.IntField("Poison Per Card Used", _scriptable._poisonPerCardUsed);
                    _scriptable._damageTakenPerCardUsed = EditorGUILayout.IntField("Damage Taken Per Card Used", _scriptable._damageTakenPerCardUsed);
                    break;
                case SpecialEffect.Special.REDUCE_MANA:
                    _scriptable._manaReduction = EditorGUILayout.IntField("Mana Reduction", _scriptable._manaReduction);
                    break;
                case SpecialEffect.Special.POISON_HIT:
                    _scriptable._poisonPerHit = EditorGUILayout.IntField("Poison Per Hit", _scriptable._poisonPerHit);
                    break;
                case SpecialEffect.Special.DEBUFF_PER_TURN:
                    _scriptable._debuffPerTurnType = (StatusEffectEnum)EditorGUILayout.Popup("Debuff Type", (int)_scriptable._debuffPerTurnType, _perDebuffNames);
                    _scriptable._debuffPerTurn = EditorGUILayout.IntField("Debuff Per Turn", _scriptable._debuffPerTurn);
                    break;
                case SpecialEffect.Special.NONE:
                default:
                    break;
            }
        }
    
        private void UpdateDescription()
        {
            string cardDesc = _scriptable._cardDescUneditted;

            cardDesc = cardDesc.Replace("<TargetDamage>", _scriptable._targetDamage.ToString());
            cardDesc = cardDesc.Replace("<AllDamage>", _scriptable._allDamage.ToString());
            cardDesc = cardDesc.Replace("<DamageInstances>", _scriptable._damageInstances.ToString());
            cardDesc = cardDesc.Replace("<DamageMultiplier>", _scriptable._damageMultiplier.ToString());
            cardDesc = cardDesc.Replace("<BonusDamage>", _scriptable._bonusDamage.ToString());

            cardDesc = cardDesc.Replace("<ArmorAmount>", _scriptable._armor.ToString());
            cardDesc = cardDesc.Replace("<DamageReduction>", _scriptable._damageReduction.ToString());
            cardDesc = cardDesc.Replace("<ArmorMultiplier>", _scriptable._armorMultiplier.ToString());

            cardDesc = cardDesc.Replace("<ManaRestore>", _scriptable._manaRestore.ToString());
            cardDesc = cardDesc.Replace("<ManaMultiplier>", _scriptable._manaMultiplier.ToString());

            cardDesc = cardDesc.Replace("<SelfDamage>", _scriptable._selfDamage.ToString());
            cardDesc = cardDesc.Replace("<SelfPoison>", _scriptable._selfPoison.ToString());
            cardDesc = cardDesc.Replace("<SelfWeakness>", _scriptable._selfWeakness.ToString());

            cardDesc = cardDesc.Replace("<HealingAmount>", _scriptable._healingAmount.ToString());
            cardDesc = cardDesc.Replace("<AddedMaxHealth>", _scriptable._addedMaxHealth.ToString());

            cardDesc = cardDesc.Replace("<Weakness>", _scriptable._weakness.ToString());
            cardDesc = cardDesc.Replace("<Poison>", _scriptable._poison.ToString());
            cardDesc = cardDesc.Replace("<Defense>", _scriptable._defense.ToString());
            cardDesc = cardDesc.Replace("<Rage>", _scriptable._rage.ToString());
            cardDesc = cardDesc.Replace("<AddedPoison>", _scriptable._addedPoison.ToString());

            cardDesc = cardDesc.Replace("<HitPoison>", _scriptable._hitPoison.ToString());
            cardDesc = cardDesc.Replace("<HitWeakness>", _scriptable._hitWeakness.ToString());
            cardDesc = cardDesc.Replace("<HitDamage>", _scriptable._hitDamage.ToString());

            cardDesc = cardDesc.Replace("<DamagePerUse>", Mathf.Abs(_scriptable._damagePerUse).ToString());
            cardDesc = cardDesc.Replace("<ArmorPerUse>", Mathf.Abs(_scriptable._armorPerUse).ToString());

            cardDesc = cardDesc.Replace("<DiscardDamage>", _scriptable._discardDamage.ToString());
            cardDesc = cardDesc.Replace("<DiscardPoison>", _scriptable._discardPoison.ToString());

            switch(_scriptable._specialType)
            {
                case SpecialEffect.Special.EXPERIENCE:
                    cardDesc = cardDesc.Replace("<ExperienceMultiplier>", _scriptable._expMult.ToString());
                    break;
                case SpecialEffect.Special.DISCARD:
                    cardDesc = cardDesc.Replace("<DiscardAmount>", _scriptable._discardAmount.ToString());
                    break;
                case SpecialEffect.Special.DEBUFF_PROT:
                    cardDesc = cardDesc.Replace("<ProtectionLength>", _scriptable._protLength.ToString());
                    break;
                case SpecialEffect.Special.MISSING_HEALTH:
                    cardDesc = cardDesc.Replace("<BaseDamage>", _scriptable._baseDamage.ToString());
                    cardDesc = cardDesc.Replace("<MissingHealthMultiplier>", (_scriptable._missingHealthMult * 100f).ToString());
                    cardDesc = cardDesc.Replace("<HealPerMissingHealth>", (_scriptable._healPerMissingHealth * 100f).ToString());
                    break;
                case SpecialEffect.Special.PERCENTAGE:
                    cardDesc = cardDesc.Replace("<DamageOddsAmount>", _scriptable._damageOddsAmount.ToString());
                    cardDesc = cardDesc.Replace("<ArmorOddsAmount>", _scriptable._armorOddsAmount.ToString());
                    cardDesc = cardDesc.Replace("<SelfDamageOddsAmount>", _scriptable._selfDamageOddsAmount.ToString());
                    break;
                case SpecialEffect.Special.PER_ENEMY_DEBUFF:
                    cardDesc = cardDesc.Replace("<DamagePerDebuff>", _scriptable._damagePerDebuff.ToString());
                    break;
                case SpecialEffect.Special.PER_CARD:
                    cardDesc = cardDesc.Replace("<HealthPerCard>", _scriptable._healthPerCard.ToString());
                    cardDesc = cardDesc.Replace("<ArmorPerCard>", _scriptable._armorPerCard.ToString());
                    break;
                case SpecialEffect.Special.PER_ARMOR:
                    cardDesc = cardDesc.Replace("<DamagePerArmor>", _scriptable._damagePerArmor.ToString());
                    cardDesc = cardDesc.Replace("<HealthPerArmor>", _scriptable._healthPerArmor.ToString());
                    break;
                case SpecialEffect.Special.PER_MANA:
                    cardDesc = cardDesc.Replace("<ArmorPerMana>", _scriptable._armorPerMana.ToString());
                    break;
                case SpecialEffect.Special.EVERY_TURN:
                    cardDesc = cardDesc.Replace("<PermanentArmor>", _scriptable._permaArmor.ToString());
                    break;
                case SpecialEffect.Special.DEBUFF_MULT:
                    cardDesc = cardDesc.Replace("<AllPoisonMultiplier>", _scriptable._allArmorMult.ToString());
                    break;
                case SpecialEffect.Special.BUFF_MULT:
                    cardDesc = cardDesc.Replace("<AllArmorMultiplier>", _scriptable._allArmorMult.ToString());
                    break;
                case SpecialEffect.Special.REMOVE_DEBUFF:
                    break;
                case SpecialEffect.Special.TRANSFER_ARMOR:
                    break;
                case SpecialEffect.Special.BLOCK_NEXT:
                    break;
                case SpecialEffect.Special.DRAW_CARD:
                    break;
                case SpecialEffect.Special.IGNORE_ARMOR:
                    break;
                case SpecialEffect.Special.DOUBLE_ATTACK:
                    break;
                case SpecialEffect.Special.CARD_DISABLE:
                    cardDesc = cardDesc.Replace("<CardDisables>", _scriptable._cardDisables.ToString());
                    break;
                case SpecialEffect.Special.ENEMY_REMAINING_HEALTH:
                    cardDesc = cardDesc.Replace("<RemainingHealthPercentage>", (_scriptable._remainingHealthPercentage * 100f).ToString());
                    cardDesc = cardDesc.Replace("<RemainingHealthMinimum>", _scriptable._remainingHealthMinimum.ToString());
                    break;
                case SpecialEffect.Special.REDUCE_STATUS:
                    cardDesc = cardDesc.Replace("<ReduceAmount>", _scriptable._reduceAmount.ToString());
                    break;
                case SpecialEffect.Special.REDUCE_MAX_MANA:
                    cardDesc = cardDesc.Replace("<MaxManaReduction>", _scriptable._maxManaReduction.ToString());
                    break;
                case SpecialEffect.Special.PERMANENT_ARMOR:
                    cardDesc = cardDesc.Replace("<PermaArmorRate>", _scriptable._armorReduction.ToString());
                    break;
                case SpecialEffect.Special.EXHAUST_RANDOM:
                    cardDesc = cardDesc.Replace("<ExhaustAmount>", _scriptable._exhaustAmount.ToString());
                    break;
                case SpecialEffect.Special.PER_DISCARD:
                    cardDesc = cardDesc.Replace("<WeaknessPerDiscard>", _scriptable._weaknessPerDiscard.ToString());
                    cardDesc = cardDesc.Replace("<HealthPerDiscard>", _scriptable._healthPerDiscard.ToString());
                    break;
                case SpecialEffect.Special.PER_CARD_USED:
                    cardDesc = cardDesc.Replace("<PoisonPerCardUsed>", _scriptable._poisonPerCardUsed.ToString());
                    cardDesc = cardDesc.Replace("<DamageTakenPerCardUsed>", _scriptable._damageTakenPerCardUsed.ToString());
                    break;
                case SpecialEffect.Special.REDUCE_MANA:
                    cardDesc = cardDesc.Replace("<ManaReduction>", _scriptable._manaReduction.ToString());
                    break;
                case SpecialEffect.Special.POISON_HIT:
                    cardDesc = cardDesc.Replace("<PoisonPerHit>", _scriptable._poisonPerHit.ToString());
                    break;
                case SpecialEffect.Special.DEBUFF_PER_TURN:
                    cardDesc = cardDesc.Replace("<DebuffPerTurn>", _scriptable._debuffPerTurn.ToString());
                    break;
                case SpecialEffect.Special.NONE:
                default:
                    break;
            }

            _scriptable._cardDesc = cardDesc;
        }
    }
}
