using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceControls : MonoBehaviour
{
    [SerializeField] private List<Button> _levelButtons = new List<Button>();
    private List<Image> _levelButtonImages = new List<Image>();

    private int _lastDirection = 0;
    private float _bufferTime = 0f;
    private float _timer = 0f;

    private int _buttonIndex = 0;

    private void Awake()
    {
        for(int i = _levelButtons.Count - 1; i >= 0; i--)
        {
            if(!_levelButtons[i].gameObject.activeInHierarchy)
            {
                _levelButtons.RemoveAt(i);
            }
        }

        for(int i = 0; i < _levelButtons.Count; i++)
        {
            _levelButtonImages.Add(_levelButtons[i].GetComponent<Image>());
        }

        _levelButtonImages[_levelButtonImages.Count - 1].color = Color.grey;
    }

    private void Update()
    {
        int input = -Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int index = _buttonIndex;
        if (input == 1)
        {
            if(_lastDirection != input || _timer <= 0f)
            {
                _lastDirection = input;
                _timer = _bufferTime;

                index++;
                if(index > _levelButtons.Count - 1)
                {
                    index = 0;
                }
            }
        }
        else if (input == -1)
        {
            if(_lastDirection != input || _timer <= 0f)
            {
                _lastDirection = input;
                _timer = _bufferTime;

                index--;
                if(index < 0)
                {
                    index = _levelButtons.Count - 1;
                }
            }
        }
        else
        {
            _lastDirection = 0;
        }

        if(index != _buttonIndex)
        {
            _levelButtonImages[_buttonIndex].color = Color.white;

            _buttonIndex = index;
            _levelButtonImages[_buttonIndex].color = Color.grey;
            _levelButtons[_buttonIndex].Select();
        }

        if(Input.GetButtonDown("Select"))
        {
            _levelButtons[_buttonIndex].onClick.Invoke();
        }
    }
}
