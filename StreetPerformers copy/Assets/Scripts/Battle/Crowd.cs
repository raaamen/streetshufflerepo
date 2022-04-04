using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public enum CrowdPosition
    {
        FRONT,
        MIDDLE,
        BACK,
        ALL
    }

    /// <summary>
    /// Handles the behavior of the crowd in the background of the battle scene
    /// </summary>
    public class Crowd : MonoBehaviour
    {
        [Header("Transforms")]
        [SerializeField][Tooltip("Leftmost position of the crowd")]
        private Transform _leftTrans;
        [SerializeField][Tooltip("Rightmost position of the crowd")]
        private Transform _rightTrans;
        [SerializeField][Tooltip("Center position of the crowd")]
        private Transform _centerTrans;

        [SerializeField] private SpriteRenderer _frontCrowd = null;
        [SerializeField] private SpriteRenderer _middleCrowd = null;
        [SerializeField] private SpriteRenderer _backCrowd = null;

        //Color of the crowd, changes depending on which area they're in
        private Color battleColor;
       
        public void Start()
        {
            battleColor = BattleHandler.Instance._crowdColor;
            foreach(SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            {
                r.color = battleColor;
            }
        }

        /// <summary>
        /// Updates the position and rotation of the crowd based off the given amount.
        /// Lerps between the left and center point from 0-.5
        /// Lerps between the center and right point from .5-1
        /// </summary>
        /// <param name="amount"></param> float from 0-1, where 0 represents the player having 0 health, and 1 represents the enemies having 0 health
        public void UpdateCrowd(float amount)
        {
            amount = Mathf.Clamp(amount, 0f, 1f);

            Vector3 newPos;
            Quaternion newRot;

            if(amount < .5f)
            {
                amount = 2f * amount;
                newPos = Vector3.Lerp(_leftTrans.position, _centerTrans.position, amount);
                newRot = Quaternion.Lerp(_leftTrans.localRotation, _centerTrans.localRotation, amount);
            }
            else
            {
                amount = (amount - .5f) * 2f;
                newPos = Vector3.Lerp(_centerTrans.position, _rightTrans.position, amount);
                newRot = Quaternion.Lerp(_centerTrans.localRotation, _rightTrans.localRotation, amount);
            }

            transform.DOKill();
            transform.DOMove(newPos, .5f);
            transform.DORotateQuaternion(newRot, .5f);
        }

        public void SetAlpha(float alpha, CrowdPosition crowdPosition)
        {
            if(alpha > 1)
            {
                alpha = alpha / 255f;
            }

            Color c = battleColor;
            c.a = alpha;
            switch(crowdPosition)
            {
                case CrowdPosition.FRONT:
                    _frontCrowd.color = c;
                    break;
                case CrowdPosition.MIDDLE:
                    _middleCrowd.color = c;
                    break;
                case CrowdPosition.BACK:
                    _backCrowd.color = c;
                    break;
                case CrowdPosition.ALL:
                    _frontCrowd.color = c;
                    _middleCrowd.color = c;
                    _backCrowd.color = c;
                    break;
            }
        }

        public void LerpAlpha(float alpha, float time, CrowdPosition crowdPosition)
        {
            if (alpha > 1)
            {
                alpha = alpha / 255f;
            }

            Color c = battleColor;
            c.a = alpha;
            switch (crowdPosition)
            {
                case CrowdPosition.FRONT:
                    _frontCrowd.DOColor(c, time);
                    break;
                case CrowdPosition.MIDDLE:
                    _middleCrowd.DOColor(c, time);
                    break;
                case CrowdPosition.BACK:
                    _backCrowd.DOColor(c, time);
                    break;
                case CrowdPosition.ALL:
                    _frontCrowd.DOColor(c, time);
                    _middleCrowd.DOColor(c, time);
                    _backCrowd.DOColor(c, time);
                    break;
            }
        }
    }
}
