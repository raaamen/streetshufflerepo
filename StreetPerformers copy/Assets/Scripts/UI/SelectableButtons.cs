using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class SelectableButtons : MonoBehaviour
    {
        [SerializeField] private List<Button> _buttons = new List<Button>();
        [SerializeField] private bool _vertical = false;
        [SerializeField] private bool _wraps = true;
        [SerializeField] private bool _reverseDirection = false;
        [SerializeField] private Color _selectColor = Color.white;

        [SerializeField] private List<SelectableButtons> _disableSelectableButtons = new List<SelectableButtons>();
        [SerializeField] private List<SelectableButtons> _enableSelectableButtons = new List<SelectableButtons>();

        [SerializeField] private List<ButtonListener> _disableButtonListeners = new List<ButtonListener>();
        [SerializeField] private List<ButtonListener> _enableButtonListeners = new List<ButtonListener>();

        private List<Image> _buttonImages = new List<Image>();
        private Color _startColor = Color.white;
        private int _currentIndex = 0;

        private float _bufferTime = .3f;
        private float _timer = 0f;
        private int _lastDirection = 0;

        private bool _enabled = true;

        private void Start()
        {
            for (int i = _buttons.Count - 1; i >= 0; i--)
            {
                if (!_buttons[i].gameObject.activeInHierarchy)
                {
                    _buttons.RemoveAt(i);
                }
            }

            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttonImages.Add(_buttons[i].GetComponent<Image>());
            }

            _startColor = _buttonImages[0].color;
            _buttonImages[0].color = _selectColor;

        }

        private void Update()
        {
            if(!_enabled)
            { return; }

            UpdateInput();

            if (_timer > 0f)
            {
                _timer -= Time.unscaledDeltaTime;
            }
        }

        private void UpdateInput()
        {
            int index = _currentIndex;

            int moveValue = 0;
            if (_vertical)
            {
                moveValue = -Mathf.RoundToInt(Input.GetAxis("Vertical"));
            }
            else
            {
                moveValue += Mathf.RoundToInt(Input.GetAxis("Horizontal"));
            }

            if (_reverseDirection)
            {
                moveValue *= -1;
            }

            if (moveValue == 0)
            {
                _lastDirection = 0;
            }
            else if (moveValue != _lastDirection || _timer <= 0f)
            {
                _timer = _bufferTime;
                _lastDirection = moveValue;
                index += moveValue;
            }

            if(_wraps)
            {
                if (index < 0)
                {
                    index = _buttons.Count - 1;
                }
                else if (index > _buttons.Count - 1)
                {
                    index = 0;
                }
            }
            else
            {
                index = Mathf.Clamp(index, 0, _buttons.Count - 1);  
            }

            if (index != _currentIndex)
            {
                _buttonImages[_currentIndex].color = _startColor;

                _currentIndex = index;
                _buttonImages[_currentIndex].color = _selectColor;
            }

            if (Input.GetButtonDown("Select"))
            {
                _buttons[_currentIndex].onClick.Invoke();

                foreach(SelectableButtons selectableButton in _disableSelectableButtons)
                {
                    selectableButton.ToggleEnabled(false);
                }

                foreach(SelectableButtons selectableButton in _enableSelectableButtons)
                {
                    selectableButton.ToggleEnabled(true);
                }

                foreach(ButtonListener buttonListener in _disableButtonListeners)
                {
                    buttonListener.ToggleEnabled(false);
                }

                foreach(ButtonListener buttonListener in _enableButtonListeners)
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