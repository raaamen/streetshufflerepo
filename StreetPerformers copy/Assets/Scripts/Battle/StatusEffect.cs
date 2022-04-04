using TMPro;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Holds the value for a single status effect (Poison, Rage, etc) for one character and handles updating it's visual 
    /// and text in the UI
    /// </summary>
    public class StatusEffect
    {
        public int Value
        {
            get
            {
                return _value + _temporaryValue;
            }
            set
            {
                _value = value;
            }
        }
        private int _value;
        private int _temporaryValue;

        public TextMeshProUGUI _text;
        public GameObject _visual;
        public bool _debuff;

        public StatusEffect(GameObject visual, bool debuff)
        {
            _value = 0;
            _temporaryValue = 0;

            _visual = visual;
            _text = visual.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            _debuff = debuff;
        }

        public void StartTurn()
        {
            _temporaryValue = 0;
            UpdateVisual();
        }

        public void Add(int value)
        {
            _value += value;
            UpdateVisual();
        }

        public void AddTemporary(int value)
        {
            _temporaryValue += value;
            UpdateVisual();
        }

        public void Subtract(int value)
        {
            _value -= value;
            _value = Mathf.Max(0, _value);
            UpdateVisual();
        }

        public void Multiply(float multiplier)
        {
            _value = Mathf.RoundToInt(_value * multiplier);
            UpdateVisual();
        }

        public void RemoveAll()
        {
            _value = 0;
            UpdateVisual();
        }

        public void ShowVisual()
        {
            _visual.SetActive(true);
        }

        public void UpdateVisual()
        {
            bool active = _visual.activeInHierarchy;
            int curValue = _value + _temporaryValue;
            if(curValue <= 0 && active)
            {
                _visual.SetActive(false);
            }
            else if(curValue > 0)
            {
                if(!active)
                {
                    _visual.SetActive(true);
                }
                _text.text = "" + curValue;
            }
        }
    }
}

