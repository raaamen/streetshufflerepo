using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightIfSelected : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _text;

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == _slider.gameObject)
        {
            _text.color = Color.grey;
        }
        else
        {
            _text.color = Color.white;
        }
    }
}
