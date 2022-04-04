using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the mana values of the player as well as other associated stats.
    /// </summary>
    public class PlayerStats : CharacterStats
    {
        [Header("Values")]
        [Tooltip("Maximum base amount of mana")]
        public int _maxMana;

        [Header("References")]
        [SerializeField][Tooltip("Text showing mana values")]
        private TextMeshProUGUI _manaText;
        [SerializeField] private GameObject _manaPanel = null;

        //Current amount of mana the player has left
        [HideInInspector]
        public int _mana
        {
            get;
            protected set;
        }

        public int _healthPerLevel;

        private string _projManaStr = "";
        private string _projMaxManaStr = "";

        private int _tempReducedMana = 0;
        
        protected override void Awake()
        {
            base.Awake();

            _mana = _maxMana;
            UpdateMana();

            SetMaxHealth(_maxHealth + (_healthPerLevel * PartyHandler.Instance._characterLevels["Ace"]) + PartyHandler.Instance._addedMaxHealth);

            //SetMaxHealth()
            OnDamage += DamageReaction;
        }

        public void DamageReaction(GameObject obj)
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Player_Health_Drop);
            GetComponentInChildren<Animator>().SetTrigger("takeDamage");
        }

        /// <summary>
        /// Updates the mana text visual to show the current amount of mana
        /// </summary>
        private void UpdateMana()
        {
            _manaText.text = _mana + "/" + _maxMana;

            _projManaStr = _mana.ToString();
            _projMaxManaStr = _maxMana.ToString();
        }

        /// <summary>
        /// Upgrades the maximum amount of mana this character can use.
        /// </summary>
        public void UpgradeMana()
        {
            _maxMana++;
            _mana = _maxMana;
            UpdateMana();
        }

        public void UpgradeMana(int amount)
        {
            _maxMana += amount;
            _mana += amount;
            UpdateMana();
        }

        public void ReduceMaxMana(int amount)
        {
            _maxMana -= amount;
            UpdateMana();
        }

        public void SetReduceMana(int reduction)
        {
            _tempReducedMana = reduction;
        }

        /// <summary>
        /// Uses the given amount of mana
        /// </summary>
        /// <param name="manaCost"></param> Amount of mana to take away
        public override void UseMana(float manaCost)
        {
            _mana -= Mathf.RoundToInt(manaCost);

            UpdateMana();
        }

        /// <summary>
        /// Adds the given amount of mana to the current mana value.
        /// </summary>
        /// <param name="manaValue"></param> Amount of mana to add
        public override void AddMana(float manaValue)
        {
            _mana += Mathf.RoundToInt(manaValue);

            UpdateMana();
        }

        /// <summary>
        /// Called at the start of the player turn.
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();

            //_manaText.gameObject.SetActive(true);
            _manaPanel.SetActive(true);
            _mana = _maxMana - _tempReducedMana;

            _tempReducedMana = 0;

            UpdateMana();

        }

        /// <summary>
        /// Called at the end of the player turn
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();

            //_manaText.gameObject.SetActive(false);
            _manaPanel.SetActive(false);
            _mana = _maxMana;

            UpdateMana();
        }

        public override void ManaProjectionView(float manaCost)
        {
            int projMana = Mathf.Max(_mana + Mathf.RoundToInt(manaCost), 0);

            //If healing results in no change in health, don't show anything
            if(projMana == _mana)
            {
                return;
            }

            if(projMana > _mana)
            {
                _projManaStr = _projectionGreenString + projMana + "</color>";
            }
            else
            {
                _projManaStr = _projectionRedString + projMana + "</color>";
            }

            _manaText.text = _projManaStr + "/" + _projMaxManaStr;
        }

        public override void MaxManaProjectionView(float maxManaChange)
        {
            int projMaxMana = Mathf.Max(_maxMana + Mathf.RoundToInt(maxManaChange), 0);

            if(projMaxMana == _maxMana)
            {
                return;
            }

            if(projMaxMana > _maxMana)
            {
                _projMaxManaStr = _projectionGreenString + projMaxMana + "</color>";
            }
            else
            {
                _projMaxManaStr = _projectionRedString + projMaxMana + "</color>";
            }

            _manaText.text = _projManaStr + "/" + _projMaxManaStr;
        }

        public override void RemoveProjectionView(bool cardPlayed = true)
        {
            UpdateMana();

            base.RemoveProjectionView(cardPlayed);
        }
    }
}

