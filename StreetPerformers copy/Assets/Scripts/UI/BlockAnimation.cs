using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace StreetPerformers
{
    public class BlockAnimation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _characterImage = null;
        [SerializeField] private RectTransform _characterTrans = null;
        [SerializeField] private TextMeshProUGUI _characterText = null;

        [SerializeField] private RectTransform _startPosition = null;
        [SerializeField] private RectTransform _middlePosition = null;
        [SerializeField] private RectTransform _endPosition = null;

        [Header("Sprites")]
        [SerializeField] private Sprite _contortSprite = null;
        [SerializeField] private Sprite _mascotSprite = null;
        [SerializeField] private Sprite _mimeSprite = null;

        [Header("Text Assets")]
        [SerializeField] private TextAsset _contortText = null;
        [SerializeField] private TextAsset _mascotText = null;
        [SerializeField] private TextAsset _mimeText = null;

        private float _openTime = .25f;
        private float _easeInTime = .75f;
        private float _easeOutTime = .75f;
        private float _closeTime = .25f;

        private string _character = "Mime";

        private void Start()
        {
            //Initialize("Mime");
        }

        public void Initialize(string character)
        {
            _character = character;
            switch (character)
            {
                case "Contortionist":
                    _characterImage.sprite = _contortSprite;
                    _characterText.text = _contortText.text;
                    break;
                case "Mascot":
                    _characterImage.sprite = _mascotSprite;
                    _characterText.text = _mascotText.text;
                    break;
                case "Mime":
                    _characterImage.sprite = _mimeSprite;
                    _characterText.text = _mimeText.text;
                    break;
                default:
                    break;
            }
            _characterImage.SetNativeSize();

            StartCoroutine(PlayBlockAnimation());
        }

        private IEnumerator PlayBlockAnimation()
        {
            yield return new WaitForSeconds(_openTime);

            _characterTrans.position = _startPosition.position;
            _characterTrans.DOMove(_middlePosition.position, _easeInTime).SetEase(Ease.OutQuart);
            yield return new WaitForSeconds(_easeInTime);

            _characterTrans.DOMove(_endPosition.position, _easeOutTime).SetEase(Ease.InQuart);
            yield return new WaitForSeconds(_easeOutTime);

            yield return new WaitForSeconds(_closeTime);

            gameObject.SetActive(false);

            /* switch(_character)
             {
                 case "Contortionist":
                     _character = "Mascot";
                     _characterImage.sprite = _mascotSprite;
                     _characterText.text = _mascotText.text;
                     break;
                 case "Mascot":
                     _character = "Mime";
                     _characterImage.sprite = _mimeSprite;
                     _characterText.text = _mimeText.text;
                     break;
                 case "Mime":
                     _character = "Contortionist";
                     _characterImage.sprite = _contortSprite;
                     _characterText.text = _contortText.text;
                     break;
                 default:
                     break;
             }
             StartCoroutine(PlayBlockAnimation());*/
        }

        public float GetTotalTime()
        {
            return _openTime + _easeInTime + _easeOutTime + _closeTime;
        }
    }

}
