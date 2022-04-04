using System.Collections.Generic;

namespace StreetPerformers
{
    /// <summary>

    /// </summary>
    public class CharacterStatChangeData
    {
        public int _damageChange = 0;
        public int _healChange = 0;
        public int _maxHealthChange = 0;
        public int _armorChange = 0;
        public int _manaChange = 0;
        public int _maxManaChange = 0;

        public Dictionary<StatusEffectEnum, int> _statChange = new Dictionary<StatusEffectEnum, int>();
        public List<PartyTurn> _exhaustedAllies = new List<PartyTurn>();

        public int _hitDamageChange = 0;
        public int _hitPoisonChange = 0;
        public int _hitWeaknessChange = 0;

        public int _hiddenDamageChange = 0;
        public int _hiddenArmorChange = 0;
        public Dictionary<StatusEffectEnum, int> _hiddenStatChange = new Dictionary<StatusEffectEnum, int>();

        public int _vulnerablePerDiscard = 0;
        public int _healthPerDiscard = 0;

        public int _poisonPerCardUsed = 0;
        public int _damagePerCardUsed = 0;

        public int _transferArmor = 0;

        public int _temporaryFortify = 0;

        //This is to keep track of how much damage/poison/weakness is applied as a result of hitting
        //a target with on hit effects on them. This is purely for display purposes
        public int _hitDamageReceived = 0;
        public int _hitPoisonReceived = 0;
        public int _hitWeaknessReceived = 0;
        
        public CharacterStatChangeData()
        {
            _damageChange = 0;
            _healChange = 0;
            _maxHealthChange = 0;
            _armorChange = 0;
            _manaChange = 0;
            _maxManaChange = 0;

            _statChange = new Dictionary<StatusEffectEnum, int>();
            _statChange[StatusEffectEnum.FORTIFY] = 0;
            _statChange[StatusEffectEnum.POISON] = 0;
            _statChange[StatusEffectEnum.RAGE] = 0;
            _statChange[StatusEffectEnum.VULNERABLE] = 0;

            _exhaustedAllies = new List<PartyTurn>();

            _hitDamageChange = 0;
            _hitPoisonChange = 0;
            _hitWeaknessChange = 0;

            _hiddenDamageChange = 0;
            _hiddenArmorChange = 0;

            _hiddenStatChange = new Dictionary<StatusEffectEnum, int>();
            _hiddenStatChange[StatusEffectEnum.FORTIFY] = 0;
            _hiddenStatChange[StatusEffectEnum.POISON] = 0;
            _hiddenStatChange[StatusEffectEnum.RAGE] = 0;
            _hiddenStatChange[StatusEffectEnum.VULNERABLE] = 0;

            _vulnerablePerDiscard = 0;
            _healthPerDiscard = 0;

            _poisonPerCardUsed = 0;
            _damagePerCardUsed = 0;

            _transferArmor = 0;

            _temporaryFortify = 0;

            _hitDamageReceived = 0;
            _hitPoisonReceived = 0;
            _hitWeaknessReceived = 0;
        }

        public void Reset()
        {
            _damageChange = 0;
            _healChange = 0;
            _maxHealthChange = 0;
            _armorChange = 0;
            _manaChange = 0;
            _maxManaChange = 0;

            _statChange[StatusEffectEnum.FORTIFY] = 0;
            _statChange[StatusEffectEnum.POISON] = 0;
            _statChange[StatusEffectEnum.RAGE] = 0;
            _statChange[StatusEffectEnum.VULNERABLE] = 0;

            _exhaustedAllies.Clear();

            _hitDamageChange = 0;
            _hitPoisonChange = 0;
            _hitWeaknessChange = 0;

            _hiddenDamageChange = 0;
            _hiddenArmorChange = 0;

            _hiddenStatChange[StatusEffectEnum.FORTIFY] = 0;
            _hiddenStatChange[StatusEffectEnum.POISON] = 0;
            _hiddenStatChange[StatusEffectEnum.RAGE] = 0;
            _hiddenStatChange[StatusEffectEnum.VULNERABLE] = 0;

            _vulnerablePerDiscard = 0;
            _healthPerDiscard = 0;

            _poisonPerCardUsed = 0;
            _damagePerCardUsed = 0;

            _transferArmor = 0;

            _temporaryFortify = 0;

            _hitDamageReceived = 0;
            _hitPoisonReceived = 0;
            _hitWeaknessReceived = 0;
        }

        public void AddExhausted(PartyTurn party)
        {
            if(!_exhaustedAllies.Contains(party))
            {
                _exhaustedAllies.Add(party);
            }
        }

        public override string ToString()
        {
            string changeString = "";

            changeString += AddNonZeroString(_damageChange, "Damage");
            changeString += AddNonZeroString(_hiddenDamageChange, "Hidden Damage");
            changeString += AddNonZeroString(_hitDamageChange, "Hit Damage");

            changeString += AddNonZeroString(_healChange, "Heal");
            changeString += AddNonZeroString(_healthPerDiscard, "Health Per Discard");
            changeString += AddNonZeroString(_damagePerCardUsed, "Health Per Card Used");

            changeString += AddNonZeroString(_maxHealthChange, "Max Health");

            changeString += AddNonZeroString(_armorChange, "Armor");
            changeString += AddNonZeroString(_hiddenArmorChange, "Hidden Armor");
            changeString += AddNonZeroString(_transferArmor, "Transfer Armor");

            changeString += AddNonZeroString(_manaChange, "Mana");

            changeString += AddNonZeroString(_maxManaChange, "Max Mana");

            changeString += AddNonZeroString(_statChange[StatusEffectEnum.FORTIFY], "Fortify");
            changeString += AddNonZeroString(_hiddenStatChange[StatusEffectEnum.FORTIFY], "Hidden Fortify");

            changeString += AddNonZeroString(_statChange[StatusEffectEnum.POISON], "Poison");
            changeString += AddNonZeroString(_hiddenStatChange[StatusEffectEnum.POISON], "Hidden Poison");
            changeString += AddNonZeroString(_poisonPerCardUsed, "Poison Per Card Used");
            changeString += AddNonZeroString(_hitPoisonChange, "Hit Poison");

            changeString += AddNonZeroString(_statChange[StatusEffectEnum.RAGE], "Rage");
            changeString += AddNonZeroString(_hiddenStatChange[StatusEffectEnum.RAGE], "Hidden Rage");

            changeString += AddNonZeroString(_statChange[StatusEffectEnum.VULNERABLE], "Vulnerable");
            changeString += AddNonZeroString(_hiddenStatChange[StatusEffectEnum.VULNERABLE], "Hidden Vulnerable");
            changeString += AddNonZeroString(_vulnerablePerDiscard, "Vulnerable Per Discard");
            changeString += AddNonZeroString(_hitWeaknessChange, "Hit Vulnerable");

            foreach(PartyTurn turn in _exhaustedAllies)
            {
                changeString += $"Exhausted: {turn._characterName}\n";
            }

            return changeString;
        }

        private string AddNonZeroString(int value, string addedString)
        {
            if(value != 0)
            {
                return $"{addedString}: {value}\n";
            }

            return "";
        }
    }
}
