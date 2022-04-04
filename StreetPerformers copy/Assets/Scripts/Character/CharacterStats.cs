using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the health, armor, and other related values for the gameObject
    /// this script is attached to
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Values")]
        [Tooltip("Maximum base health of this character")]
        public int _maxHealth;

        //Current health value of this character
        [HideInInspector]
        public int _health
        {
            get;
            protected set;
        }

        private List<GameObject> _leftoverTargets;

        [Header("References")]
        [SerializeField]
        private GameObject _healthPanel;
        [SerializeField] private GameObject _healthCanvas = null;

        private Image _healthSlider;
        private Image _healthLerpSlider;
        private TextMeshProUGUI _healthText;

        [SerializeField][Tooltip("This character's health color gradient")]
        protected Gradient _gradient;
        [SerializeField] protected Color _projectionRed = Color.white;
        [SerializeField] protected Color _projectionGreen = Color.white;

        [SerializeField]
        private GameObject _healthDiscard = null;
        [SerializeField]
        private TextMeshProUGUI _healthDiscardText = null;

        //Action invoked upon death
        [HideInInspector]
        public Action<GameObject> OnDeath;
        //Action invoked upon any health change
        [HideInInspector]
        public Action OnHealthChange;
        [HideInInspector]
        public Action<GameObject> OnDamage;

        private int _hitDamage;
        private int _hitPoison;
        private int _hitWeakness;

        private int _healthPerDiscard;

        protected string _projectionRedString = "<color=#FF9293>";
        protected string _projectionGreenString = "<color=#96FF96>";

        private string _projHealthStr = "";
        private string _projMaxHealthStr = "";

        private StatusEffectManager _statusManager;
        private CombatEffectManager _combatEffManager;
        private ArmorManager _armorManager;

        private PlayerTurn _playerTurn = null;

        [SerializeField] private Animator _animator = null;

        protected virtual void Awake()
        {
            Initialize();
            SetMaxHealth(_maxHealth);

            _leftoverTargets = new List<GameObject>();

            _playerTurn = GetComponent<PlayerTurn>();
        }

        private void Initialize()
        {
            _healthSlider = _healthPanel.transform.Find("HealthFill").GetComponent<Image>();
            _healthLerpSlider = _healthPanel.transform.Find("HealthGrayFill").GetComponent<Image>();
            _healthText = _healthPanel.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();

            Transform statuses = _healthPanel.transform.Find("Statuses");

            if(_statusManager != null)
            { return; }

            _statusManager = gameObject.AddComponent<StatusEffectManager>();
            _statusManager.Initialize(statuses, _projectionRedString, _projectionGreenString);

            _armorManager = gameObject.AddComponent<ArmorManager>();
            _armorManager.Initialize(this, statuses, _projectionRedString, _projectionGreenString);

            _combatEffManager = gameObject.AddComponent<CombatEffectManager>();
        }

        public virtual void SetMaxHealth(int health)
        {
            _maxHealth = health;

            _health = _maxHealth;

            _healthSlider.fillAmount = 1f;
            _healthSlider.color = _gradient.Evaluate(1f);
            _healthText.text = _health + "/" + _maxHealth;

            _healthLerpSlider.fillAmount = 1f;

            _projHealthStr = _health.ToString();
            _projMaxHealthStr = _maxHealth.ToString();
        }

        /// <summary>
        /// Updates the health slider, text, and lerp slider as well as invokes the OnHealthChange action
        /// </summary>
        protected virtual void UpdateHealth()
        {
            if(_healthSlider == null)
            {
                Initialize();
            }

            float updatedHealth = (float)_health / (float)_maxHealth;

            if(updatedHealth < _healthLerpSlider.fillAmount)
            {
                _healthSlider.fillAmount = updatedHealth;
                _healthSlider.color = _gradient.Evaluate(_healthSlider.fillAmount);

                _healthLerpSlider.color = _projectionRed;
                _healthLerpSlider.DOFillAmount(_healthSlider.fillAmount, 1f);
            }
            else if(_healthLerpSlider.fillAmount != _healthSlider.fillAmount || updatedHealth > _healthLerpSlider.fillAmount)
            {
                _healthLerpSlider.color = _projectionGreen;
                _healthLerpSlider.fillAmount = updatedHealth;
                _healthSlider.DOFillAmount(updatedHealth, 1f).OnUpdate(
                delegate
                {
                    _healthSlider.color = _gradient.Evaluate(_healthSlider.fillAmount);
                });
            }

            _healthText.text = _health + "/" + _maxHealth;

            _projHealthStr = _health.ToString();
            _projMaxHealthStr = _maxHealth.ToString();

            OnHealthChange?.Invoke();
        }

        public virtual void TakeDamage(float damage, GameObject damageSource, bool ignoreArmor, bool ignoreBuffs, bool ignoreOnHit)
        {
            if(_health <= 0)
            { return; }

            int damageTaken = Mathf.RoundToInt(damage);

            if(damageTaken <= 0)
            {
                return;
            }

            if(!ignoreOnHit)
            {
                CheckOnHit(damageSource);
            }

            if(!ignoreBuffs &&
                (_combatEffManager.GetCombatEffect(CombatEffectEnum.BLOCK_ALL) || _combatEffManager.GetCombatEffect(CombatEffectEnum.BLOCK_NEXT)))
            {
                _combatEffManager.SetCombatEffect(CombatEffectEnum.BLOCK_NEXT, false);
                return;
            }

            if(!ignoreBuffs && !_combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            {
                damageTaken += _statusManager.GetStatusEffect(StatusEffectEnum.VULNERABLE);
                damageTaken -= _statusManager.GetStatusEffect(StatusEffectEnum.FORTIFY);
                if(damageTaken <= 0)
                { return; }
            }

            damageTaken = Mathf.Max(damageTaken, 0);

            if(!ignoreArmor)
            {
                //Reduces damage based off the amount of active armor
                int armorDamage = Mathf.Min(damageTaken, _armorManager._armor);
                damageTaken -= armorDamage;
                _armorManager.Subtract(armorDamage);
            }

            //Deals damage
            _health -= damageTaken;
            _health = Mathf.Max(0, _health);

            UpdateHealth();

            //Checks if this player is dead
            if(_health <= 0)
            {
                OnDeath?.Invoke(this.gameObject);
            }
            else if(damageTaken > 0)
            {
                OnDamage?.Invoke(this.gameObject);
            }
        }

        public virtual void CheckProjectedDamage(float damage, GameObject damageSource, bool ignoreArmor, bool ignoreBuffs, bool ignoreOnHit, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool hideProjectedChanges = false)
        {
            if(_health <= 0)
            { return; }

            int damageTaken = Mathf.RoundToInt(damage);

            if(damageTaken <= 0)
            {
                return;
            }

            if(!ignoreOnHit)
            {
                CheckProjectedOnHit(damageSource, ref projectedChanges, hideProjectedChanges);
            }

            if(!ignoreBuffs && !_combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            {
                damageTaken += _statusManager.GetStatusEffect(StatusEffectEnum.VULNERABLE);
                if(damageTaken <= 0)
                { return; }
            }

            damageTaken -= _statusManager.GetStatusEffect(StatusEffectEnum.FORTIFY);
            damageTaken = Mathf.Max(damageTaken, 0);

            CombatEffectManager damageSourceCombatManager = damageSource.GetComponent<CombatEffectManager>();
            //Deals damage
            if (hideProjectedChanges)
            {
                
                if(damageSourceCombatManager.GetCombatEffect(CombatEffectEnum.POISON_HIT))
                {
                    int poisonPerHit = damageSourceCombatManager.GetCombatEffectValue(CombatEffectEnum.POISON_HIT);
                    if (ignoreArmor && damageTaken > 0)
                    {
                        projectedChanges[this]._hiddenStatChange[StatusEffectEnum.POISON] += poisonPerHit;
                    }
                    else if (damageTaken > _armorManager._armor)
                    {
                        projectedChanges[this]._hiddenStatChange[StatusEffectEnum.POISON] += poisonPerHit;
                    }
                }
                projectedChanges[this]._hiddenDamageChange += damageTaken;
            }
            else
            {
                if (damageSourceCombatManager.GetCombatEffect(CombatEffectEnum.POISON_HIT))
                {
                    int poisonPerHit = damageSourceCombatManager.GetCombatEffectValue(CombatEffectEnum.POISON_HIT);
                    if (ignoreArmor && damageTaken > 0)
                    {
                        projectedChanges[this]._statChange[StatusEffectEnum.POISON] += poisonPerHit;
                    }
                    else if (damageTaken > _armorManager._armor)
                    {
                        projectedChanges[this]._statChange[StatusEffectEnum.POISON] += poisonPerHit;
                    }
                }
                projectedChanges[this]._damageChange += damageTaken;
            }
        }

        protected virtual void CheckOnHit(GameObject damageSource)
        {
            CharacterStats stat = damageSource.GetComponent<CharacterStats>();
            if(_hitDamage > 0)
            {
                stat.TakeDamage(_hitDamage, this.gameObject, false, true, true);
            }

            StatusEffectManager statManager = stat.GetComponent<StatusEffectManager>();
            if(_hitPoison > 0)
            {
                statManager.AddStatusEffect(StatusEffectEnum.POISON, _hitPoison);
            }

            if(_hitWeakness > 0)
            {
                statManager.AddStatusEffect(StatusEffectEnum.VULNERABLE, _hitWeakness);
            }
        }

        protected virtual void CheckProjectedOnHit(GameObject damageSource, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool hideProjectedChanges = false)
        {
            CharacterStats stat = damageSource.GetComponent<CharacterStats>();
            if(_hitDamage > 0)
            {
                stat.CheckProjectedDamage(_hitDamage, this.gameObject, false, true, true, ref projectedChanges, hideProjectedChanges);
                projectedChanges[stat]._hitDamageReceived += _hitDamage;
            }

            bool debuff = !stat._combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION);

            if(_hitPoison > 0 && debuff)
            {
                if(hideProjectedChanges)
                {
                    projectedChanges[stat]._hiddenStatChange[StatusEffectEnum.POISON] += _hitPoison;
                }
                else
                {
                    projectedChanges[stat]._statChange[StatusEffectEnum.POISON] += _hitPoison;
                    projectedChanges[stat]._hitPoisonReceived += _hitPoison;
                }
            }

            if(_hitWeakness > 0 && debuff)
            {
                if(hideProjectedChanges)
                {
                    projectedChanges[stat]._hiddenStatChange[StatusEffectEnum.VULNERABLE] += _hitWeakness;
                }
                else
                {
                    projectedChanges[stat]._statChange[StatusEffectEnum.VULNERABLE] += _hitWeakness;
                    projectedChanges[stat]._hitWeaknessReceived += _hitWeakness;
                }
            }
        }

        /// <summary>
        /// Used to heal this character by the given amount
        /// </summary>
        /// <param name="amount"></param> Amount to heal by
        public virtual void AddHealth(float amount)
        {
            _health = Mathf.Min(_health + Mathf.RoundToInt(amount), _maxHealth);
            UpdateHealth();
        }

        public virtual void AddHealthProjectionView(float amount)
        {
            int projHealth = Mathf.Min(_health + Mathf.RoundToInt(amount), _maxHealth);

            //If healing results in no change in health, don't show anything
            if(projHealth == _health)
            {
                return;
            }

            float projHealthRatio = (float)projHealth / (float)_maxHealth;

            _projHealthStr = _projectionGreenString + projHealth + "</color>";
            _healthText.text = _projHealthStr + "/" + _projMaxHealthStr;

            _healthLerpSlider.fillAmount = projHealthRatio;
            _healthLerpSlider.color = _projectionGreen;
        }

        public virtual void TakeDamageProjectionView(float damage)
        {
            int projHealth = Mathf.RoundToInt(Mathf.Max(0, _health - damage));
            float projHealthRatio = (float)projHealth / _maxHealth;
            float curHealthRatio = (float)_health / _maxHealth;

            _projHealthStr = _projectionRedString + projHealth + "</color>";
            _healthText.text = _projHealthStr + "/" + _projMaxHealthStr;

            _healthSlider.fillAmount = projHealthRatio;

            _healthLerpSlider.fillAmount = curHealthRatio;
            _healthLerpSlider.color = _projectionRed;
        }

        public virtual void ManaProjectionView(float manaCost)
        {

        }

        public virtual void MaxManaProjectionView(float maxManaChange)
        {

        }

        public void MaxHealthProjectionView(float maxHealthChange)
        {
            float projMaxHealth = Mathf.Max(_maxHealth + Mathf.RoundToInt(maxHealthChange), 0);

            if(projMaxHealth == _maxHealth)
            {
                return;
            }

            if(projMaxHealth > _maxHealth)
            {
                _projMaxHealthStr = _projectionGreenString + projMaxHealth + "</color>";
            }
            else
            {
                _projMaxHealthStr = _projectionRedString + projMaxHealth + "</color>";
            }

            _healthText.text = _projHealthStr + "/" + _projMaxHealthStr;
        }

        public virtual void RemoveProjectionView(bool cardPlayed = true)
        {
            _healthText.text = _health + "/" + _maxHealth;

            _projHealthStr = _health.ToString();
            _projMaxHealthStr = _maxHealth.ToString();

            _healthSlider.fillAmount = (float)_health / _maxHealth;
            if(!cardPlayed)
            {
                _healthLerpSlider.fillAmount = _healthSlider.fillAmount;
            }

            _armorManager.RemoveProjectionView(cardPlayed);
            _statusManager.RemoveProjectionView(cardPlayed);

            if(_playerTurn != null)
            {
                _playerTurn.RemoveProjectionView();
            }
        }

        /// <summary>
        /// Virtual function that uses up the mana given.
        /// </summary>
        /// <param name="manaCost"></param>
        public virtual void UseMana(float manaCost)
        {

        }

        /// <summary>
        /// Virtual function that adds the given mana value
        /// </summary>
        /// <param name="manaValue"></param>
        public virtual void AddMana(float manaValue)
        {

        }

        /// <summary>
        /// Called at the beginning of the character's turn.
        /// </summary>
        public virtual void StartTurn()
        {
            if(_combatEffManager.GetCombatEffect(CombatEffectEnum.LEFTOVER_ARMOR_ATTACK))
            {
                foreach(GameObject target in _leftoverTargets)
                {
                    if(target == null)
                    { continue; }

                    target.GetComponent<CharacterStats>().TakeDamage(_armorManager._armor, this.gameObject, false, false, false);
                }
                _leftoverTargets.Clear();
            }

            _armorManager.StartTurn();
            _statusManager.StartTurn();
            _combatEffManager.StartTurn();

            _hitDamage = 0;
            _hitPoison = 0;
            _hitWeakness = 0;

            _healthPerDiscard = 0;
        }

        /// <summary>
        /// Called at the end of the character's turn
        /// </summary>
        public virtual void EndTurn()
        {
            if(!_combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            {
                TakeDamage(_statusManager.GetStatusEffect(StatusEffectEnum.POISON), this.gameObject, true, true, true);
            }

            _statusManager.SubtractStatusEffect(StatusEffectEnum.POISON, 1);
            _combatEffManager.EndTurn();
        }
        
        /// <summary>
        /// Returns the amount of health missing.
        /// </summary>
        /// <returns></returns>
        public virtual float GetMissingHealth()
        {
            return _maxHealth - _health;
        }

        public virtual void AddMaxHealth(int addedHealth, bool resetHealth)
        {
            _maxHealth += addedHealth;
            if(resetHealth)
            {
                _health = _maxHealth;
            }
            UpdateHealth();
        }

        public virtual void AddHitDamage(int damage)
        {
            _hitDamage += damage;
        }

        public virtual void AddHitPoison(int poison)
        {
            _hitPoison += poison;
        }

        public virtual void AddHitWeakness(int weakness)
        {
            _hitWeakness += weakness;
        }

        public virtual bool CheckFatal(int damage)
        {
            if(_combatEffManager.GetCombatEffect(CombatEffectEnum.BLOCK_ALL) || _combatEffManager.GetCombatEffect(CombatEffectEnum.BLOCK_NEXT))
            { return false; }

            int projectedDamage = damage + _statusManager.GetStatusEffect(StatusEffectEnum.VULNERABLE) - _statusManager.GetStatusEffect(StatusEffectEnum.FORTIFY);
            return projectedDamage >= _health + _armorManager._armor;
        }

        public virtual void LeftoverArmorAttack(List<GameObject> targets)
        {
            _combatEffManager.SetCombatEffect(CombatEffectEnum.LEFTOVER_ARMOR_ATTACK, true);
            _leftoverTargets = targets;
        }

        public virtual void AddHealthPerDiscard(int healthAmount)
        {
            _healthPerDiscard = healthAmount;
        }

        public virtual void HealthPerDiscard(int discardAmount)
        {
            AddHealth(_healthPerDiscard * discardAmount);
        }

        public virtual void ToggleHealthCanvas(bool toggle)
        {
            _healthCanvas.SetActive(toggle);
        }

        public virtual void SetAnimation(string animation)
        {
            _animator.SetTrigger(animation);
            switch(animation)
            {
                case "takeDamage":
                    FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Player_Health_Drop);
                    break;
            }
        }
    }
}