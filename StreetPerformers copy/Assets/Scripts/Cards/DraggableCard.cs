using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the draggable cards in the player turn. Handles how dragging feels as well
    /// as targetting game objects in the scene.
    /// </summary>
    public class DraggableCard : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //Reference to the RectTransform of this object
        private RectTransform _rect;
        //Original position of this card in the hand
        private Vector3 _originalPos;
        //Original rotation of this card in the hand
        private Quaternion _originalRot;
        //Original scale of this card in the hand
        private Vector3 _originalScale;
        //Offset of card while being held
        private Vector3 _heldOffset;
        //Sibling index for sorting order in the hand
        private int _siblingIndex;
        
        //Reference to the scriptable object for this card
        private CardScriptable _scriptable;

        //Action called when the card is released onto the field
        private UnityAction _activateDelegate;
        //Set to true when this script is active and can be dragged
        [HideInInspector] public bool _active = true;
        //Set to true when this object is being dragged
        private bool _selected = false;
        private bool _hovered = false;

        //Transform to set this to when selected
        private Transform _selectedTrans;
        //Transform to set this to when activated
        private Transform _activatedTrans;
        private Transform _hoverTrans;

        //Handles whether the card is in the active section of the screen or not
        private bool _topHalf = false;

        //Currently selected GameObject
        private List<GameObject> _selectedTargets;
        private GameObject _singleTarget;
        private GameObject _selfTarget = null;

        //List of available targets for this card
        private List<Transform> _targets;
        private GameObject _user = null;
        

        //Reference to the player hand script so only one card is selected at once
        private PlayerHand _hand;

        private List<GameObject> _statusText;

        private Card _cardScript;

        private int _enemyIndex = 0;
        private int _lastDirection = 0;
        private float _bufferTime = .3f;
        private float _timer = 0f;

        private bool _justSelected = false;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _originalPos = _rect.position;
            _originalRot = _rect.rotation;
            _originalScale = _rect.localScale;

            _targets = new List<Transform>();
            _selectedTargets = new List<GameObject>();

            _statusText = new List<GameObject>();
        }

        private void Start()
        {
            _siblingIndex = transform.GetSiblingIndex();

            _cardScript = GetComponent<Card>();
        }

        /// <summary>
        /// Initializes this script with its corresponding scriptable object, user, and targets
        /// </summary>
        /// <param name="scriptable"></param> Scriptable object for this card
        /// <param name="user"></param> GameObject that activated this card
        /// <param name="targetTag"></param> Tag for objects that can be targetted
        public void Initialize(CardScriptable scriptable, GameObject user, string targetTag)
        {
            _scriptable = scriptable;

            _user = user;

            if(_scriptable._canTargetEnemy)
            {
                foreach(GameObject target in GameObject.FindGameObjectsWithTag(targetTag))
                {
                    _targets.Add(target.transform);
                }
            }

            InitializeStatusText();
        }

        /// <summary>
        /// Sets a new transform for this card
        /// </summary>
        /// <param name="newPos"></param> New position to set to
        /// <param name="newRot"></param> New rotation to set to
        public void SetNewPosition(Vector3 newPos, Quaternion newRot, Vector3 newScale)
        {
            _originalPos = newPos;
            _originalRot = newRot;
            _originalScale = newScale;
        }

        /// <summary>
        /// Resets this transform to its original state
        /// </summary>
        public void ResetTransform()
        {
            _rect.position = _originalPos;
            _rect.rotation = _originalRot;
            _rect.localScale = _originalScale;
        }

        /// <summary>
        /// Called every frame while this object is being dragged. Handles the position,
        /// scale, and rotation of the card while being dragged.
        /// </summary>
        /// <param name="pointerEvent"></param>
        public void OnDrag(PointerEventData pointerEvent)
        {
            if(!_active || !_selected)
            { return; }

#if UNITY_STANDALONE
            bool topScreen = Input.mousePosition.y > Screen.height / 3f;
#elif UNITY_IOS || UNITY_ANDROID
                bool topScreen = Input.mousePosition.y > Screen.height / 4f;
#endif

            //If the cursor is in the active section of the screen, sets the card to follow
            //the cursor directly
            if(topScreen)
            {
                //Called when the cursor enters the active section for the first time
                if(!_topHalf)
                {
                    _topHalf = true;
                    transform.DOKill();
                    transform.rotation = _activatedTrans.rotation;
                    transform.DOScale(_activatedTrans.localScale, .5f);

                    //Close status text
                    ToggleStatusText(false);
                }
                transform.DOMove(Input.mousePosition, .25f);
                HighlightClosestTarget();
            }
            else
            {
                //Called when the cursor exits the active section for the first time
                if(_topHalf)
                {
                    _topHalf = false;
                    //transform.DOMove(_selectedTrans.position, .5f);
#if UNITY_STANDALONE
                    transform.DOScale(_hoverTrans.localScale, .5f);
                    _heldOffset = new Vector3(0f, 0f, 0f);
#elif UNITY_IOS || UNITY_ANDROID
                    transform.rotation = _selectedTrans.rotation;
                    transform.DOScale(_selectedTrans.localScale, .5f);
#endif
                    //Open status text
                    ToggleStatusText(true);

                    RemoveTargetHighlight(false);
                    _singleTarget = null;
                    _selfTarget = null;
                    _selectedTargets.Clear();
                }
                Vector2 target = Input.mousePosition + _heldOffset;
                transform.DOMove(target, .25f);
            } 
        }

        private void HighlightTarget(int enemyIndex = 0)
        {
            for (int i = _targets.Count - 1; i >= 0; i--)
            {
                if (_targets[i].GetComponent<CharacterTurn>() == null)
                {
                    _targets.RemoveAt(i);
                }
            }

            if (_scriptable._canTargetSelf)
            {
                HighlightSelf();
            }

            if(_scriptable._hitSingleEnemy)
            {
                HighlightSingleEnemy(_targets[enemyIndex].gameObject);
            }

            if(_scriptable._hitAllEnemies)
            {
                HighlightAllEnemies();
            }
        }

        private void HighlightSelf()
        {
            if (_selfTarget == null)
            {
                _selfTarget = _user;

                if (!_scriptable._hitSingleEnemy)
                {
                    _cardScript.InitializeProjectionTargets();
                    _cardScript.ProjectedActivate();
                    _cardScript.ShowProjection();
                }

                SpriteFlash flash = _selfTarget.AddComponent<SpriteFlash>();
                flash.Initialize(2f);
            }
        }

        private void HighlightSingleEnemy(GameObject enemy)
        {
            //Destroys any active flash script if the closest target has changed
            if (_singleTarget != null && _singleTarget != enemy)
            {
                RemoveTargetHighlight(false);
            }

            //Adds the flash script to the newly selected target
            if (_singleTarget == null || _singleTarget != enemy)
            {
                _singleTarget = enemy;

                _cardScript.SetTarget(_singleTarget);
                _cardScript.InitializeProjectionTargets();
                _cardScript.ProjectedActivate();
                _cardScript.ShowProjection();

                SpriteFlash flash = _singleTarget.AddComponent<SpriteFlash>();
                flash.Initialize(2f);
            }
        }

        private void HighlightAllEnemies()
        {
            _selectedTargets.Clear();
            foreach (Transform target in _targets)
            {
                if (target == null)
                {
                    continue;
                }

                _selectedTargets.Add(target.gameObject);

                if (_scriptable._hitSingleEnemy)
                { continue; }

                SpriteFlash targetFlash = target.GetComponent<SpriteFlash>();
                if (targetFlash == null)
                {
                    _cardScript.SetTarget(_selectedTargets);
                    _cardScript.InitializeProjectionTargets();
                    _cardScript.ProjectedActivate();
                    _cardScript.ShowProjection();

                    targetFlash = target.gameObject.AddComponent<SpriteFlash>();
                    targetFlash.Initialize(2f);
                }
            }
        }

        /// <summary>
        /// Finds the closest available target to the card and starts highlighting it.
        /// </summary>
        private void HighlightClosestTarget()
        {
            for(int i = _targets.Count - 1; i >= 0; i--)
            {
                if(_targets[i].GetComponent<CharacterTurn>() == null)
                {
                    _targets.RemoveAt(i);
                }
            }

            if(_scriptable._canTargetSelf)
            {
                HighlightSelf();
            }

            if(_scriptable._hitSingleEnemy)
            {
                HighlightSingleEnemy(FindClosestEnemy());
            }

            if(_scriptable._hitAllEnemies)
            {
                HighlightAllEnemies();
            }
        }

        private GameObject FindClosestEnemy()
        {
            Vector3 cardPos = Camera.main.ScreenToWorldPoint(transform.position);
            float minDistance = Mathf.Infinity;
            GameObject selectedObject = null;

            //Loops through each target to find the closest
            foreach (Transform target in _targets)
            {
                if (target == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(cardPos, target.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedObject = target.gameObject;
                }
            }

            return selectedObject;
        }

        private void RemoveTargetHighlight(bool cardPlayed = true)
        {
            _cardScript.RemoveProjectionView(cardPlayed);
            _user.GetComponent<CharacterStats>().RemoveProjectionView(cardPlayed);

            if(_selfTarget != null)
            {
                SpriteFlash targetFlash = _selfTarget.GetComponent<SpriteFlash>();
                if(targetFlash != null)
                {
                    targetFlash.Destroy();
                }
            }

            if(_singleTarget != null)
            {
                _singleTarget.GetComponent<CharacterStats>().RemoveProjectionView(cardPlayed);
                SpriteFlash targetFlash = _singleTarget.GetComponent<SpriteFlash>();
                if(targetFlash != null)
                {
                    targetFlash.Destroy();
                }
            }
            
            foreach(GameObject target in _selectedTargets)
            {
                if(target != null)
                {
                    target.GetComponent<CharacterStats>().RemoveProjectionView(cardPlayed);
                    SpriteFlash targetFlash = target.GetComponent<SpriteFlash>();
                    if(targetFlash != null)
                    {
                        targetFlash.Destroy();
                    }
                }
            }
        }

        /// <summary>
        /// Called when the pointer is released after selecting this card
        /// </summary>
        /// <param name="pointerEvent"></param>
        public void OnPointerUp(PointerEventData pointerEvent)
        {
            if(!_active || !_selected)
            { return; }

            //Close status text
#if UNITY_STANDALONE
            ToggleStatusText(true);
#elif UNITY_IOS || UNITY_ANDROID
            ToggleStatusText(false);

            RemoveTargetHighlight();
            transform.SetSiblingIndex(_siblingIndex);
#endif

            _hand._cardSelected = false; 

            //If released in the active side of the screen, sets the card's target and invokes
            //the activate delegate
#if UNITY_STANDALONE
                bool topScreen = Input.mousePosition.y > Screen.height / 3f;
#elif UNITY_IOS || UNITY_ANDROID
                bool topScreen = Input.mousePosition.y > Screen.height / 4f;
#endif
            if(topScreen)
            {
                HighlightClosestTarget();
                RemoveTargetHighlight();
                _activateDelegate?.Invoke();
            }
            else
            {
                transform.DOKill();

#if UNITY_STANDALONE
                if(!_hovered)
                {
                    Dehover();
                }
                else
                {
                    Vector3 targetPos = _originalPos;
                    targetPos.y = _hoverTrans.position.y;
                    transform.DOMove(targetPos, .3f);
                    transform.DOScale(_hoverTrans.localScale, .3f);
                }
#elif UNITY_IOS || UNITY_ANDROID
                transform.DOMove(_originalPos, .3f);
                transform.rotation = _originalRot;
                transform.DOScale(_originalScale, .3f);
#endif

                FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_Retract);
            }

            _selectedTargets.Clear();
            _singleTarget = null;
            _selfTarget = null;
            _selected = false;
        }

        /// <summary>
        /// Called when the pointer is clicked on this object. Starts the change in scale and transform.
        /// </summary>
        /// <param name="pointerEvent"></param>
        public void OnPointerDown(PointerEventData pointerEvent)
        {
            if(!_active || _hand._cardSelected)
            { return; }

#if UNITY_IOS || UNITY_ANDROID
            ToggleStatusText(true);
#endif

            _selected = true;
            _hand._cardSelected = true;

#if UNITY_ANDROID || UNITY_IOS
                _siblingIndex = transform.GetSiblingIndex();
                transform.SetAsLastSibling();

                transform.DOKill();
                Vector2 target = Input.mousePosition + _heldOffset;
                transform.DOMove(target, .25f);
                transform.rotation = _selectedTrans.rotation;
                transform.DOScale(_selectedTrans.localScale, .3f);
#endif

            _heldOffset = transform.position - Input.mousePosition;

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_Pickup);
        }

        /// <summary>
        /// Sets the selected and activated transforms
        /// </summary>
        /// <param name="selectedTrans"></param> Transform for when the card is first selected
        /// <param name="activatedTrans"></param> Transform for when the card is in the active section
        public void SetSelectionPos(Transform selectedTrans, Transform activatedTrans, Transform hoverTrans)
        {
            _selectedTrans = selectedTrans;
            _activatedTrans = activatedTrans;
            _hoverTrans = hoverTrans;

#if UNITY_STANDALONE
                float aspect = 1f;
#elif UNITY_IOS || UNITY_ANDROID
                float normalAspect = .5625f;
                float aspect = Camera.main.aspect;
                if(normalAspect >= aspect)
                {
                    aspect = ((normalAspect + aspect) / 2f) / aspect;
                }
                else
                {
                    aspect = ((normalAspect + aspect) / 2f) / normalAspect;
                } 
#endif

            _heldOffset = new Vector3(0f, 0f, 0f);
            _heldOffset.y = _rect.rect.height * _selectedTrans.localScale.y * aspect;
        }

        /// <summary>
        /// Adds a function to the activate delegate
        /// </summary>
        /// <param name="eventDelegate"></param> Function to add to the delegate
        public void AddActivateListener(UnityAction eventDelegate)
        {
            _activateDelegate += eventDelegate;
        }

        /// <summary>
        /// Toggles the active state of this script
        /// </summary>
        /// <param name="active"></param> Set to true if this script should be active 
        public void ToggleInteractive(bool active)
        {
            _active = active;
        }

        /// <summary>
        /// Sets the _hand equal to the parameter
        /// </summary>
        /// <param name="hand"></param>
        public void SetHand(PlayerHand hand)
        {
            _hand = hand;
        }

        private void InitializeStatusText()
        {
            int index = 0;
            string desc = _scriptable._cardDesc.ToLower();

            if(desc.Contains("poison"))
            {
                SetText("Poison", index);
                index++;
            }
            if(desc.Contains("vulnerable"))
            {
                SetText("Vulnerable", index);
                index++;
            }
            if(desc.Contains("fortify"))
            {
                SetText("Fortify", index);
                index++;
            }
            if(desc.Contains("rage"))
            {
                SetText("Rage", index);
                index++;
            }
            if(desc.Contains("armor"))
            {
                SetText("Armor", index);
                index++;
            }
            if(desc.Contains("burned"))
            {
                SetText("Burned", index);
                index++;
            }
            if(desc.Contains("exhaust"))
            {
                SetText("Exhaust", index);
            }
            if(desc.Contains("debuff"))
            {
                SetText("Debuff", index);
            }
        }

        private void ToggleStatusText(bool on)
        {
            if(on && _cardScript.IsBlind())
            { return; }

            foreach(GameObject status in _statusText)
            {
                status?.SetActive(on);
            }
        }

        private void SetText(string id, int index)
        {
            GameObject text = transform.Find("CardMask").Find(id + "Text").gameObject;
            text.transform.position = transform.Find("CardMask").Find("Status" + index).transform.position;
            _statusText.Add(text);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;

#if UNITY_IOS || UNITY_ANDROID
            return;
#endif

            if(_selected || _hand._cardSelected)
            { return; }

            Hover();
        }

        private void Hover()
        {
            ToggleStatusText(true);

            _cardScript.UpdateOnUseVisual();
            _cardScript.UpdateDoubleUse();

            transform.DOKill();
            Vector3 targetPos = _originalPos;
            targetPos.y = _hoverTrans.position.y;
            transform.DOMove(targetPos, .3f);
            transform.DORotateQuaternion(_hoverTrans.rotation, .3f);
            transform.DOScale(_hoverTrans.localScale, .3f);

            _siblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        }

        public void SetHover(bool hover)
        {
            _hovered = hover;
            if(_hovered)
            {
                Hover();
            }
            else
            {
                Dehover();
            }
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
            _justSelected = selected;
            
            if(_selected)
            {
                transform.DOKill();
                transform.DOMove(_selectedTrans.position, .3f);
                transform.DOScale(_selectedTrans.localScale, .3f);

                ToggleStatusText(false);

                _enemyIndex = 0;
                HighlightTarget(_enemyIndex);
            }
            else
            {
                transform.DOKill();
                transform.DOMove(_originalPos, .3f);
                transform.DOScale(_originalScale, .3f);

                transform.SetSiblingIndex(_siblingIndex);

                FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_Retract);

                _selectedTargets.Clear();
                _singleTarget = null;
                _selfTarget = null;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;

#if UNITY_IOS || UNITY_ANDROID
            return;
#endif
            if(_selected || _hand._cardSelected)
            { return; }

            Dehover();
        }

        private void Dehover()
        {
            ToggleStatusText(false);

            transform.DOKill();
            transform.DOMove(_originalPos, .3f);
            transform.DORotateQuaternion(_originalRot, .3f);
            transform.DOScale(_originalScale, .3f);
            transform.SetSiblingIndex(_siblingIndex);

            _cardScript.DisableOnUseVisual();
            _cardScript.DisableDoubleUse();
        }

        private void Update()
        {
            if(!_active || !_selected || _justSelected)
            {
                _justSelected = false;
                return; 
            }

            if (Input.GetButtonDown("Back") || Mathf.RoundToInt(Input.GetAxis("Vertical")) == -1)
            {
                RemoveTargetHighlight(false);
                _hand.DeselectCard();
                return;
            }

            if(Input.GetButtonDown("Select"))
            {
                HighlightTarget(_enemyIndex);
                RemoveTargetHighlight(false);
                _activateDelegate?.Invoke();

                _selectedTargets.Clear();
                _singleTarget = null;
                _selfTarget = null;
                _selected = false;

                return;
            }

            if(!_scriptable._hitSingleEnemy)
            { return; }

            int input = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
            int index = _enemyIndex;
            if(input == 1)
            {
                if(input != _lastDirection || _timer <= 0f)
                {
                    _lastDirection = input;
                    _timer = _bufferTime;

                    index++;
                    if(index > _targets.Count - 1)
                    {
                      index = 0;
                    }
                }
            }
            else if(input == -1)
            {
                if(input != _lastDirection || _timer <= 0f)
                {
                    _lastDirection = input;
                    _timer = _bufferTime;

                    index--;
                    if(index < 0)
                    {
                      index = _targets.Count - 1;
                    }
                }
            }
            else
            {
                _lastDirection = 0;
            }

            if(index != _enemyIndex)
            {
                _enemyIndex = index;
                HighlightTarget(_enemyIndex);
            }
        }

        public void Discard()
        {
            _hand.CardDiscarded(this);
        }
    }

}
