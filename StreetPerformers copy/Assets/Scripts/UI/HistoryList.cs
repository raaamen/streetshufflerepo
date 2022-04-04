using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class HistoryList : MonoBehaviour
    {
        private struct CardHistory
        {
            public CardScriptable _card;
            public string _name;

            public CardHistory(CardScriptable card, string name)
            {
                _card = card;
                _name = name;
            }
        }

        private List<CardHistory> _usedCards;
        private List<GameObject> _textList;

        private Card _displayCard;
        private bool _displaying = false;
        private bool _animating = false;
        private float _startScale;

        private int _maxCards = 18;

        private int _cardIndex = 0;
        private List<Image> _textBackgroundImages = new List<Image>();

        private int _lastDirection = 0;
        private float _bufferTime = .3f;
        private float _timer = 0f;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if(_usedCards == null)
            {
                _usedCards = new List<CardHistory>();
            }

            if(_textList == null)
            {
                _textList = new List<GameObject>();
                for(int i = 0; i < _maxCards; i++)
                {
                    _textList.Add(transform.Find("Text" + i).gameObject);
                    _textBackgroundImages.Add(_textList[i].transform.Find("Image").GetComponent<Image>());
                }
            }

            if(_displayCard == null)
            {
                _displayCard = transform.Find("Card-View").GetComponent<Card>();
                _startScale = _displayCard.transform.localPosition.x;
            }
        }

        public void Populate()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            Initialize();

            for(int i = 0; i < _usedCards.Count; i++)
            {
                TextMeshProUGUI text = _textList[i].GetComponent<TextMeshProUGUI>();
                if(_usedCards[i]._name == "Ace")
                {
                    text.horizontalAlignment = HorizontalAlignmentOptions.Left;
                }
                else
                {
                    text.horizontalAlignment = HorizontalAlignmentOptions.Right;
                }

                string color = "";
                switch(_usedCards[i]._card._class)
                {
                    case CardScriptable.CardClass.ATTACK:
                        color = "red";
                        break;
                    case CardScriptable.CardClass.DEFENSE:
                        color = "blue";
                        break;
                    case CardScriptable.CardClass.SUPPORT:
                    default:
                        color = "yellow";
                        break;
                }
                text.text = _usedCards[i]._name + " played <color=" + color + ">[" + _usedCards[i]._card._cardName + "]</color>";

                Button button = _textList[i].GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                int index = i;
                button.onClick.AddListener(delegate { DisplayCard(_usedCards[index]._card); });

                DisplayCard(_usedCards[i]._card);
                _cardIndex = i;

                _textBackgroundImages[i].gameObject.SetActive(false);
            }

            if(_usedCards.Count > 0)
            {
                _textBackgroundImages[_cardIndex].gameObject.SetActive(true);
            }

            for(int i = _usedCards.Count; i < _maxCards; i++)
            {
                _textList[i].GetComponent<TextMeshProUGUI>().text = "";
                _textList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        public void AddCard(CardScriptable card, string name)
        {
            Initialize();

            if(_usedCards.Count >= _maxCards)
            {
                _usedCards.RemoveAt(0);
            }
            if(card._adjustable)
            {
                CardScriptable cardCopy = (CardScriptable)ScriptableObject.CreateInstance("CardScriptable");
                cardCopy.CopyCard(card);
                _usedCards.Add(new CardHistory(cardCopy, name));
            }
            else
            {
                _usedCards.Add(new CardHistory(card, name));
            }
        }

        public void DisplayCard(CardScriptable card)
        {
            /*if(_displaying || _animating)
            { return; }*/

            _displaying = true;
            //_animating = true;

            _displayCard.Initialize(card);
            //_displayCard.transform.localScale = new Vector3(0f, 0f, 0f);
            _displayCard.gameObject.SetActive(true);
#if UNITY_STANDALONE
            //_displayCard.transform.DOScale(1.5f, 1.5f).SetUpdate(true).OnComplete(delegate { _animating = false; });
#elif UNITY_IOS || UNITY_ANDROID
            _displayCard.transform.DOScale(2.5f, .5f).SetUpdate(true).OnComplete(delegate { _animating = false; });

#endif
        }

        private void Update()
        {
            int input = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
            int newIndex = _cardIndex;
            if(input == 1)
            {
                //Move up
                if(input != _lastDirection || _timer <= 0f)
                {
                    _lastDirection = input;
                    _timer = _bufferTime;

                    newIndex--;
                    if(newIndex < 0)
                    {
                        newIndex = _usedCards.Count - 1;
                    }
                }
            }
            else if(input == -1)
            {
                //Move down
                if(input != _lastDirection || _timer <= 0f)
                {
                    _lastDirection = input;
                    _timer = _bufferTime;

                    newIndex++;
                    if(newIndex > _usedCards.Count - 1)
                    {
                        newIndex = 0;
                    }
                }
            }
            else
            {
                _lastDirection = 0;
            }

            if(newIndex != _cardIndex)
            {
                _textBackgroundImages[_cardIndex].gameObject.SetActive(false);

                _cardIndex = newIndex;
                _textBackgroundImages[_cardIndex].gameObject.SetActive(true);
                DisplayCard(_usedCards[_cardIndex]._card);
            }

            if(_timer > 0f)
            {
                _timer -= Time.unscaledDeltaTime;
            }
        }

        /*private void Deselect()
        {
            if(_animating)
            { return; }

            _displaying = false;
            _animating = true;

            _displayCard.transform.DOScale(0f, .5f).SetUpdate(true).OnComplete(
                delegate { 
                    _animating = false;
                    _displayCard.gameObject.SetActive(false);
                });
        }

        private void Update()
        {
            if(!_displaying && _displayCard != null)
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
        }*/
    }

}
