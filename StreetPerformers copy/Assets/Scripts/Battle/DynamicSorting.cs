using UnityEngine;

namespace StreetPerformers
{
    public class DynamicSorting : MonoBehaviour
    {
        //Renderer sorting order at the beginning of battle
        private int _startOrder;
        //Sprite Renderer attached to this game object
        private SpriteRenderer _rend;

        private void Start()
        {
            _rend = GetComponent<SpriteRenderer>();
            _startOrder = _rend.sortingOrder;
        }

        /// <summary>
        /// Set the sorting order equal to the y position times a multiplier.
        /// </summary>
        private void Update()
        {
            _rend.sortingOrder = _startOrder - Mathf.RoundToInt(transform.parent.parent.position.y * 20f);
        }
    }
}

