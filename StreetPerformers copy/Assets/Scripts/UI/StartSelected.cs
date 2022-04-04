using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class StartSelected : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private Slider _slider = null;

        private void Start()
        {
            _text.color = Color.grey;
            _slider.Select();
        }
    }
}