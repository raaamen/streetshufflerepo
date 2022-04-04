using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using System;

namespace StreetPerformers
{
    /// <summary>
    /// Handles basic functionality of card effects (dealing damage, healing, using mana, etc).
    /// </summary>
    public class Card : MonoBehaviour
    {
        [Header("Icons")]
        public Sprite _attackImage;
        public Sprite _defenseImage;
        public Sprite _supportImage;

        [Header("Backgrounds")]
        [SerializeField]
        private Sprite _attackBackground;
        [SerializeField]
        private Sprite _defenseBackground;
        [SerializeField]
        private Sprite _supportBackground;

        [Header("Suits")]
        [SerializeField]
        private Sprite _spades;
        [SerializeField]
        private Sprite _clubs;
        [SerializeField]
        private Sprite _diamonds;
        [SerializeField]
        private Sprite _hearts;

        [Header("Backsides")]
        [SerializeField]
        private Sprite _aceBack;
        [SerializeField]
        private Sprite _contortionistBack;
        [SerializeField]
        private Sprite _mascotBack;
        [SerializeField]
        private Sprite _mimeBack;

        [SerializeField] private GameObject _onUsePoison = null;
        [SerializeField] private TextMeshProUGUI _onUsePoisonText = null;
        [SerializeField] private GameObject _onUseDamage = null;
        [SerializeField] private TextMeshProUGUI _onUseDamageText = null;
        [SerializeField] private GameObject _onUseWeakness = null;
        [SerializeField] private TextMeshProUGUI _onUseWeaknessText = null;

        [SerializeField] private GameObject _burnImage = null;
        [SerializeField] private GameObject _doubleUse = null;

        public TextMeshProUGUI _nameText = null;
        public TextMeshProUGUI _manaCostText = null;
        public TextMeshProUGUI _descriptionText = null;
        public GameObject _classObject = null;
        public Image _classImage = null;
        public Image _cardImage = null;
        public Image _backsideImage = null;
        public Image _backgroundImage = null;
        public RectTransform _burningTransform = null;
        public RectMask2D _cardMask = null;

        //Scriptable object that holds the data of the card
        public CardScriptable _scriptable
        {
            get;
            protected set;
        }

        //GameObject of the agent that activated the card
        protected GameObject _user;
        protected List<GameObject> _allies;
        //GameObject of the target of the card
        protected List<GameObject> _targets;
        protected GameObject _singleTarget;

        private bool _tempBurn = false;

        private bool _activated = false;

        private bool _blinded = false;

        private Dictionary<CharacterStats, CharacterStatChangeData> _projectedChangePerTarget = new Dictionary<CharacterStats, CharacterStatChangeData>();

        private PartyTurn _partyBlockCharacter = null;

        private void Start()
        {
            if(_targets == null)
            {
                _targets = new List<GameObject>();
                _allies = new List<GameObject>();
            }
        }

        public void InitializeProjectionTargets()
        {
            _projectedChangePerTarget.Clear();
            _projectedChangePerTarget[_user.GetComponent<CharacterStats>()] = new CharacterStatChangeData();

            if(_scriptable._canTargetAllAllies)
            {
                foreach(GameObject ally in _allies)
                {
                    _projectedChangePerTarget[ally.GetComponent<CharacterStats>()] = new CharacterStatChangeData();
                }
            }

            if(_scriptable._hitSingleEnemy)
            {
                _projectedChangePerTarget[_singleTarget.GetComponent<CharacterStats>()] = new CharacterStatChangeData();
            }

            if(_scriptable._hitAllEnemies)
            {
                foreach(GameObject target in _targets)
                {
                    if(_scriptable._hitSingleEnemy && target == _singleTarget)
                    {
                        continue;
                    }

                    _projectedChangePerTarget[target.GetComponent<CharacterStats>()] = new CharacterStatChangeData();
                }
            }

           UpdateOnUseVisual();
        }

        public void UpdateOnUseVisual(bool hasTarget = false, int damage = 0, int poison = 0, int weakness = 0)
        {
            CombatEffectManager combatEffectManager = _user.GetComponent<CombatEffectManager>();
            bool showDebuffs = !combatEffectManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION) && _scriptable._specialType != SpecialEffect.Special.DEBUFF_PROT;

            if(combatEffectManager.GetCombatEffect(CombatEffectEnum.BLIND))
            {
                return;
            }

            int damageValue;
            int poisonValue;
            int weaknessValue = 0;

            if(hasTarget)
            {
                damageValue = damage;
                poisonValue = poison;
                weaknessValue = weakness;
            }
            else
            {
                CharacterTurn charTurn = _user.GetComponent<CharacterTurn>();

                damageValue = charTurn._damagePerCardUse;
                poisonValue = charTurn._poisonPerCardUse;
            }

            if(poisonValue > 0 && showDebuffs)
            {
                _onUsePoison.SetActive(true);

                _onUsePoisonText.text = poisonValue.ToString();
            }
            else
            {
                _onUsePoison.SetActive(false);
            }

            if(weaknessValue > 0 && showDebuffs)
            {
                _onUseWeakness.SetActive(true);

                _onUseWeaknessText.text = weaknessValue.ToString();
            }
            else
            {
                _onUseWeakness.SetActive(false);
            }

            if(damageValue > 0)
            {
                _onUseDamage.SetActive(true);

                _onUseDamageText.text = damageValue.ToString();
            }
            else
            {
                _onUseDamage.SetActive(false);
            }
        }

        public void DisableOnUseVisual()
        {
            _onUsePoison.SetActive(false);
            _onUseDamage.SetActive(false);
        }

        public void UpdateDoubleUse()
        {
            if(_user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.BLIND))
            {
                return;
            }

            if(_user.GetComponent<CharacterTurn>()._doubleAttack && _scriptable._class == CardScriptable.CardClass.ATTACK)
            {
                _doubleUse.SetActive(true);
            }
            else
            {
                _doubleUse.SetActive(false);
            }
        }

        public void DisableDoubleUse()
        {
            _doubleUse.SetActive(false);
        }

        public void ProjectedActivate()
        {
            bool doubleActive = _user.GetComponent<CharacterTurn>()._doubleAttack && _scriptable._class == CardScriptable.CardClass.ATTACK;

            int iteration = 1;
            if(doubleActive)
            {
                iteration = 2;
            }

            foreach(CharacterStats target in _projectedChangePerTarget.Keys)
            {
                _projectedChangePerTarget[target].Reset();
            }

            for(int i = 0; i < iteration; i++)
            {
                if(_scriptable._givesArmor)
                {
                    AddArmor(true);
                }

                if(_scriptable._dealsDamage)
                {
                    DealDamage(true);
                }

                if(_scriptable._restoresMana)
                {
                    AddMana(true);
                }

                if(_scriptable._dealsSelfDamage)
                {
                    SelfDamage(true);
                }

                if(_scriptable._heals)
                {
                    Healing(true);
                }

                if(_scriptable._appliesEffect)
                {
                    StatusEffects(true);
                }

                if(_scriptable._effectOnHit)
                {
                    OnHit(true);
                }

                if(_scriptable._hitSingleEnemy)
                {
                    List<GameObject> singleTargetList = new List<GameObject>();
                    singleTargetList.Add(_singleTarget);
                    SpecialEffect.ApplySpecial(_scriptable, _user, singleTargetList, ref _projectedChangePerTarget, checkProjection: true);
                }
                else
                {
                    SpecialEffect.ApplySpecial(_scriptable, _user, _targets, ref _projectedChangePerTarget, checkProjection: true);
                }

                CharacterTurn charTurn = _user.GetComponent<CharacterTurn>();
                CharacterStats charStats = _user.GetComponent<CharacterStats>();
                if(charTurn._poisonPerCardUse > 0 && !_user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION) && _scriptable._specialType != SpecialEffect.Special.DEBUFF_PROT)
                {
                    _projectedChangePerTarget[charStats]._statChange[StatusEffectEnum.POISON] += charTurn._poisonPerCardUse;
                    _projectedChangePerTarget[charStats]._hitPoisonReceived += charTurn._poisonPerCardUse;
                }
                if(charTurn._damagePerCardUse > 0)
                {
                    _projectedChangePerTarget[charStats]._damageChange += charTurn._damagePerCardUse;
                    _projectedChangePerTarget[charStats]._hitDamageReceived += charTurn._damagePerCardUse;
                }
            }

            UseMana(true);

#if UNITY_EDITOR
            string debugString = "";
            CharacterStats userStats = _user.GetComponent<CharacterStats>();
            debugString += $"{_user.GetComponent<CharacterTurn>()._characterName}\n";
            debugString += $"{_projectedChangePerTarget[userStats].ToString()}\n";

            if(_scriptable._hitSingleEnemy)
            {
                CharacterStats charStats = _singleTarget.GetComponent<CharacterStats>();
                debugString += $"{_singleTarget.GetComponent<CharacterTurn>()._characterName}\n";
                debugString += $"{_projectedChangePerTarget[charStats].ToString()}\n";
            }

            if(_scriptable._hitAllEnemies)
            {
                foreach(GameObject target in _targets)
                {
                    if(_scriptable._hitSingleEnemy && target == _singleTarget)
                    {
                        continue;
                    }

                    CharacterStats charStats = target.GetComponent<CharacterStats>();
                    debugString += $"{target.GetComponent<CharacterTurn>()._characterName}\n";
                    debugString += $"{_projectedChangePerTarget[charStats].ToString()}\n";
                }
            }
            Debug.Log(debugString);
#endif
        }

        public void ShowProjection()
        {
            if(_user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.BLIND))
            {
                return;
            }

            foreach(CharacterStats charStats in _projectedChangePerTarget.Keys)
            {
                CharacterStatChangeData statChange = _projectedChangePerTarget[charStats];
                StatusEffectManager statusManager = charStats.GetComponent<StatusEffectManager>();
                CharacterTurn charTurn = charStats.GetComponent<CharacterTurn>();
                ArmorManager armorManager = charStats.GetComponent<ArmorManager>();
                CombatEffectManager combatEffectManager = charStats.GetComponent<CombatEffectManager>();

                int armorChange = statChange._armorChange;
                if(armorChange != 0)
                {
                    armorManager.ShowProjectionView(armorChange);
                }
                int postArmorChange = armorManager._armor + armorChange;

                int damageChange = statChange._damageChange;

                if(combatEffectManager.GetCombatEffect(CombatEffectEnum.BLOCK_ALL) || combatEffectManager.GetCombatEffect(CombatEffectEnum.BLOCK_NEXT))
                {
                    damageChange = 0;
                }

                if(damageChange != 0)
                {
                    CombatEffectManager userCombatManager = _user.GetComponent<CombatEffectManager>();
                    //if ignores armor, healing/damage are treated the same
                    if(userCombatManager.GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR) && _user.GetComponent<CharacterStats>() != charStats)
                    {
                        damageChange -= statChange._healChange;
                        if(damageChange < 0)
                        {
                            charStats.AddHealthProjectionView(damageChange);
                        }
                        else if(damageChange > 0)
                        {
                            charStats.TakeDamageProjectionView(damageChange);
                        }
                    }
                    else
                    {

                        //if damage is less than armor, deal armor damage and heal
                        if(postArmorChange >= damageChange)
                        {
                            armorManager.ShowProjectionView(armorChange - damageChange);
                            charStats.AddHealthProjectionView(statChange._healChange);
                        }
                        //else remove armor, then take the difference between damage and healing
                        else
                        {
                            if(postArmorChange > 0)
                            {
                                damageChange -= postArmorChange;
                                armorManager.ShowProjectionView(-armorManager._armor);
                            }

                            damageChange -= statChange._healChange;
                            if(damageChange < 0)
                            {
                                charStats.AddHealthProjectionView(damageChange);
                            }
                            else if(damageChange > 0)
                            {
                                charStats.TakeDamageProjectionView(damageChange);
                            }
                        }
                    }
                }
                else if(statChange._healChange != 0)
                {
                    charStats.AddHealthProjectionView(statChange._healChange);
                }

                if(statChange._maxHealthChange != 0)
                {
                    charStats.MaxHealthProjectionView(statChange._maxHealthChange);
                }

                if(statChange._manaChange != 0)
                {
                    charStats.ManaProjectionView(statChange._manaChange);
                }

                if(statChange._maxManaChange != 0)
                {
                    charStats.MaxManaProjectionView(statChange._maxManaChange);
                }

                int fortify = statChange._statChange[StatusEffectEnum.FORTIFY] + statChange._temporaryFortify;
                if(fortify != 0)
                {
                    statusManager.ShowProjectionView(StatusEffectEnum.FORTIFY, fortify);
                }

                bool debuffProt = combatEffectManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION) || (_user == charStats.gameObject && _scriptable._specialType == SpecialEffect.Special.DEBUFF_PROT);
                int poison = statChange._statChange[StatusEffectEnum.POISON];
                if(poison > 0 && debuffProt)
                {
                    poison = 0;
                }
                if(poison != 0)
                {
                    statusManager.ShowProjectionView(StatusEffectEnum.POISON, poison);
                }

                int rage = statChange._statChange[StatusEffectEnum.RAGE];
                if(rage != 0)
                {
                    statusManager.ShowProjectionView(StatusEffectEnum.RAGE, rage);
                }

                int vulnerable = statChange._statChange[StatusEffectEnum.VULNERABLE];
                if(vulnerable > 0 && debuffProt)
                {
                    vulnerable = 0;
                }
                if(vulnerable != 0)
                {
                    statusManager.ShowProjectionView(StatusEffectEnum.VULNERABLE, vulnerable);
                }

                foreach(PartyTurn partyTurn in statChange._exhaustedAllies)
                {
                    partyTurn.ExhaustionProjectionView();
                }

                /* Currently don't have plans to add something for this but TODO
                if(statChange._hitDamageChange > 0)
                {
                    //charStats.AddHitDamage(statChange._hitDamageChange);
                }

                if(statChange._hitPoisonChange > 0)
                {
                    //charStats.AddHitPoison(statChange._hitPoisonChange);
                }

                if(statChange._hitWeaknessChange > 0)
                {
                    //charStats.AddHitWeakness(statChange._hitWeaknessChange);
                }*/

                //probably don't need to add to projection viewight 
                /*if(statChange._vulnerablePerDiscard > 0)
                {
                    //charTurn.AddVulnerablePerDiscard(statChange._vulnerablePerDiscard);
                }

                if(statChange._healthPerDiscard > 0)
                {
                    //charStats.AddHealthPerDiscard(statChange._healthPerDiscard);
                }

                if(statChange._poisonPerCardUsed > 0)
                {
                    //charTurn.AddPoisonPerCardUsed(statChange._poisonPerCardUsed);
                }

                if(statChange._damagePerCardUsed > 0)
                {
                    //charTurn.AddDamageTakenPerCardUsed(statChange._damagePerCardUsed);
                }*/
            }

            CharacterStatChangeData userStatChange = _projectedChangePerTarget[_user.GetComponent<CharacterStats>()];
            UpdateOnUseVisual(true, userStatChange._hitDamageReceived, userStatChange._hitPoisonReceived, userStatChange._hitWeaknessReceived);

            UpdateBurnImage();
        }

        public void UpdateBurnImage()
        {
            CombatEffectManager combatEffManager = _user.GetComponent<CombatEffectManager>();

            if(combatEffManager.GetCombatEffect(CombatEffectEnum.BLIND))
            {
                return;
            }

            if(combatEffManager != null)
            {
                if(combatEffManager.GetCombatEffect(CombatEffectEnum.BURN_NEXT) || combatEffManager.GetCombatEffect(CombatEffectEnum.BURN_ALL) || _scriptable._burned)
                {
                    _burnImage.SetActive(true);
                }
                else if(_scriptable._adjustable)
                {
                    if(_scriptable._damagePerUse != 0)
                    {
                        int postUseDamage = _scriptable._accumulatedDamage + _scriptable._damagePerUse;
                        if(postUseDamage < 0 && Mathf.Abs(postUseDamage) >= Mathf.Abs(_scriptable._targetDamage))
                        {
                            _burnImage.SetActive(true);
                        }
                    }

                    if(_scriptable._armorPerUse != 0)
                    {
                        int postUseArmor = _scriptable._accumulatedArmor + _scriptable._armorPerUse;
                        if(postUseArmor < 0 && Mathf.Abs(postUseArmor) >= Mathf.Abs(_scriptable._armor))
                        {
                            _burnImage.SetActive(true);
                        }
                    }
                }
            }
        }

        public bool WillBurn()
        {
            return _burnImage.activeInHierarchy;
        }

        public void RemoveProjectionView(bool cardPlayed)
        {
            if(!cardPlayed)
            {
                _burnImage.SetActive(false);
            }

            UpdateOnUseVisual();
        }

        /// <summary>
        /// Called when the card is played onto the field. Will deal damage, add armor, or any
        /// other effect of the card.
        /// </summary>
        public void Activate(bool doubleActivate, bool burnNext, bool teamAnimation = true)
        {
            _activated = true;

            CardSoundEffect();

            foreach(CharacterStats charStats in _projectedChangePerTarget.Keys)
            {
                CharacterStatChangeData statChange = _projectedChangePerTarget[charStats];
                StatusEffectManager statusManager = charStats.GetComponent<StatusEffectManager>();
                CharacterTurn charTurn = charStats.GetComponent<CharacterTurn>();
                ArmorManager armorManager = charStats.GetComponent<ArmorManager>();
                CombatEffectManager combatEffectManager = charStats.GetComponent<CombatEffectManager>();

                if(_partyBlockCharacter != null && charTurn._characterName.Equals("Ace"))
                {
                    _partyBlockCharacter.PermaExhaust();
                    _partyBlockCharacter = null;
                    continue;
                }

                int armorChange = statChange._armorChange + statChange._hiddenArmorChange;
                if(armorChange != 0)
                {
                    armorManager.Add(armorChange);
                }

                if(statChange._transferArmor > 0)
                {
                    armorManager.AddTransferArmor(statChange._transferArmor);
                }

                int damageChange = statChange._damageChange + statChange._hiddenArmorChange;
                if(combatEffectManager.GetCombatEffect(CombatEffectEnum.BLOCK_ALL) || combatEffectManager.GetCombatEffect(CombatEffectEnum.BLOCK_NEXT))
                {
                    combatEffectManager.SetCombatEffect(CombatEffectEnum.BLOCK_NEXT, false);
                    damageChange = 0;
                }

                if(damageChange != 0)
                {
                    CombatEffectManager userCombatManager = _user.GetComponent<CombatEffectManager>();
                    //if ignores armor, healing/damage are treated the same
                    if(userCombatManager.GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR))
                    {
                        damageChange -= statChange._healChange;

                        if(damageChange < 0)
                        {
                            charStats.AddHealth(damageChange);
                        }
                        else if(damageChange > 0)
                        {
                            charStats.TakeDamage(damageChange, _user, true, true, true);
                        }
                    }
                    else
                    {

                        //if damage is less than armor, deal armor damage and heal
                        if(armorManager._armor >= damageChange)
                        {
                            armorManager.Subtract(damageChange);
                            charStats.AddHealth(statChange._healChange);
                        }
                        //else remove armor, then take the difference between damage and healing
                        else
                        {
                            if(armorManager._armor > 0)
                            {
                                damageChange -= armorManager._armor;
                                armorManager.Subtract(armorManager._armor);
                            }

                            damageChange -= statChange._healChange;
                            if(damageChange < 0)
                            {
                                charStats.AddHealth(damageChange);
                            }
                            else if(damageChange > 0)
                            {
                                charStats.TakeDamage(damageChange, _user, true, true, true);
                            }
                        }
                    }
                }
                else
                {
                    charStats.AddHealth(statChange._healChange);
                }

                if(statChange._maxHealthChange != 0)
                {
                    charStats.AddMaxHealth(statChange._maxHealthChange, false);
                    PartyHandler.Instance._addedMaxHealth += statChange._maxHealthChange;
                }

                if(statChange._manaChange > 0)
                {
                    charStats.AddMana(statChange._manaChange);
                }
                else if(statChange._manaChange < 0)
                {
                    charStats.UseMana(Mathf.Abs(statChange._manaChange));
                }

                if(statChange._maxManaChange > 0)
                {
                    charStats.GetComponent<PlayerStats>().UpgradeMana(statChange._maxManaChange);
                }
                else if(statChange._maxManaChange < 0)
                {
                    charStats.GetComponent<PlayerStats>().ReduceMaxMana(Mathf.Abs(statChange._maxManaChange));
                }

                int fortify = statChange._statChange[StatusEffectEnum.FORTIFY] + statChange._hiddenStatChange[StatusEffectEnum.FORTIFY];
                if(fortify > 0)
                {
                    statusManager.AddStatusEffect(StatusEffectEnum.FORTIFY, fortify);
                }
                else if(fortify < 0)
                {
                    statusManager.SubtractStatusEffect(StatusEffectEnum.FORTIFY, Mathf.Abs(fortify));
                }

                if(statChange._temporaryFortify > 0)
                {
                    statusManager.AddTemporary(StatusEffectEnum.FORTIFY, statChange._temporaryFortify);
                }

                int poison = statChange._statChange[StatusEffectEnum.POISON] + statChange._hiddenStatChange[StatusEffectEnum.POISON];
                if(poison > 0)
                {
                    statusManager.AddStatusEffect(StatusEffectEnum.POISON, poison);
                }
                else if(poison < 0)
                {
                    statusManager.SubtractStatusEffect(StatusEffectEnum.POISON, Mathf.Abs(poison));
                }

                int rage = statChange._statChange[StatusEffectEnum.RAGE] + statChange._hiddenStatChange[StatusEffectEnum.RAGE];
                if(rage > 0)
                {
                    statusManager.AddStatusEffect(StatusEffectEnum.RAGE, rage);
                }
                else if(rage < 0)
                {
                    statusManager.SubtractStatusEffect(StatusEffectEnum.RAGE, Mathf.Abs(rage));
                }

                int vulnerable = statChange._statChange[StatusEffectEnum.VULNERABLE] + statChange._hiddenStatChange[StatusEffectEnum.VULNERABLE];
                if(vulnerable > 0)
                {
                    statusManager.AddStatusEffect(StatusEffectEnum.VULNERABLE, vulnerable);
                }
                else if(vulnerable < 0)
                {
                    statusManager.SubtractStatusEffect(StatusEffectEnum.VULNERABLE, Mathf.Abs(vulnerable));
                }

                foreach(PartyTurn partyTurn in statChange._exhaustedAllies)
                {
                    partyTurn.Exhaust();
                }

                if(statChange._hitDamageChange > 0)
                {
                    charStats.AddHitDamage(statChange._hitDamageChange);
                }

                if(statChange._hitPoisonChange > 0)
                {
                    charStats.AddHitPoison(statChange._hitPoisonChange);
                }

                if(statChange._hitWeaknessChange > 0)
                {
                    charStats.AddHitWeakness(statChange._hitWeaknessChange);
                }

                if(statChange._vulnerablePerDiscard > 0)
                {
                    charTurn.AddVulnerablePerDiscard(statChange._vulnerablePerDiscard);
                }
                
                if(statChange._healthPerDiscard > 0)
                {
                    charStats.AddHealthPerDiscard(statChange._healthPerDiscard);
                }

                if(statChange._poisonPerCardUsed > 0)
                {
                    charTurn.AddPoisonPerCardUsed(statChange._poisonPerCardUsed);
                }

                if(statChange._damagePerCardUsed > 0)
                {
                    charTurn.AddDamageTakenPerCardUsed(statChange._damagePerCardUsed);
                }
            }

            if(_scriptable._adjustable)
            {
                Adjustable();
                if(doubleActivate)
                {
                    Adjustable();
                }
            }

            if(_scriptable._hitSingleEnemy)
            {
                List<GameObject> singleTargetList = new List<GameObject>();
                singleTargetList.Add(_singleTarget);
                SpecialEffect.ApplySpecial(_scriptable, _user, singleTargetList, ref _projectedChangePerTarget, doubleActivate: doubleActivate);
            }
            else
            {
                SpecialEffect.ApplySpecial(_scriptable, _user, _targets, ref _projectedChangePerTarget, doubleActivate: doubleActivate);
            }

            if(teamAnimation)
            {
                UseAnimation();
            }

           /* if(doubleActivate)
            {
                Activate(false, false);
                return;
            }*/
            
            if(burnNext || _tempBurn || _scriptable._burned)
            {
                CardBurn cardBurn = this.gameObject.AddComponent<CardBurn>();
                cardBurn.Initialize(_cardMask, _burningTransform);

                Burn();

                Destroy(GetComponent<DraggableCard>());

                DiscardCard(false, _user.tag);
            }
            else
            {
                DiscardCard(false, _user.tag);
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Returns true if activating this card will kill the user
        /// </summary>
        /// <returns></returns>
        public PartyTurn CheckPartyBlock()
        {
            foreach(CharacterStats charStats in _projectedChangePerTarget.Keys)
            {
                PlayerTurn playerTurn = charStats.GetComponent<PlayerTurn>();

                int damageChange = _projectedChangePerTarget[charStats]._damageChange + _projectedChangePerTarget[charStats]._hiddenDamageChange;
                if(playerTurn != null && damageChange > 0)
                {
                    CombatEffectManager combatEffectManager = charStats.GetComponent<CombatEffectManager>();
                    if(combatEffectManager.GetCombatEffect(CombatEffectEnum.BLOCK_ALL) || combatEffectManager.GetCombatEffect(CombatEffectEnum.BURN_NEXT))
                    {
                        return null;
                    }

                    ArmorManager armorManager = charStats.GetComponent<ArmorManager>();

                    if(damageChange > armorManager._armor + charStats._health)
                    {
                        return playerTurn.CanPartyBlock();
                    }

                    if(_user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR) && damageChange > charStats._health)
                    {
                        return playerTurn.CanPartyBlock();
                    }
                }
            }

            return null;
        }

        public void SetPartyBlock(PartyTurn partyBlockCharacter)
        {
            _partyBlockCharacter = partyBlockCharacter;
        }

        /// <summary>
        /// Uses mana based off the mana cost of the card.
        /// </summary>
        private void UseMana(bool checkProjection = false)
        {
            int manaCost = 0;
            switch(_scriptable._manaType)
            {
                case CardScriptable.ManaType.VALUE:
                    manaCost = _scriptable._manaCost;
                    break;
                case CardScriptable.ManaType.MAX:
                    if (_user.GetComponent<PlayerStats>())
                    {
                        manaCost = _user.GetComponent<PlayerStats>()._maxMana;
                    }
                    break;
                case CardScriptable.ManaType.X:
                    if (_user.GetComponent<PlayerStats>())
                    {
                        manaCost = _user.GetComponent<PlayerStats>()._mana;
                    }
                    break;
                case CardScriptable.ManaType.NONE:
                default:
                    break;
            }

            CharacterStats userStats = _user.GetComponent<CharacterStats>();
            if(checkProjection)
            {
                _projectedChangePerTarget[userStats]._manaChange -= manaCost;
            }
            else
            {
                userStats.UseMana(manaCost);
            }
        }

        /// <summary>
        /// Deals basic instances of damage.
        /// </summary>
        private void DealDamage(bool checkProjection = false)
        {
            bool ignoreArmor = _user.GetComponent<CombatEffectManager>().GetCombatEffect(CombatEffectEnum.IGNORE_ARMOR);
            float rage = _user.GetComponent<StatusEffectManager>().GetStatusEffect(StatusEffectEnum.RAGE);

            if(_scriptable._hitSingleEnemy)
            {
                float targetDamage = _scriptable._targetDamage;
                CardConditions.Conditions cond = _scriptable._condition;
                if(cond != CardConditions.Conditions.NO_CONDITION && CardConditions.Evaluate(cond, _user, _singleTarget, _scriptable))
                {
                    targetDamage *= _scriptable._damageMultiplier;
                    targetDamage += _scriptable._bonusDamage;
                }
                if(_scriptable._attackType == CardScriptable.AttackType.SINGLE)
                {
                    _scriptable._damageInstances = 1;
                }
                else if(_scriptable._attackType == CardScriptable.AttackType.X)
                {
                    if(_user.tag == "Player")
                    {
                        _scriptable._damageInstances = _user.GetComponent<PlayerStats>()._mana;
                    }
                    else
                    {
                        _scriptable._damageInstances = 5;
                    }
                }

                targetDamage += _scriptable._accumulatedDamage + rage;

                //Deals damage once for every _damageInstance
                CharacterStats target = _singleTarget.GetComponent<CharacterStats>();
                for(int i = 0; i < _scriptable._damageInstances; i++)
                {
                    if (_user.tag == "Magician")
                    { 
                        //_singleTarget = GameObject.FindGameObjectWithTag("Player");
                    }
                    if(checkProjection)
                    {
                        target.CheckProjectedDamage(targetDamage, _user.gameObject, ignoreArmor, false, false, ref _projectedChangePerTarget);
                    }
                    else
                    {
                        target.TakeDamage(targetDamage, _user.gameObject, ignoreArmor, false, false);
                    }
                }
            }

            if(_scriptable._hitAllEnemies)
            {
                foreach(GameObject target in _targets)
                {
                    if(_scriptable._hitSingleEnemy && _singleTarget == target)
                    { continue; }

                    float allDamage = _scriptable._allDamage;
                    CardConditions.Conditions cond = _scriptable._condition;
                    if(cond != CardConditions.Conditions.NO_CONDITION && CardConditions.Evaluate(cond, _user, target, _scriptable))
                    {
                        allDamage *= _scriptable._damageMultiplier;
                        allDamage += _scriptable._bonusDamage;
                    }
                    if(_scriptable._attackType == CardScriptable.AttackType.SINGLE)
                    {
                        _scriptable._damageInstances = 1;
                    }
                    else if(_scriptable._attackType == CardScriptable.AttackType.X)
                    {
                        if(_user.tag == "Player")
                        {
                            _scriptable._damageInstances = _user.GetComponent<PlayerStats>()._mana;
                        }
                        else
                        {
                            _scriptable._damageInstances = 5;
                        }
                    }

                    allDamage += rage;

                    CharacterStats targetStats = target.GetComponent<CharacterStats>();

                    //Deals damage once for every _damageInstance
                    for(int i = 0; i < _scriptable._damageInstances; i++)
                    {
                        if(checkProjection)
                        {
                            targetStats.CheckProjectedDamage(allDamage, _user.gameObject, ignoreArmor, false, false, ref _projectedChangePerTarget);
                        }
                        else
                        {
                            targetStats.TakeDamage(allDamage, _user.gameObject, ignoreArmor, false, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds basic armor to the character.
        /// </summary>
        private void AddArmor(bool checkProjection = false)
        {
            float armor = _scriptable._armor;
            CardConditions.Conditions cond = _scriptable._condition;
            if(cond != CardConditions.Conditions.NO_CONDITION && CardConditions.Evaluate(cond, _user, _singleTarget, _scriptable))
            {
                armor *= _scriptable._armorMultiplier;
            }
            armor += _scriptable._accumulatedArmor;

            if(checkProjection)
            {
                CharacterStats stat = _user.GetComponent<CharacterStats>();
                _projectedChangePerTarget[stat]._armorChange += Mathf.RoundToInt(armor);
                _projectedChangePerTarget[stat]._temporaryFortify += _scriptable._damageReduction;
            }
            else
            {
                _user.GetComponent<ArmorManager>().Add(armor);
                _user.GetComponent<StatusEffectManager>().AddTemporary(StatusEffectEnum.FORTIFY, _scriptable._damageReduction);
            }
        }

        /// <summary>
        /// Restores basic mana to the character.
        /// </summary>
        private void AddMana(bool checkProjection = false)
        {
            float mana = _scriptable._manaRestore;

            //Checks if there is a special condition to restore extra mana
            CardConditions.Conditions cond = _scriptable._condition;
            if(cond != CardConditions.Conditions.NO_CONDITION && CardConditions.Evaluate(cond, _user, _singleTarget, _scriptable))
            {
                mana *= _scriptable._manaMultiplier;
            }

            CharacterStats stat = _user.GetComponent<CharacterStats>();
            if(checkProjection)
            {
                _projectedChangePerTarget[stat]._manaChange += Mathf.RoundToInt(mana);
            }
            else
            {
                _user.GetComponent<CharacterStats>().AddMana(mana);
            }
        }

        private void SelfDamage(bool checkProjection = false)
        {
            CharacterStats stat = _user.GetComponent<CharacterStats>();

            if(_scriptable._selfDamage > 0)
            {
                if(checkProjection)
                {
                    stat.CheckProjectedDamage(_scriptable._selfDamage, _user.gameObject, false, true, true, ref _projectedChangePerTarget);
                }
                else
                {
                    stat.TakeDamage(_scriptable._selfDamage, _user.gameObject, false, true, true);
                }
            }

            StatusEffectManager statusEffectMan = _user.GetComponent<StatusEffectManager>();

            if(_scriptable._selfPoison > 0)
            {
                if(checkProjection)
                {
                    statusEffectMan.AddProjectedStatusEffect(StatusEffectEnum.POISON, _scriptable._selfPoison, ref _projectedChangePerTarget);
                }
                else
                {
                    statusEffectMan.AddStatusEffect(StatusEffectEnum.POISON, _scriptable._selfPoison);
                }
            }

            if(_scriptable._selfWeakness > 0)
            {
                if(checkProjection)
                {
                    statusEffectMan.AddProjectedStatusEffect(StatusEffectEnum.VULNERABLE, _scriptable._selfWeakness, ref _projectedChangePerTarget);
                }
                else
                {
                    statusEffectMan.AddStatusEffect(StatusEffectEnum.VULNERABLE, _scriptable._selfWeakness);
                }
            }
        }

        private void Healing(bool checkProjection = false)
        {
            CharacterStats stat = _user.GetComponent<CharacterStats>();

            if(checkProjection)
            {
                _projectedChangePerTarget[stat]._healChange += _scriptable._healingAmount;
            }
            else
            {
                stat.AddHealth(_scriptable._healingAmount);
            }
            
            CardConditions.Conditions cond = _scriptable._condition;
            if(cond != CardConditions.Conditions.NO_CONDITION && CardConditions.Evaluate(cond, _user, _singleTarget, _scriptable))
            {
                if(checkProjection)
                {
                    _projectedChangePerTarget[stat]._maxHealthChange += _scriptable._addedMaxHealth;
                }
                else
                {
                    stat.AddMaxHealth(_scriptable._addedMaxHealth, false);
                    PartyHandler.Instance._addedMaxHealth += _scriptable._addedMaxHealth;
                }
            }

            if(_scriptable._canTargetAllAllies)
            {
                foreach(GameObject ally in _allies)
                {
                    CharacterStats allyStat = ally.GetComponent<CharacterStats>();
                    if(checkProjection)
                    {
                        _projectedChangePerTarget[allyStat]._healChange += _scriptable._healingAmount;
                    }
                    else
                    {
                        allyStat.AddHealth(_scriptable._healingAmount);
                    }
                }
            }
        }

        private void StatusEffects(bool checkProjection = false)
        {
            if(_scriptable._hitSingleEnemy)
            {
                int poison = _scriptable._poison;
                CardConditions.Conditions cond = _scriptable._condition;
                if(cond != CardConditions.Conditions.NO_CONDITION && CardConditions.Evaluate(cond, _user, _singleTarget, _scriptable))
                {
                    poison += _scriptable._addedPoison;
                }

                StatusEffectManager statusEffectMan = _singleTarget.GetComponent<StatusEffectManager>();

                if(poison > 0)
                {
                    if(checkProjection)
                    {
                        statusEffectMan.AddProjectedStatusEffect(StatusEffectEnum.POISON, poison, ref _projectedChangePerTarget);
                    }
                    else
                    {
                        statusEffectMan.AddStatusEffect(StatusEffectEnum.POISON, poison);
                    }
                }

                if(_scriptable._weakness > 0)
                {
                    if(checkProjection)
                    {
                        statusEffectMan.AddProjectedStatusEffect(StatusEffectEnum.VULNERABLE, _scriptable._weakness, ref _projectedChangePerTarget);
                    }
                    else
                    {
                        statusEffectMan.AddStatusEffect(StatusEffectEnum.VULNERABLE, _scriptable._weakness);
                    }
                }
            }

            if(_scriptable._hitAllEnemies)
            {
                foreach(GameObject target in _targets)
                {
                    int poison = _scriptable._poison;
                    CardConditions.Conditions cond = _scriptable._condition;
                    if(cond != CardConditions.Conditions.NO_CONDITION && CardConditions.Evaluate(cond, _user, target, _scriptable))
                    {
                        poison += _scriptable._addedPoison;
                    }

                    StatusEffectManager statusEffectMan = target.GetComponent<StatusEffectManager>();

                    if(poison > 0)
                    {
                        if(checkProjection)
                        {
                            statusEffectMan.AddProjectedStatusEffect(StatusEffectEnum.POISON, poison, ref _projectedChangePerTarget);
                        }
                        else
                        {
                            statusEffectMan.AddStatusEffect(StatusEffectEnum.POISON, Mathf.RoundToInt(poison));
                        }
                    }

                    if(_scriptable._weakness > 0)
                    {
                        if(checkProjection)
                        {
                            statusEffectMan.AddProjectedStatusEffect(StatusEffectEnum.VULNERABLE, _scriptable._weakness, ref _projectedChangePerTarget);
                        }
                        else
                        {
                            statusEffectMan.AddStatusEffect(StatusEffectEnum.VULNERABLE, _scriptable._weakness);
                        }
                    }
                }
            }

            StatusEffectManager userStatEffectMan = _user.GetComponent<StatusEffectManager>();
            //add defense
            if(_scriptable._defense > 0)
            {
                if(checkProjection)
                {
                    userStatEffectMan.AddProjectedStatusEffect(StatusEffectEnum.FORTIFY, _scriptable._defense, ref _projectedChangePerTarget);
                }
                else
                {
                    userStatEffectMan.AddStatusEffect(StatusEffectEnum.FORTIFY, _scriptable._defense);
                }
            }

            if(_scriptable._rage > 0)
            {
                if(checkProjection)
                {
                    userStatEffectMan.AddProjectedStatusEffect(StatusEffectEnum.RAGE, _scriptable._rage, ref _projectedChangePerTarget);
                }
                else
                {
                    userStatEffectMan.AddStatusEffect(StatusEffectEnum.RAGE, _scriptable._rage);
                }
            }

            //add exhausted
            if(_scriptable._exhausted)
            {
                if (_user.GetComponent<PlayerTurn>())//not allowing magician to exhaust party member by using exhausting cards
                {
                    string charId = _scriptable._id.Split('-')[0];
                    foreach (GameObject party in GameObject.FindGameObjectsWithTag("PartyMember"))
                    {
                        if (party.name == charId)
                        {
                            PartyTurn partyTurn = party.GetComponent<PartyTurn>();
                            if(partyTurn != null)
                            {
                                if(checkProjection)
                                {
                                    _projectedChangePerTarget[_user.GetComponent<CharacterStats>()].AddExhausted(partyTurn);
                                }
                                else
                                {
                                    partyTurn.Exhaust();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnHit(bool checkProjection = false)
        {
            CharacterStats stat = _user.GetComponent<CharacterStats>();
            if(_scriptable._hitDamage > 0)
            {
                if(checkProjection)
                {
                    _projectedChangePerTarget[stat]._hitDamageChange += _scriptable._hitDamage;
                }
                else
                {
                    stat.AddHitDamage(_scriptable._hitDamage);
                }
            }

            if(_scriptable._hitPoison > 0)
            {
                if(checkProjection)
                {
                    _projectedChangePerTarget[stat]._hitPoisonChange += _scriptable._hitPoison;
                }
                else
                {
                    stat.AddHitPoison(_scriptable._hitPoison);
                }
            }

            if(_scriptable._hitWeakness > 0)
            {
                if(checkProjection)
                {
                    _projectedChangePerTarget[stat]._hitWeaknessChange += _scriptable._hitWeakness;
                }
                else
                {
                    stat.AddHitWeakness(_scriptable._hitWeakness);
                }
            }
        }

        private void Adjustable()
        {
            CharacterTurn turn = _user.GetComponent<CharacterTurn>();
            if (turn._characterName.Equals("Ace") || turn._characterName.Equals("Mascot"))
            {
                if (_scriptable._damagePerUse != 0)
                {
                    _scriptable._accumulatedDamage += _scriptable._damagePerUse;
                    if (_scriptable._accumulatedDamage < 0 && Mathf.Abs(_scriptable._accumulatedDamage) >= Mathf.Abs(_scriptable._targetDamage))
                    {
                        _tempBurn = true;
                    }

                    string desc = _scriptable._cardDescUneditted;
                    desc = desc.Replace("<TargetDamage>", "" + (_scriptable._targetDamage + _scriptable._accumulatedDamage));
                    desc = desc.Replace("<DamagePerUse>", "" + Mathf.Abs(_scriptable._damagePerUse));
                    _scriptable._cardDesc = desc;
                }

                if (_scriptable._armorPerUse != 0)
                {
                    _scriptable._accumulatedArmor += _scriptable._armorPerUse;
                    if (_scriptable._accumulatedArmor < 0 && Mathf.Abs(_scriptable._accumulatedArmor) >= Mathf.Abs(_scriptable._armor))
                    {
                        _tempBurn = true;
                    }

                    string desc = _scriptable._cardDescUneditted;
                    desc = desc.Replace("<ArmorAmount>", "" + (_scriptable._armor + _scriptable._accumulatedArmor));
                    desc = desc.Replace("<ArmorPerUse>", "" + Mathf.Abs(_scriptable._armorPerUse));
                    _scriptable._cardDesc = desc;
                }
            }
        }

        private void OnDiscard(bool checkProjection = false)
        {
            if(_scriptable._discardDamage > 0)
            {
                CharacterStats userStats = _user.GetComponent<CharacterStats>();
                if(checkProjection)
                {
                    userStats.CheckProjectedDamage(_scriptable._discardDamage, _user.gameObject, false, true, true, ref _projectedChangePerTarget);
                }
                else
                {
                    userStats.TakeDamage(_scriptable._discardDamage, _user.gameObject, false, true, true);
                }
            }

            if(_scriptable._discardPoison > 0)
            {
                StatusEffectManager statEffectMan = _user.GetComponent<StatusEffectManager>();
                if(checkProjection)
                {
                    statEffectMan.AddProjectedStatusEffect(StatusEffectEnum.POISON, _scriptable._discardPoison, ref _projectedChangePerTarget);
                }
                else
                {
                    statEffectMan.AddStatusEffect(StatusEffectEnum.POISON, _scriptable._discardPoison);
                }
            }
        }

        /// <summary>
        /// Initializes the card with user and target values
        /// </summary>
        /// <param name="scriptable"></param> Scriptable object holding the data of this card
        /// <param name="user"></param> GameObject of the agent that activated this card
        /// <param name="targetTag"></param> Tag for who can be hit by this card
        public virtual void Initialize(CardScriptable scriptable, GameObject user, string targetTag)
        {
            _targets = new List<GameObject>();
            _allies = new List<GameObject>();

            Initialize(scriptable);
            Initialize(user, targetTag);

            if(_scriptable._canTargetAllAllies)
            {
                foreach(GameObject ally in GameObject.FindGameObjectsWithTag(user.tag))
                {
                    if(ally == _user || ally.GetComponent<CharacterTurn>() == null)
                    { continue; }

                    _allies.Add(ally);
                }
            }
        }

        /// <summary>
        /// Initializes the card with user and target values
        /// </summary>
        /// <param name="user"></param> GameObject of the agent that activated this card
        /// <param name="targetTag"></param> Tag for who can be hit by this card
        public virtual void Initialize(GameObject user, string targetTag)
        {
            _user = user;
            FindTargets(targetTag);
        }

        /// <summary>
        /// Initializes the card with the scriptable object values
        /// </summary>
        /// <param name="scriptable"></param> Scriptable object holding the data of this card
        public virtual void Initialize(CardScriptable scriptable)
        {
            _scriptable = scriptable;

            _nameText.text = _scriptable._cardName;
            string manaText = "";
            switch(_scriptable._manaType)
            {
                case CardScriptable.ManaType.VALUE:
                    manaText = "" + _scriptable._manaCost;
                    break;
                case CardScriptable.ManaType.MAX:
                    manaText = "MAX";
                    break;
                case CardScriptable.ManaType.X:
                    manaText = "X";
                    break;
                default:
                    break;
            }
            _manaCostText.text = manaText;
            _descriptionText.text = _scriptable._cardDesc;

            //Sets the color of the card background based off the card class
            switch(_scriptable._class)
            {
                case CardScriptable.CardClass.ATTACK:
                    _classImage.sprite = _attackImage;
                    _backgroundImage.sprite = _attackBackground;
                    break;
                case CardScriptable.CardClass.DEFENSE:
                    _classImage.sprite = _defenseImage;
                    _backgroundImage.sprite = _defenseBackground;
                    break;
                case CardScriptable.CardClass.SUPPORT:
                    _classImage.sprite = _supportImage;
                    _backgroundImage.sprite = _supportBackground;
                    break;
                default:
                    break;
            }

            switch(_scriptable._id.Split('-')[0])
            {
                case "Ace":
                    _cardImage.sprite = _spades;
                    _backsideImage.sprite = _aceBack;
                    break;
                case "Contortionist":
                    _cardImage.sprite = _diamonds;
                    _backsideImage.sprite = _contortionistBack;
                    break;
                case "Mascot":
                    _cardImage.sprite = _hearts;
                    _backsideImage.sprite = _mascotBack;
                    break;
                case "Mime":
                    _cardImage.sprite = _clubs;
                    _backsideImage.sprite = _mimeBack;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Finds a target based off the targetTag
        /// </summary>
        /// <param name="targetTag"></param> String tag of any target that can be hit by this card
        protected virtual void FindTargets(string targetTag)
        {
            if(_scriptable._canTargetEnemy)
            {
                if(_scriptable._hitAllEnemies)
                {
                    if(_targets.Count == 0)
                    {
                        foreach(GameObject target in GameObject.FindGameObjectsWithTag(targetTag))
                        {
                            _targets.Add(target);
                        }
                    }
                    _singleTarget = _targets[0];
                }
                else
                {
                    _singleTarget = GameObject.FindGameObjectWithTag(targetTag);
                }
            }
        }

        public GameObject GetTarget()
        {
            if(_scriptable._canTargetSelf)
            {
                return _user;
            }

            if(_scriptable._hitAllEnemies && !_scriptable._hitSingleEnemy)
            {
                return _targets[0];
            }
            
            return _singleTarget;
        }

        /// <summary>
        /// Destroys this gameObject
        /// </summary>
        public void Destroy()
        {
            transform.DOComplete();
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Sets the target of the card directly
        /// </summary>
        /// <param name="target"></param> GameObject being targetted by this card
        public void SetTarget(GameObject target)
        {
            _singleTarget = target;
        }

        public void SetTarget(List<GameObject> targets)
        {
            _targets = new List<GameObject>(targets);
        }
        public void UseAnimation()
        {
            string charId = _scriptable._id.Split('-')[0];
            if (charId == "Mime" || charId == "Mascot" || charId == "Contortionist")
            {
                foreach (GameObject party in GameObject.FindGameObjectsWithTag("PartyMember"))
                {
                    if (party.name == charId)
                    {
                        Sequence seq = DOTween.Sequence();
                        seq.Append(party.transform.DOMove(party.GetComponent<PartyTurn>()._onScreenPos, .6f).SetEase(Ease.OutSine)).Append(party.transform.DOMove(party.GetComponent<PartyTurn>()._offScreenPos, .6f).SetEase(Ease.OutSine).SetDelay(.5f));
                    }
                }
            }
        }

        public void DiscardCard(bool onDiscard, string userTag)
        {
            if(onDiscard && !_activated && _scriptable._discardEffect)
            {
                OnDiscard();
            }

            if(userTag != "Player")
            { return; }

            DraggableCard drag = GetComponent<DraggableCard>();
            if(drag != null)
            {
                drag.Discard();
            }

            string charId = _scriptable._id.Split('-')[0];
            if(charId == "Mime" || charId == "Mascot" || charId == "Contortionist")
            {
                foreach(GameObject party in GameObject.FindGameObjectsWithTag("PartyMember"))
                {
                    if(party.name == charId)
                    {
                        party.GetComponent<CharacterTurn>().AddDiscard(_scriptable);
                    }
                }
            }
            else if(charId == "Ace")
            {
                _user.GetComponent<CharacterTurn>().AddDiscard(_scriptable);
            }
        }

        private void Burn()
        {
            if(!_activated && _scriptable._discardEffect)
            {
                OnDiscard();
            }

            string charId = _scriptable._id.Split('-')[0];
            if(charId == "Mime" || charId == "Mascot" || charId == "Contortionist")
            {
                EnemyTurn turn = _user.GetComponent<EnemyTurn>();
                if(turn != null)
                {
                    turn.Burn(_scriptable);
                }
                return;
            }
            else if(charId == "Ace")
            {
                return;
            }
            else
            {
                _user.GetComponent<EnemyTurn>().Burn(_scriptable);
            } 
        }

        public void Blind()
        {
            _blinded = true;
            _nameText.text = "Smokescreen";
            _manaCostText.text = "";
            _descriptionText.text = "???";
            _classObject.SetActive(false);
            _cardImage.sprite = _spades;
        }

        public bool IsBlind()
        {
            return _blinded;
        }

        public static GameObject ParseUser(string id)
        {
            string charId = id.Split('-')[0];
            if(charId == "Ace")
            {
                return GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                foreach(GameObject party in GameObject.FindGameObjectsWithTag("PartyMember"))
                {
                    if(party.name == charId)
                    {
                        return party;
                    }
                }
            }
            return null;
        }

        private void CardSoundEffect() {
            switch (_scriptable._class) 
            {
                case CardScriptable.CardClass.ATTACK:
                    FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_Hit);
                    break;
                case CardScriptable.CardClass.DEFENSE:
                    FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_Armor);
                    break;
                case CardScriptable.CardClass.SUPPORT:
                    FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_Special);
                    break;
                default:
                    break;

            }
        }
    }
}

