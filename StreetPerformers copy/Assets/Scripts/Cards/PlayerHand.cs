using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the position, scale, and rotation of cards while in the player's hand.
    /// </summary>
    public class PlayerHand : MonoBehaviour
    {
        [Header("References")]
        [SerializeField][Tooltip("Center position of the player hand")]
        private Transform _cardTrans;
        [SerializeField][Tooltip("Leftmost position of the player hand")]
        private Transform _leftMostCardTrans;
        [SerializeField][Tooltip("Rightmost position of the player hand")]
        private Transform _rightMostCardTrans;
        [SerializeField][Tooltip("Transform of the selected card")]
        private Transform _selectedTrans;
        [SerializeField][Tooltip("Transform of the activated card")]
        private Transform _activatedTrans;
        [SerializeField]
        private Transform _hoverTrans;

        //List of cards in the hand currently
        private List<GameObject> _cards;
        private List<DraggableCard> _draggableCards = new List<DraggableCard>();
        private int _cardIndex = 0;

        //Set to true if a card is currently being selected
        [HideInInspector]
        public bool _cardSelected;
        [HideInInspector]
        public bool _firstCardPlayed = false;

        private int _lastDirection = 0;
        private float _bufferTime = .3f;
        private float _timer = 0f;

        private PlayerTurn _playerTurn = null;

        [SerializeField] private List<GameObject> _disableControlsIfActive = new List<GameObject>();

        private void Awake()
        {
            _playerTurn = GetComponent<PlayerTurn>();
        }

        /// <summary>
        /// Initializes the player hand with the given card list. Sets up the listeners of the
        /// card and fans them out.
        /// </summary>
        /// <param name="cards"></param>
        public void Initialize(List<GameObject> cards)
        {
            _draggableCards.Clear();
            _cards = cards;
            foreach(GameObject card in cards)
            {
                Initialize(card);
            }
            FanOutCards();

            _cardIndex = 0;
            Invoke("SelectFirstCard", 1f);
        }

        private void SelectFirstCard()
        {
            if(_cardIndex == 0 && _draggableCards.Count > 0)
            {
                _draggableCards[_cardIndex].SetHover(true);
            }
        }

        /// <summary>
        /// Initializes the individual card in the player's hand. 
        /// </summary>
        /// <param name="card"></param>
        public void Initialize(GameObject card)
        {
            DraggableCard drag = card.GetComponent<DraggableCard>();
            int index = _draggableCards.Count;
            drag.AddActivateListener(delegate { ActivateCard(drag); });
            drag.SetSelectionPos(_selectedTrans, _activatedTrans, _hoverTrans);
            drag.SetHand(this);
            _draggableCards.Add(drag);
        }

        /// <summary>
        /// Called when a card is released in the active section of the screen.
        /// Refans out the cards
        /// </summary>
        public void ActivateCard(DraggableCard dragCard)
        {
            FanOutCards();
            _firstCardPlayed = true;
            _cardSelected = false;

            for(int i = 0; i < _draggableCards.Count; i++)
            {
                if(_draggableCards[i] == null || _draggableCards[i] == dragCard)
                {
                    _cardIndex = i;
                    _draggableCards.RemoveAt(i);
                }
            }

            ResetCardHover();
        }

        private void ResetCardHover()
        {
            for(int i = 0; i < _draggableCards.Count; i++)
            {
                if(_draggableCards[i] != null)
                {
                    _draggableCards[i].SetHover(false);
                }
            }

            if (_draggableCards.Count > 0)
            {
                if (_cardIndex > _draggableCards.Count - 1)
                {
                    _cardIndex = _draggableCards.Count - 1; ;
                }
                _draggableCards[_cardIndex].SetHover(true);
            }
        }

        public void CardDiscarded(DraggableCard card)
        {
            for(int i = 0; i < _draggableCards.Count; i++)
            {
                if(_draggableCards[i] == null || _draggableCards[i] == card)
                {
                    _draggableCards.RemoveAt(i);
                }
            }

            ResetCardHover();
        }

        /// <summary>
        /// Fans out the cards in the player's hand.
        /// </summary>
        public void FanOutCards()
        {
            if(_cards.Count == 0)
            {
                return;
            }

            #if UNITY_STANDALONE
                float maxGapBetweenCards = Screen.width / 10f;
                float totalTwist = 20f;
            #elif UNITY_IPHONE || UNITY_ANDROID
                float maxGapBetweenCards = Screen.width / 8f;
                float totalTwist = 30f;
            #endif

            float twistPerCard = totalTwist / _cards.Count;

            float scalingFactor = .3f;

            Vector2 startPosition = _cardTrans.position;
            startPosition.x -= ((_cards.Count - 1) / 2f) * maxGapBetweenCards;

            //If the start position is too far to the left, update the gap value so all cards will fit on screen
            if(_leftMostCardTrans.position.x > startPosition.x)
            {
                //startPosition.x = _leftMostCardTrans.position.x;
                maxGapBetweenCards = Mathf.Abs(_leftMostCardTrans.position.x - _rightMostCardTrans.position.x) / (_cards.Count - 1);
                startPosition.x = _cardTrans.position.x - ((_cards.Count - 1) / 2f) * maxGapBetweenCards;
            }

            Vector2 position = startPosition;

            //Loops through each card in your hand and sets their position and rotation
            for(int i = 0; i < _cards.Count; i++)
            {
                float twist = (((_cards.Count - 1) / 2f) - i) * twistPerCard;
                position.y = startPosition.y - (twist * twist) * scalingFactor;

                Quaternion twistQuat = Quaternion.Euler(new Vector3(0f, 0f, twist));

                Vector3 goalScale = new Vector3(1f, 1f, 1f);
                _cards[i].transform.DOMove(position, .5f);
                _cards[i].transform.DOScale(goalScale, .5f);
                _cards[i].transform.DORotateQuaternion(twistQuat, .5f);

                _cards[i].GetComponent<DraggableCard>().SetNewPosition(position, twistQuat, goalScale);

                position.x += maxGapBetweenCards;

                _cards[i].transform.SetAsLastSibling();
            }
        }

        private void Update()
        {
            if(!_playerTurn._isTurn || _cardSelected || _draggableCards.Count == 0)
            { return; }

            foreach(GameObject obj in _disableControlsIfActive)
            {
                if(obj.activeInHierarchy)
                { return; }
            }

            int input = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
            int index = _cardIndex;
            if(input == 1)
            {
                if(_lastDirection != input || _timer <= 0f)
                {
                    _lastDirection = input;
                    _timer = _bufferTime;

                    index++;
                    if(index > _draggableCards.Count - 1)
                    {
                        index = 0;
                    }
                }
            }
            else if(input == -1)
            {
                if(_lastDirection != input || _timer <= 0f)
                {
                    _lastDirection = input;
                    _timer = _bufferTime;

                    index--;
                    if(index < 0)
                    {
                        index = _draggableCards.Count - 1;
                    }
                }
            }
            else
            {
                _lastDirection = 0;
            }

            if(index != _cardIndex)
            {
                if(_draggableCards[_cardIndex] == null)
                {
                    _draggableCards.RemoveAt(_cardIndex);
                    _cardIndex = Mathf.Min(_draggableCards.Count - 1, _cardIndex);
                }
                _draggableCards[_cardIndex]?.SetHover(false);

                _cardIndex = index;
                _draggableCards[_cardIndex]?.SetHover(true);
            }

            if(_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }

            //TODO add up input as well?
            if(Input.GetButtonDown("Select") || Mathf.RoundToInt(Input.GetAxis("Vertical")) == 1)
            {
                if(_draggableCards[_cardIndex] != null && _draggableCards[_cardIndex]._active)
                {
                    _cardSelected = true;
                    _draggableCards[_cardIndex]?.SetSelected(true);
                }
            }
        }

        public void DeselectCard()
        {
            _cardSelected = false;
            _draggableCards[_cardIndex].SetSelected(false);
            _draggableCards[_cardIndex].SetHover(true);
        }
    }
}

