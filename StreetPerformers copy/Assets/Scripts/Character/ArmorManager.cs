using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StreetPerformers
{
    public class ArmorManager : MonoBehaviour
    {
        [HideInInspector]
        public int _armor
        {
            get;
            private set;
        }
        private int _transferArmor = 0;
        private int _persistentArmor = 0;
        private float _permanentArmorMult = 0f;

        private CharacterStats _charStats;
        private GameObject _visual;
        private TextMeshProUGUI _text;

        private string _projectionRed = "";
        private string _projectionGreen = "";

        public void Initialize(CharacterStats charStats, Transform armorParentPanel, string projectionRed, string projectionGreen)
        {
            _charStats = charStats;

            _visual = armorParentPanel.parent.Find("ArmorBackground").gameObject;
            _text = _visual.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            _projectionRed = projectionRed;
            _projectionGreen = projectionGreen;
        }

        public void StartTurn()
        {
            _armor = Mathf.RoundToInt(_armor * _permanentArmorMult) + _persistentArmor + _transferArmor;
            _transferArmor = 0;
            UpdateVisual();
        }

        public void Add(int value)
        {
            _armor += value;
            _armor = Mathf.Max(0, _armor);
            UpdateVisual();
        }

        public void Add(float value)
        {
            Add(Mathf.RoundToInt(value));
        }

        public void Subtract(int value)
        {
            _armor -= value;
            _armor = Mathf.Max(0, _armor);
            UpdateVisual();
        }

        public void Multiply(float multiplier, ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection)
        {
            if(checkProjection)
            {
                projectedChanges[GetComponent<CharacterStats>()]._armorChange += Mathf.RoundToInt(_armor * (multiplier - 1f));
            }
            else
            {
                _armor = Mathf.RoundToInt(_armor * multiplier);
                UpdateVisual();
            }
        }

        public void RemoveAll()
        {
            _armor = 0;
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            bool active = _visual.activeInHierarchy;
            if(_armor <= 0 && active)
            {
                _visual.SetActive(false);
            }
            else if(_armor > 0)
            {
                if(!active)
                {
                    _visual.SetActive(true);
                }
                _text.text = "" + _armor;
            }
        }

        public virtual void AddPersistentArmor(int armorAmount)
        {
            _persistentArmor += armorAmount;
        }

        public virtual void PermanentArmor(float _armorReduction)
        {
            _permanentArmorMult = _armorReduction;
        }

        public virtual void TransferArmor(ref Dictionary<CharacterStats, CharacterStatChangeData> projectedChanges, bool checkProjection)
        {
            if(checkProjection)
            {
                CharacterStats charStats = GetComponent<CharacterStats>();
                projectedChanges[charStats]._transferArmor = _armor;
                projectedChanges[charStats]._armorChange -= _armor;
            }
        }

        public virtual void AddTransferArmor(int armor)
        {
            _transferArmor += armor;
        }

        public void ShowProjectionView(int armorChange)
        {
            int projectedArmor = Mathf.Max(_armor + armorChange, 0);
            if(projectedArmor > _armor)
            {
                if(_armor == 0)
                {
                    _visual.SetActive(true);
                }
                _text.text = _projectionGreen + projectedArmor + "</color>";
            }
            else
            {
                _text.text = _projectionRed + projectedArmor + "</color>";
            }
        }

        public void RemoveProjectionView(bool cardPlayed = true)
        {
            UpdateVisual();
        }
    }
}
