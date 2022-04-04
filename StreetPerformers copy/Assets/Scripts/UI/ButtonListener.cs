using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class ButtonListener : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private string _key = "";

        [SerializeField] private List<SelectableButtons> _disableSelectableButtons = new List<SelectableButtons>();
        [SerializeField] private List<SelectableButtons> _enableSelectableButtons = new List<SelectableButtons>();

        [SerializeField] private List<ButtonListener> _disableButtonListeners = new List<ButtonListener>();
        [SerializeField] private List<ButtonListener> _enableButtonListeners = new List<ButtonListener>();

        [SerializeField] private List<GameObject> _disabledIfActiveList = new List<GameObject>();

        private bool _enabled = true;

        private void Update()
        {
            if(!_enabled)
            { return; }

            foreach(GameObject obj in _disabledIfActiveList)
            {
                if(obj.activeInHierarchy)
                { return; }
            }

            if(Input.GetButtonDown(_key))
            {
                _button.onClick.Invoke();

                foreach (SelectableButtons selectableButton in _disableSelectableButtons)
                {
                    selectableButton.ToggleEnabled(false);
                }

                foreach (SelectableButtons selectableButton in _enableSelectableButtons)
                {
                    selectableButton.ToggleEnabled(true);
                }

                foreach (ButtonListener buttonListener in _disableButtonListeners)
                {
                    buttonListener.ToggleEnabled(false);
                }

                foreach (ButtonListener buttonListener in _enableButtonListeners)
                {
                    buttonListener.ToggleEnabled(true);
                }
            }
        }

        public void ToggleEnabled(bool enabled)
        {
            _enabled = enabled;
        }
    }
}

