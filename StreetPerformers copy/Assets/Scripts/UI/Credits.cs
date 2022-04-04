using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the scrolling of credits.
    /// </summary>
    public class Credits : MonoBehaviour
    {
        [Header("References")]
        [SerializeField][Tooltip("The object containing the credits")]
        private Transform _creditContents;

        [Header("Values")]
        [SerializeField][Tooltip("Speed of credit scrolling")]
        private float _moveSpeed;

        //Set to true while credits are scrolling
        private bool _active = false;

        /// <summary>
        /// Resets the position of the credits and starts the scrolling action
        /// </summary>
        public void StartScrolling()
        {
            _active = true;
            Vector2 pos = _creditContents.position;
            pos.y = 0f;
            _creditContents.position = pos;
        }

        /// <summary>
        /// Stops the scrolling action
        /// </summary>
        public void EndScrolling()
        {
            _active = false;
        }

        /// <summary>
        /// While active, moves the credits down
        /// </summary>
        private void Update()
        {
            if(_active)
            {
                Vector2 pos = _creditContents.position;
                pos.y += _moveSpeed * Time.deltaTime;
                _creditContents.position = pos;
            }
        }
    }

}
