using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class DeckViewer : MonoBehaviour, IDragHandler
    {
        private Transform _deckPanel;

        private List<Transform> _cards;
        private float _movedAmount = 0f;
        private float _topOffset = 0f;

        private Vector3[] _panelCorners;

        private bool _selecting = false;
        private Transform _centerCard;
        private Transform _selectedCard;
        private Vector3 _originalPosition;
        private Vector3 _originalScale;

        [Header("Character Colors")]
        [SerializeField]
        private Color _aceColor;
        [SerializeField]
        private Color _mimeColor;
        [SerializeField]
        private Color _contortionistColor;
        [SerializeField]
        private Color _mascotColor;

        [Header("Card Backsides")]
        [SerializeField]
        private Sprite _aceBack;
        [SerializeField]
        private Sprite _contortionistBack;
        [SerializeField]
        private Sprite _mascotBack;
        [SerializeField]
        private Sprite _mimeBack;

        public void ToggleStatus()
        {
            if(gameObject.activeInHierarchy)
            {
                foreach(Transform card in _cards)
                {
                    if(card == _selectedCard)
                    {
                        _selectedCard.transform.GetChild(0).Find("Background").gameObject.SetActive(false);
                        card.position = _originalPosition;
                        card.localScale = _originalScale;
                        _selectedCard = null;
                    }
                    Vector2 pos = card.position;
                    pos.y -= _movedAmount;
                    card.position = pos;

                    for(int i = 0; i < card.childCount; i++)
                    {
                        Destroy(card.GetChild(i).gameObject);
                    }
                }
                gameObject.SetActive(false);
                if (!_selecting) 
                {
                    FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);
                }
                

            }
            else
            {
                gameObject.SetActive(true);
                //FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            }
        }

        public void Initialize(string name)
        {
            Image background = GetComponent<Image>();
            switch(name)
            {
                case "Ace":
                    background.color = _aceColor;
                    break;
                case "Contortionist":
                    background.color = _contortionistColor;
                    break;
                case "Mime":
                    background.color = _mimeColor;
                    break;
                case "Mascot":
                    background.color = _mascotColor;
                    break;
                default:
                    break;
            }

            _deckPanel = transform.Find("DeckPanel");
            _centerCard = _deckPanel.Find("Card-View");
            _cards = new List<Transform>();
            _movedAmount = 0f;

            transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name + "'s Cards";
            int level = PartyHandler.Instance._characterLevels[name];

            List<CardScriptable> cards = new List<CardScriptable>();
            List<CardScriptable> faceCards = new List<CardScriptable>();
            Object[] cardList = Resources.LoadAll("ScriptableObjects/" + name + "/Party/");
            foreach(Object card in cardList)
            {
                CardScriptable scriptable = (CardScriptable)card;
                if(level >= scriptable._requiredLevel)
                {
                    if(scriptable._requiredLevel >= 6)
                    {
                        faceCards.Add((CardScriptable)card);
                    }
                    else
                    {
                        cards.Add((CardScriptable)card);
                    }
                }
            }

            for(int i = 0; i < 13; i++)
            {
                Transform cardPos = _deckPanel.Find("Card-" + i);
                Button cardButton = cardPos.GetComponent<Button>();
                cardButton.onClick.RemoveAllListeners();

                if(i < cards.Count)
                {
                    cardPos.GetComponent<Button>().enabled = true;
                    GameObject card = Instantiate(cards[i]._cardPrefab, cardPos);

                    card.GetComponent<Card>().Initialize(cards[i]);
                    card.transform.Find("CardImage").GetComponent<Image>().raycastTarget = false;

                    int index = i;
                    cardButton.enabled = true;
                    cardButton.onClick.AddListener(delegate { Select(index); });
                }
                else if(i - cards.Count < faceCards.Count)
                {
                    cardPos.GetComponent<Button>().enabled = true;
                    GameObject card = Instantiate(faceCards[i - cards.Count]._cardPrefab, cardPos);

                    card.GetComponent<Card>().Initialize(faceCards[i - cards.Count]);
                    card.transform.Find("CardImage").GetComponent<Image>().raycastTarget = false;

                    int index = i;
                    cardButton.enabled = true;
                    cardButton.onClick.AddListener(delegate { Select(index); });
                }
                else
                {
                    if(cardPos.childCount > 0)
                    {
                        Destroy(cardPos.GetChild(0).gameObject);
                    }
                    cardButton.enabled = false;
                    switch(name)
                    {
                        case "Ace":
                            cardPos.GetComponent<Image>().sprite = _aceBack;
                            break;
                        case "Contortionist":
                            cardPos.GetComponent<Image>().sprite = _contortionistBack;
                            break;
                        case "Mascot":
                            cardPos.GetComponent<Image>().sprite = _mascotBack;
                            break;
                        case "Mime":
                            cardPos.GetComponent<Image>().sprite = _mimeBack;
                            break;
                        default:
                            cardPos.GetComponent<Image>().sprite = _aceBack;
                            break;
                    }
                }
                _cards.Add(cardPos);
            }

            _panelCorners = new Vector3[4];
            _deckPanel.GetComponent<RectTransform>().GetWorldCorners(_panelCorners);
            Vector3[] topCorners = new Vector3[4];
            _cards[0].GetComponent<RectTransform>().GetWorldCorners(topCorners);
            _topOffset = _panelCorners[1].y - topCorners[1].y;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(_selecting || _selectedCard != null)
            { return; }

            float dragAmount = eventData.delta.y;

            Vector3[] topCorners = new Vector3[4];
            _cards[0].GetComponent<RectTransform>().GetWorldCorners(topCorners);
            if(topCorners[1].y + dragAmount + _topOffset < _panelCorners[1].y)
            {
                return;
            }

            Vector3[] botCorners = new Vector3[4];
            _cards[_cards.Count - 1].GetComponent<RectTransform>().GetWorldCorners(botCorners);
            if(botCorners[0].y + dragAmount - _topOffset > _panelCorners[0].y)
            {
                return;
            }

            foreach(Transform card in _cards)
            {
                Vector2 pos = card.position;
                pos.y += eventData.delta.y;
                card.position = pos;
            }
            _movedAmount += eventData.delta.y;
        }

        public void Select(int index)
        {
            if(_selecting || _selectedCard != null)
            { return; }

            _selecting = true;
            _originalPosition = _cards[index].position;
            _originalScale = _cards[index].localScale;

            _cards[index].transform.GetChild(0).Find("Background").gameObject.SetActive(true);
            _cards[index].SetAsLastSibling();
            _cards[index].DOMove(_centerCard.position, .5f).SetUpdate(true).OnComplete(delegate { _selecting = false; });
            _cards[index].DOScale(_centerCard.localScale, .5f).SetUpdate(true);

            _selectedCard = _cards[index];

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            return;
        }

        private void Deselect()
        {
            if(_selecting)
            { return; }

            _selecting = true;
            _selectedCard.transform.GetChild(0).Find("Background").gameObject.SetActive(false);
            _selectedCard.DOMove(_originalPosition, .5f).SetUpdate(true).OnComplete(delegate { _selecting = false; });
            _selectedCard.DOScale(_originalScale, .5f).SetUpdate(true);
            _selectedCard = null;

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);

        }

        private void Update()
        {
            if(_selecting || _selectedCard == null)
            { return; }

            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Deselect();
            }

            if(Input.touchCount > 0)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Deselect();
                }
            }
        }
    }
}
