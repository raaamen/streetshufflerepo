using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class CombatEffectManager : MonoBehaviour
    {
        private Dictionary<CombatEffectEnum, CombatEffect> _combatEffects;

        public void Awake()
        {
            _combatEffects = new Dictionary<CombatEffectEnum, CombatEffect>();
            _combatEffects.Add(CombatEffectEnum.BLIND, new CombatEffect(false, false, true));
            _combatEffects.Add(CombatEffectEnum.BLOCK_ALL, new CombatEffect(false, true, false));
            _combatEffects.Add(CombatEffectEnum.BLOCK_NEXT, new CombatEffect(true, false, false));
            _combatEffects.Add(CombatEffectEnum.BURN_ALL, new CombatEffect(false, false, true));
            _combatEffects.Add(CombatEffectEnum.BURN_NEXT, new CombatEffect(true, false, false));
            _combatEffects.Add(CombatEffectEnum.DEBUFF_PROTECTION, new CombatEffect(false, true, false));
            _combatEffects.Add(CombatEffectEnum.DOUBLE_ATTACK, new CombatEffect(true, false, false));
            _combatEffects.Add(CombatEffectEnum.IGNORE_ARMOR, new CombatEffect(false, true, false));
            _combatEffects.Add(CombatEffectEnum.LEFTOVER_ARMOR_ATTACK, new CombatEffect(false, true, false));
            _combatEffects.Add(CombatEffectEnum.POISON_HIT, new CombatEffect(false, false, false));
        }

        public void StartTurn()
        {
            foreach(CombatEffect effect in _combatEffects.Values)
            {
                if(effect._resetOnStart)
                {
                    effect.SetStatus(false);
                }
            }
        }

        public void EndTurn()
        {
            foreach(CombatEffect effect in _combatEffects.Values)
            {
                if(effect._resetOnEnd)
                {
                    effect.SetStatus(false);
                }
            }
        }

        public bool GetCombatEffect(CombatEffectEnum effectEnum)
        {
            return _combatEffects[effectEnum]._active;
        }

        public void SetCombatEffect(CombatEffectEnum effectEnum, bool active)
        {
            _combatEffects[effectEnum].SetStatus(active);
        }

        public int GetCombatEffectValue(CombatEffectEnum effectEnum)
        {
            return _combatEffects[effectEnum]._value;
        }

        public void AddCombatEffectValue(CombatEffectEnum effectEnum, int value)
        {
            _combatEffects[effectEnum]._value += value;
        }
    }

}
