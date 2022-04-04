using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class StatusEffectManager : MonoBehaviour
    {
        private Dictionary<StatusEffectEnum, StatusEffect> _statusEffects;
        private Dictionary<StatusEffectEnum, int> _perTurnValues = new Dictionary<StatusEffectEnum, int>();

#if UNITY_IOS || UNITY_ANDROID
        private List<Transform> _statusPos;
#endif

        private CombatEffectManager _combatEffManager;

        private string _projectionGreen = "";
        private string _projectionRed = "";

        public void Initialize(Transform statusParentPanel, string projectionRed, string projectionGreen)
        {
            _statusEffects = new Dictionary<StatusEffectEnum, StatusEffect>();
            _statusEffects.Add(StatusEffectEnum.FORTIFY, new StatusEffect(statusParentPanel.Find("FortifyBackground").gameObject, false));
            _statusEffects.Add(StatusEffectEnum.POISON, new StatusEffect(statusParentPanel.Find("PoisonBackground").gameObject, true));
            _statusEffects.Add(StatusEffectEnum.RAGE, new StatusEffect(statusParentPanel.Find("RageBackground").gameObject, false));
            _statusEffects.Add(StatusEffectEnum.VULNERABLE, new StatusEffect(statusParentPanel.Find("VulnerableBackground").gameObject, true));

            _perTurnValues.Add(StatusEffectEnum.FORTIFY, 0);
            _perTurnValues.Add(StatusEffectEnum.POISON, 0);
            _perTurnValues.Add(StatusEffectEnum.RAGE, 0);
            _perTurnValues.Add(StatusEffectEnum.VULNERABLE, 0);

            _projectionGreen = projectionGreen;
            _projectionRed = projectionRed;

#if UNITY_IOS || UNITY_ANDROID
            _statusPos = new List<Transform>();
            for(int i = 0; i <= 3; i++)
            {
                _statusPos.Add(statusParentPanel.Find("Status" + i));
            }
#endif
        }

        public void StartTurn()
        {
            foreach(StatusEffect status in _statusEffects.Values)
            {
                status.StartTurn();
            }

            foreach(StatusEffectEnum statusEffect in _perTurnValues.Keys)
            {
                int perTurnValue = _perTurnValues[statusEffect];
                if(perTurnValue > 0)
                {
                    AddStatusEffect(statusEffect, perTurnValue);
                }
            }
        }

        private void UpdateStatusList()
        {
#if UNITY_IOS || UNITY_ANDROID
            int index = 0;
            foreach(StatusEffect status in _statusEffects.Values)
            {
                if(status.Value > 0)
                {
                    status._visual.transform.position = _statusPos[index].position;
                    index++;
                }
            }
#endif
        }

        public int GetStatusEffect(StatusEffectEnum statusEnum)
        {
            return _statusEffects[statusEnum].Value;
        }

        public void AddStatusEffect(StatusEffectEnum statusEnum, int value)
        {
            StatusEffect status = _statusEffects[statusEnum];

            if(_combatEffManager == null)
            {
                _combatEffManager = GetComponent<CombatEffectManager>();
            }

            if(status._debuff && _combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            { return; }

            status.Add(value);

            UpdateStatusList();
        }

        public void AddProjectedStatusEffect(StatusEffectEnum statusEnum, int value, ref Dictionary<CharacterStats ,CharacterStatChangeData> projectedChanges)
        {
            StatusEffect status = _statusEffects[statusEnum];

            if(_combatEffManager == null)
            {
                _combatEffManager = GetComponent<CombatEffectManager>();
            }

            if(status._debuff && _combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            { return; }

            projectedChanges[GetComponent<CharacterStats>()]._statChange[statusEnum] += value;
        }

        public void AddTemporary(StatusEffectEnum statusEnum, int value)
        {
            StatusEffect status = _statusEffects[statusEnum];

            if(_combatEffManager == null)
            {
                _combatEffManager = GetComponent<CombatEffectManager>();
            }

            if(status._debuff && _combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            { return; }

            status.AddTemporary(value);

            UpdateStatusList();
        }

        public void SubtractStatusEffect(StatusEffectEnum statusEnum, int value)
        {
            _statusEffects[statusEnum].Subtract(value);

            UpdateStatusList();
        }

        public void MultiplyStatusEffect(StatusEffectEnum statusEnum, float multiplier, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection = false)
        {
            StatusEffect status = _statusEffects[statusEnum];

            if(_combatEffManager == null)
            {
                _combatEffManager = GetComponent<CombatEffectManager>();
            }

            if(status._debuff && _combatEffManager.GetCombatEffect(CombatEffectEnum.DEBUFF_PROTECTION))
            { return; }

            if(checkProjection)
            {
                projectedChanges[GetComponent<CharacterStats>()]._statChange[statusEnum] += Mathf.RoundToInt(status.Value * (multiplier - 1f));
            }
            else
            {
                status.Multiply(multiplier);

                UpdateStatusList();
            }
        }

        public void RemoveDebuffs(ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection)
        {
            CharacterStats userStats = GetComponent<CharacterStats>();

            foreach(StatusEffectEnum statEnum in _statusEffects.Keys)
            {
                StatusEffect status = _statusEffects[statEnum];
                if(status._debuff)
                {
                    if(checkProjection)
                    {
                        projectedChanges[userStats]._statChange[statEnum] -= status.Value;
                    }
                    else
                    {
                        status.RemoveAll();
                    }
                }
            }

            if(!checkProjection)
            {
                UpdateStatusList();
            }
        }

        public void ReduceStatus(int reduceAmount, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection)
        {
            foreach(StatusEffectEnum statEnum in _statusEffects.Keys)
            {
                StatusEffect status = _statusEffects[statEnum];
                if(checkProjection)
                {
                    projectedChanges[GetComponent<CharacterStats>()]._statChange[statEnum] -= reduceAmount;
                }
                else
                {
                    status.Subtract(reduceAmount);
                }
            }
        }

        public int GetDebuffCount(StatusEffectEnum statusEffect = StatusEffectEnum.ALL)
        {
            if(statusEffect == StatusEffectEnum.ALL)
            {
                return GetStatusEffect(StatusEffectEnum.POISON) + GetStatusEffect(StatusEffectEnum.VULNERABLE);
            }
            else if(statusEffect == StatusEffectEnum.POISON)
            {
                return GetStatusEffect(StatusEffectEnum.POISON);
            }
            else if(statusEffect == StatusEffectEnum.VULNERABLE)
            {
                return GetStatusEffect(StatusEffectEnum.VULNERABLE);
            }
            return 0;
        }

        public void ShowProjectionView(StatusEffectEnum statusEnum, int statChange)
        {
            if(statChange == 0)
            {
                return;
            }

            StatusEffect statusEffect = _statusEffects[statusEnum];

            int curStat = statusEffect.Value;
            int projectedStat = Mathf.Max(curStat + statChange, 0);

            if(projectedStat > curStat)
            {
                if(curStat == 0)
                {
                    statusEffect.ShowVisual();
                }
                statusEffect._text.text = _projectionGreen + projectedStat + "</color>";
            }
            else
            {
                statusEffect._text.text = _projectionRed + projectedStat + "</color>";
            }
        }

        public void RemoveProjectionView(bool cardPlayed = true)
        {
            foreach(StatusEffect statusEffect in _statusEffects.Values)
            {
                statusEffect.UpdateVisual();
            }
        }

        public void AddStatusEffectPerTurn(StatusEffectEnum statusEffect, int value)
        {
            _perTurnValues[statusEffect] += value;
        }
    }

}
