using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class HubBackground : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField]
        private GameObject _aceIcon;
        private RectTransform _aceRect = null;

        private Vector3 _dragOffset;
        private RectTransform _rect;
        private Vector3 _startPos;

        private bool _active = true;
        private bool _hasOffset = false;

        [HideInInspector]
        public bool _menuOpen = false;

        [SerializeField] private List<HubArea> _hubs = new List<HubArea>();

        private void Awake()
        {
            _aceRect = _aceIcon.GetComponent<RectTransform>();

            AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();
            fitter.enabled = false;
            fitter.enabled = true;
        }
        private void Start()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!_active)
            { return; }

            if(!_hasOffset)
            {
                _dragOffset = _rect.position - Input.mousePosition;
            }

            Vector3 dragPos = Input.mousePosition + _dragOffset;

            //MoveTowards(dragPos, 0f);
        }

        public bool MoveAceTowards(Vector3 pos, float time)
        {
            if(Vector3.Distance(pos, _aceIcon.transform.position) <= .1f)
            {
                return false;
            }

            _active = false;
            float beginScale = _aceIcon.transform.localScale.x;
            Sequence seq = DOTween.Sequence();
            seq.Append(_aceIcon.transform.DOScale(.5f * beginScale, time / 3f)).Append(_aceIcon.transform.DOMove(pos, time / 3f))
                .Append(_aceIcon.transform.DOScale(beginScale, time / 3f)).OnComplete(delegate { _active = true; });
            return true;
        }

        public float DistanceToAce(Vector3 pos)
        {
            return Vector3.Distance(pos, _aceIcon.transform.position);
        }

        public void SetAce(Vector3 pos)
        {
            _aceIcon.transform.position = pos;
        }

        public Vector3 MoveTowards(Vector3 pos, float time)
        {
            Vector3[] rectCorners = new Vector3[4];
            _rect.GetWorldCorners(rectCorners);

            Vector2 rectDim = (rectCorners[2] - rectCorners[0]) / 2f;
            rectDim.x = Mathf.Abs(rectDim.x);

            pos.x = Mathf.Clamp(pos.x, Screen.width - rectDim.x, rectDim.x);
            pos.y = Mathf.Clamp(pos.y, Screen.height - rectDim.y, rectDim.y);

            Vector3 moveVec = pos - _rect.position;

            //_rect.DOMove(pos, time);

            return moveVec;
        }

        public void SetPosition(Vector3 pos)
        {
            MoveTowards(pos, 0f);
            _startPos = pos;
        }

        public void ResetPosition()
        {
            MoveTowards(_startPos, 1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!_active)
            { return; }

            _hasOffset = true;
            _dragOffset = _rect.position - Input.mousePosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _hasOffset = false;
        }

        public void SetActiveStatus(bool active)
        {
            _active = active;
        }

        public bool IsActive()
        {
            return _active;
        }

        public Vector2 GetAcePos()
        {
            return _aceIcon.transform.position;
        }

        public void ResetContainsAce()
        {
            foreach(HubArea hub in _hubs)
            {
                hub.ResetContainsAce();
            }
        }
    }
}

