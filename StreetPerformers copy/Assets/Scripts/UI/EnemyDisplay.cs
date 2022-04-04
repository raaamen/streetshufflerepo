using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class EnemyDisplay : MonoBehaviour
    {
        [SerializeField] private Image _enemyImage = null;
        [SerializeField] private TextMeshProUGUI _levelText = null;

        public void Initialize(Sprite enemySprite, int enemyLevel, bool encountered)
        {
            _enemyImage.sprite = enemySprite;
            if(encountered)
            {
                _enemyImage.color = Color.white;
            }
            else
            {
                _enemyImage.color = Color.black;
            }
            _enemyImage.SetNativeSize();

            _levelText.text = "Level " + enemyLevel;
        }
    }
}