using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    /// <summary>
    /// Handles an individual party member turn's drafting phase.
    /// </summary>
    public class PartyTurn : CharacterTurn
    {
        [Header("Sprite")]
        [SerializeField][Tooltip("Character sprite to display during drafting")]
        private Sprite _characterSpriteDefault;
        [SerializeField] private Sprite _characterSpriteNegative = null;
        [SerializeField] private Sprite _characterSpriteOther = null;
        [SerializeField] private Sprite _characterSpriteMobile = null;

        [Header("Text Assets")]
        [SerializeField] private TextAsset _draftingTutorial = null;
        [SerializeField] private TextAsset _drafting0Text = null;
        [SerializeField] private TextAsset _drafting1Text = null;
        [SerializeField] private TextAsset _drafting2_2Text = null;
        [SerializeField] private TextAsset _drafting2_1Text = null;
        [SerializeField] private TextAsset _exhaustedText = null;
        [SerializeField] private TextAsset _tempExhaustedText = null;
        [SerializeField] private TextAsset _confusedText = null;

        [SerializeField] private GameObject _exhaustedIcon = null;

        //Gap between cards in the deck
        private float _deckCardGap = 20f;

        //List of scriptables being displayed for drafting
        private List<CardScriptable> _scriptablesInHand;
        //Current number of cards in the hand
        private int _numCardsInHand = 2;
        //Maximum base number of cards to choose
        private int _maxCardChoice = 1;
        //Number of cards left to choose
        private int _numCardChoiceLeft;

        [HideInInspector]
        public Vector3 _offScreenPos;      //go to position when not being used
        [HideInInspector]
        public Vector3 _onScreenPos;       //go to when activate card is used

        //List of GameObject cards being displayed for drafting
        private List<GameObject> _cardsInHand;
        
        //Drafting panel transform
        private Transform _draftingPanel;
        //Card panel transform
        private Transform _cardPanel;
        //Selected card transform for deck placement
        private Transform _selectedCardPos;
        private Transform _selectedCardPos2;
        private Transform _discardPos;

        //Reference to the instruction text during drafting phase
        private TextMeshProUGUI _instructionText;

        private bool _exhausted = false;
        private bool _permaExhausted = false;
        public bool _randomChooseCard
        {
            get;
            private set;
        }

        private PlayerTurn _playerTurn;

        private Image _characterImage = null;
        public Sprite _nameSprite = null;

        private List<Button> _cardButtonList = new List<Button>();
        private List<Image> _cardImageList = new List<Image>();
        private int _cardIndex = 0;

        private int _lastDirection = 0;
        private float _bufferTime = .3f;
        private float _timer = 0f;

        private bool _isTurn = false;

        protected override void Awake()
        {
            base.Awake();
            _cardsInHand = new List<GameObject>();

            _draftingPanel = GameObject.Find("Canvas").transform.Find("DraftingPanel").transform.Find("CardPanel");
            _instructionText = _draftingPanel.parent.Find("Textbox").Find("InstructionText")
                .GetComponent<TextMeshProUGUI>();
            _discardPos = _draftingPanel.parent.Find("OutsidePosition");

            _scriptablesInHand = new List<CardScriptable>();
            _selectedCardPos = GameObject.FindGameObjectWithTag("SupportHand").transform;
            _selectedCardPos2 = _selectedCardPos.parent.Find("SelectedCardPos2");

            _randomChooseCard = false;

#if UNITY_STANDALONE
            _deckCardGap = Mathf.Abs(_selectedCardPos.position.x - _selectedCardPos2.position.x);
#endif

            _characterLevel = PartyHandler.Instance._characterLevels[_characterName];
            if(_characterLevel >= 3)
            {
                _numCardsInHand++;
            }
            if(_characterLevel >= 5)
            {
                _maxCardChoice++;
            }
            if(_characterLevel >= 8)
            {
                _numCardsInHand++;
            }

            gameObject.name = _characterName;

            _playerTurn = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTurn>();
        }

        protected void Start()
        {
            GenerateDeck();
        }

        /// <summary>
        /// Called at the start of the party member's turn.
        /// Spawns the cards available for drafting.
        /// </summary>
        public override void StartTurn(bool tutorial = false)
        {
            _isTurn = true;

            _cardsInHand.Clear();

            Transform characterImage = _draftingPanel.parent.Find("CharacterImage");
            _characterImage = characterImage.GetComponent<Image>();
#if UNITY_IOS || UNITY_ANDROID
            SetSprite(_characterSpriteMobile);
#elif UNITY_STANDALONE
            SetSprite(_characterSpriteDefault);
#endif

            if (_exhausted || _permaExhausted)
            {
                _exhausted = false;
                
                if(_permaExhausted)
                {
                    _instructionText.text = _exhaustedText.text;
#if UNITY_STANDALONE
                    SetSprite(_characterSpriteNegative);
#endif
                }
                else
                {
                    _exhaustedIcon.SetActive(false);

                    _instructionText.text = _tempExhaustedText.text;
#if UNITY_STANDALONE
                    SetSprite(_characterSpriteNegative);
#endif
                }
                characterImage.DOMove(_draftingPanel.parent.Find("InsidePosition").position, .5f).OnComplete(delegate { Invoke("EndTurn", 1f); });
                return;
            }

            _numCardChoiceLeft = _maxCardChoice;

            //Choose the number of cards we need
            _scriptablesInHand.Clear();
            for(int i = 0; i < _numCardsInHand; i++)
            {
                if(_deck.Count == 0)
                {
                    if(_discards.Count == 0)
                    {
                        break;
                    }
                    Shuffle();
                }
                int index = Random.Range(0, _deck.Count);
                _scriptablesInHand.Add(_deck[index]);
                _deck.RemoveAt(index);
            }

            //Present them to the player
            SpawnCards();
            characterImage.DOMove(_draftingPanel.parent.Find("InsidePosition").position, .5f);

            if(tutorial && _turnManager._turnNumber == 1)
            {
                _instructionText.text = _draftingTutorial.text;
                SetSprite(_characterSpriteDefault);
            }
            else if(_cardsInHand.Count == 0)
            {
                _instructionText.text = _drafting0Text.text;
#if UNITY_STANDALONE
                SetSprite(_characterSpriteOther);
#endif
                Invoke("EndTurn", 1f);
            }
            else if(_randomChooseCard)
            {
                _instructionText.text = _confusedText.text;
#if UNITY_STANDALONE
                SetSprite(_characterSpriteOther);
#endif
                StartCoroutine(RandomAssignCards());
                
            }
            else if(_numCardChoiceLeft > 1)
            {
                _instructionText.text = _drafting2_2Text.text;
#if UNITY_STANDALONE
                SetSprite(_characterSpriteDefault);
#endif
            }
            else
            {
                if(_maxCardChoice == 1)
                {
                    _instructionText.text = _drafting1Text.text;
#if UNITY_STANDALONE
                    SetSprite(_characterSpriteDefault);
#endif
                }
                else
                {
                    _instructionText.text = _drafting2_1Text.text;
#if UNITY_STANDALONE
                    SetSprite(_characterSpriteDefault);
#endif
                }
            }
        }

        public IEnumerator RandomAssignCards()
        {
            yield return new WaitForSeconds(1f);
            int index = Random.Range(0, _cardsInHand.Count);
            ActivateCard(_cardsInHand[index]);

            yield return new WaitForSeconds(1f);
            if(_numCardChoiceLeft > 0 && _cardsInHand.Count > 0)
            {
                index = Random.Range(0, _cardsInHand.Count);
                ActivateCard(_cardsInHand[index]);
            }
        }

        /// <summary>
        /// Spawns the cards to display for drafting.
        /// </summary>
        private void SpawnCards()
        {
            List<Transform> cardTrans = GetDraftingPositions();

            _cardButtonList.Clear();
            _cardImageList.Clear();

            for(int i = 0; i < _scriptablesInHand.Count; i++)
            {
                int index = i;
                _cardsInHand.Add(Instantiate(_scriptablesInHand[index]._cardPrefab, cardTrans[i]));

                GameObject card = _cardsInHand[index];
                Card cardScr = card.GetComponent<Card>();
                cardScr._backgroundImage.gameObject.SetActive(true);
                cardScr.Initialize(_scriptablesInHand[index]);

                if(_randomChooseCard)
                { continue; }

                Button cardButton = card.GetComponent<Button>();
                cardButton.enabled = true;
                cardButton.onClick.AddListener(delegate { ActivateCard(card); });

                _cardButtonList.Add(cardButton);
                _cardImageList.Add(cardScr._cardImage);
            }

            if(!_randomChooseCard)
            {
                _cardImageList[0].color = Color.grey;
                _cardButtonList[0].Select();
                _cardIndex = 0;
            }
        }

        /// <summary>
        /// Sets the position for each drafting card.
        /// </summary>
        /// <returns></returns>
        private List<Transform> GetDraftingPositions()
        {
            if(_scriptablesInHand.Count == 1)
            {
                _cardPanel = _draftingPanel.Find("1-Card");
            }
            else if(_scriptablesInHand.Count == 2)
            {
                _cardPanel = _draftingPanel.Find("2-Cards");
            }
            else if(_scriptablesInHand.Count == 3)
            {
                _cardPanel = _draftingPanel.Find("3-Cards");
            }
            else
            {
                _cardPanel = _draftingPanel.Find("4-Cards");
            }

            _cardPanel.gameObject.SetActive(true);

            List<Transform> transList = new List<Transform>();
            for(int i = 0; i < _scriptablesInHand.Count; i++)
            {
                transList.Add(_cardPanel.Find("Card" + (i + 1)));
            }
            return transList;
        }

        /// <summary>
        /// Called at the end of the party member's turn. Moves the character image off screen.
        /// </summary>
        public override void EndTurn()
        {
            if(_cardsInHand.Count > 0)
            {
                foreach(GameObject cardInHand in _cardsInHand)
                {
                    cardInHand.transform.DOScale(.45f, .5f);
                    cardInHand.transform.DOMove(_discardPos.position, .5f).OnComplete(delegate { Destroy(cardInHand); });
                }
            }

            Transform characterImage = _draftingPanel.parent.Find("CharacterImage");
            characterImage.DOMove(_draftingPanel.parent.Find("OutsidePosition").position, .5f)
                .OnComplete(delegate{ EndOfAnimation(); });

            _isTurn = false;
            _randomChooseCard = false;
        }

        /// <summary>
        /// Called at the end of the character image move off screen animation.
        /// </summary>
        private void EndOfAnimation()
        {
            _cardPanel.gameObject.SetActive(false);
            _turnManager.EndTurn();
        }

        /// <summary>
        /// Called when the card gets activated. Starts moving the clicked card to the corner deck.
        /// </summary>
        /// <param name="card"></param>
        protected override void ActivateCard(GameObject card)
        {
            int cardIndex = 0;
            for(int i = 0; i < _cardsInHand.Count; i++)
            {
                if(_cardsInHand[i] == card)
                {
                    cardIndex = i;
                }
                _cardImageList[i].color = Color.white;
            }

            _cardButtonList.RemoveAt(cardIndex);
            _cardImageList.RemoveAt(cardIndex);

            if(cardIndex > _cardButtonList.Count - 1)
            {
                cardIndex = 0;
            }

            if(_cardImageList.Count > cardIndex)
            {
                _cardImageList[cardIndex].color = Color.grey;
                _cardIndex = cardIndex;
                _cardButtonList[cardIndex].Select();
            }

            card.GetComponent<Card>()._backgroundImage.gameObject.SetActive(false);

            Vector3 adjustedCardPos = _selectedCardPos.position;
            adjustedCardPos.x +=  _deckCardGap * _playerTurn.GetDeckCount();

            GameObject chosenCard = card;

            chosenCard.transform.SetParent(_selectedCardPos);
            chosenCard.transform.SetAsLastSibling();
            //Destroy(chosenCard.GetComponent<Card>());
            chosenCard.GetComponent<Button>().enabled = false;
            _cardsInHand.Remove(chosenCard);

            _playerTurn.AddCardToHand(card);
            _numCardChoiceLeft--;
            bool end = false;
            if(_numCardChoiceLeft <= 0 || _cardsInHand.Count == 0)
            {
                _randomChooseCard = false;
                end = true;
                foreach(GameObject cardInHand in _cardsInHand)
                {
                    cardInHand.GetComponent<Button>().enabled = false;
                    Card cardScript = cardInHand.GetComponent<Card>();
                    cardScript.DiscardCard(false, "Player");
                    cardScript._cardImage.color = Color.white;
                    BattleHandler.Instance.DiscardCard(cardScript._scriptable);
                }
            }

            if(_cardsInHand.Count == 0)
            {
                _instructionText.text = _drafting0Text.text;
#if UNITY_STANDALONE
                SetSprite(_characterSpriteOther);
#endif
                Invoke("EndTurn", 1f);
            }
            else if(_randomChooseCard)
            {
                _instructionText.text = _confusedText.text;
#if UNITY_STANDALONE
                SetSprite(_characterSpriteOther);
#endif
            }
            else if(_numCardChoiceLeft > 1)
            {
                _instructionText.text = _drafting2_2Text.text;
#if UNITY_STANDALONE
                SetSprite(_characterSpriteDefault);
#endif
            }
            else if(_numCardChoiceLeft == 1)
            {
                if(_maxCardChoice == 1)
                {
                    _instructionText.text = _drafting1Text.text;
#if UNITY_STANDALONE
                    SetSprite(_characterSpriteDefault);
#endif
                }
                else
                {
                    _instructionText.text = _drafting2_1Text.text;
#if UNITY_STANDALONE
                    SetSprite(_characterSpriteDefault);
#endif
                }
            }
            else
            {
                _instructionText.text = "";
            }

            chosenCard.transform.DOMove(adjustedCardPos, .3f).OnComplete(delegate { ChosenCard(end); });
            chosenCard.transform.DOScale(.6f, .3f);
        }

        /// <summary>
        /// Called after the animation dragging the clicked card to the corner deck.
        /// If the card is the last chosen in this party member's phase, ends the turn.
        /// </summary>
        /// <param name="end"></param>
        protected void ChosenCard(bool end)
        {
            if(end)
            {
                EndTurn();
            }
        }

        public void Exhaust()
        {
            _exhausted = true;
            _exhaustedIcon.SetActive(true);
        }

        public void PermaExhaust()
        {
            _permaExhausted = true;
            _exhaustedIcon.SetActive(true);
        }

        public bool IsExhausted()
        {
            return _exhausted || _permaExhausted;
        }

        public void ExhaustionProjectionView()
        {
            _exhaustedIcon.SetActive(true);
        }

        public void RemoveProjectionView(bool cardPlayed = true)
        {
            _exhaustedIcon.SetActive(_exhausted || _permaExhausted);
        }

        public void RandomChooseCard()
        {
            _randomChooseCard = true;
        }

        private void SetSprite(Sprite spr)
        {
            _characterImage.sprite = spr;
            _characterImage.SetNativeSize();
        }

        public float MoveToCenter()
        {
            float moveDuration = 1f;
            transform.DOMove(_onScreenPos, moveDuration);
            return moveDuration;
        }

        public float MoveToSide()
        {
            float moveDuration = 1f;
            transform.DOMove(_offScreenPos, moveDuration);
            return moveDuration;
        }

        private void Update()
        {
            if(!_isTurn)
            { return; }

            int input = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
            int index = _cardIndex;
            if(input == 1)
            {
                if(_lastDirection != input || _timer <= 0f)
                {
                    _lastDirection = input;
                    _timer = _bufferTime;

                    index++;
                    if (index > _cardButtonList.Count - 1)
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
                        index = _cardButtonList.Count - 1;
                    }
                }
            }
            else
            {
                _lastDirection = 0;
            }

            if(index != _cardIndex)
            {
                _cardImageList[_cardIndex].color = Color.white;

                _cardIndex = index;
                _cardImageList[_cardIndex].color = Color.grey;
                _cardButtonList[_cardIndex].Select();
            }

            if(_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }

            if(Input.GetButtonDown("Select") && _numCardChoiceLeft > 0 && !_randomChooseCard)
            {
                _cardButtonList[_cardIndex].onClick.Invoke();
            }
        }
    }

}
