using StreetPerformers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollBox : MonoBehaviour
{
    [SerializeField]
    private GameObject _container;
    [SerializeField]
    private Scrollbar _scrollBar;
    [SerializeField]
    private string _scrollObjectTag;
    [SerializeField]
    private float _bottomPadding = 5f;

    public UnityEvent _enterPressed;
    public UnityAction<int> _buttonSelected;

    private GameObject _selectHighlight;
    private List<GameObject> _buttonObjects;

    private Vector3 _endScrollPos;
    private Vector3 _beginScrollPos;

    private float _curScroll = 0f;
    private float _totalScrollAmount = 0f;

    private int _curScrollIndex = 0;
    private int _numButtonsOnScreen = 0;

    private float _rectTop = 0;
    private float _rectBot = 0;

    private bool _ignoreScrollBarChange = false;

    private float _bufferTime = .3f;
    private int _lastDirection = 0;
    private float _timer = 0f;

    private bool _enabled = true;

    private void Awake()
    {
        _selectHighlight = _container.transform.GetChild(0).gameObject;
        _beginScrollPos = _container.transform.position;
        _endScrollPos = _container.transform.position;

        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        _rectTop = corners[1].y;
        _rectBot = corners[0].y;

        float topScrollEnd = _rectBot + _bottomPadding;

        _buttonObjects = new List<GameObject>();
        int buttonIndex = 0;
        for(int i = 0; i < _container.transform.childCount; i++)
        {
            GameObject obj = _container.transform.GetChild(i).gameObject;
            
            if(!obj.CompareTag(_scrollObjectTag))
            { continue; }

            _buttonObjects.Add(obj);
            obj.GetComponent<ScrollItem>().Initialize(buttonIndex, this);

            obj.GetComponent<RectTransform>().GetWorldCorners(corners);
            if(corners[0].y >= _rectBot)
            {
                _numButtonsOnScreen++;
            }

            buttonIndex++;
        }

        if(_buttonObjects.Count <= 1)
        { return; }

        _beginScrollPos = _container.transform.position;

        if(_numButtonsOnScreen >= _buttonObjects.Count)
        {
            _endScrollPos = _container.transform.position;
            return;
        }

        _buttonObjects[_buttonObjects.Count - 1].GetComponent<RectTransform>().GetWorldCorners(corners);
        float botScrollEnd = corners[0].y;

        _totalScrollAmount = Mathf.Abs(topScrollEnd - botScrollEnd);
        _endScrollPos = new Vector3(_beginScrollPos.x, _beginScrollPos.y + _totalScrollAmount);
    }

    public void OnScrollBar()
    {
        if(_ignoreScrollBarChange)
        { return; }

        _curScroll = Mathf.Clamp(_scrollBar.value * _totalScrollAmount, 0f, _totalScrollAmount);

        Vector3[] corners = new Vector3[4];
        _buttonObjects[_curScrollIndex].GetComponent<RectTransform>().GetWorldCorners(corners);

        if(corners[1].y > _rectTop)
        {
            MoveHighlight(1);
        }
        else if(corners[0].y < _rectBot)
        {
            MoveHighlight(-1);
        }

        UpdateScroll();
    }

    private void UpdateScroll()
    {
        _container.transform.position = Vector3.Lerp(_beginScrollPos, _endScrollPos, _curScroll / _totalScrollAmount);
    }

    public void SetHighlight(int index)
    {
        _curScrollIndex = index;
        _selectHighlight.transform.position = _buttonObjects[index].transform.position;

        Vector3[] corners = new Vector3[4];
        RectTransform rect = _buttonObjects[_curScrollIndex].GetComponent<RectTransform>();
        rect.GetWorldCorners(corners);
        if(IsOutOfBox(true, corners))
        {
            if(_curScrollIndex == 0)
            {
                _curScroll = 0;
            }
            else
            {
                float targetTop = _rectTop - _bottomPadding;
                float deltaScroll = Mathf.Abs(targetTop - corners[1].y);
                _curScroll -= deltaScroll;
            }
        }
        else if(IsOutOfBox(false, corners))
        {
            if(_curScrollIndex == _buttonObjects.Count - 1)
            {
                _curScroll = _totalScrollAmount;
            }
            else
            {
                float targetBot = _rectBot + _bottomPadding;
                float deltaScroll = Mathf.Abs(targetBot - corners[0].y);
                _curScroll += deltaScroll;
            }
        }

        _ignoreScrollBarChange = true;
        _scrollBar.value = _curScroll / _totalScrollAmount;
        _ignoreScrollBarChange = false;

        UpdateScroll();
    }

    private void Update()
    {
        if(!_enabled)
        { return; }

        float scroll = Input.mouseScrollDelta.y;
        if(scroll < 0)
        {
            MoveDownOne();
        }
        else if(scroll > 0)
        {
            MoveUpOne();
        }
        else
        {
            int index = _curScrollIndex;

            int moveValue = -Mathf.RoundToInt(Input.GetAxis("Vertical"));

            if (moveValue == 0)
            {
                _lastDirection = 0;
            }
            else if (moveValue != _lastDirection || _timer <= 0f)
            {
                _timer = _bufferTime;
                _lastDirection = moveValue;
                index += moveValue;
            }

            if(index > _curScrollIndex)
            {
                MoveDownOne();
            }
            else if(index < _curScrollIndex)
            {
                MoveUpOne();
            }
        }

        if(_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Toggle"))
        {
            _buttonObjects[_curScrollIndex].GetComponent<ScrollItem>().ActivateToggle();
        }
    }

    private void MoveDownOne()
    {
        if(_curScrollIndex >= _buttonObjects.Count - 1)
        { return; }

        MoveHighlight(1);

        Vector3[] corners = new Vector3[4];
        _buttonObjects[_curScrollIndex].GetComponent<RectTransform>().GetWorldCorners(corners);

        if(IsOutOfBox(false, corners))
        {
            if(_curScrollIndex == _buttonObjects.Count - 1)
            {
                _curScroll = _totalScrollAmount;
            }
            else
            {
                float targetBot = _rectBot + _bottomPadding;
                float deltaScroll = Mathf.Abs(targetBot - corners[0].y);
                _curScroll += deltaScroll;
            }

            _ignoreScrollBarChange = true;
            _scrollBar.value = _curScroll / _totalScrollAmount;
            _ignoreScrollBarChange = false;

            UpdateScroll();
        }
    }

    private void MoveUpOne()
    {
        if(_curScrollIndex <= 0)
        { return; }

        MoveHighlight(-1);

        Vector3[] corners = new Vector3[4];
        _buttonObjects[_curScrollIndex].GetComponent<RectTransform>().GetWorldCorners(corners);

        if(IsOutOfBox(true, corners))
        {
            if(_curScrollIndex == 0)
            {
                _curScroll = 0;
            }
            else
            {
                float targetTop = _rectTop - _bottomPadding;
                float deltaScroll = Mathf.Abs(targetTop - corners[1].y);
                _curScroll -= deltaScroll;
            }

            _ignoreScrollBarChange = true;
            _scrollBar.value = _curScroll / _totalScrollAmount;
            _ignoreScrollBarChange = false;

            UpdateScroll();
        }
    }

    private void MoveHighlight(int dir)
    {
        _curScrollIndex += dir;
        _curScrollIndex = Mathf.Clamp(_curScrollIndex, 0, _buttonObjects.Count - 1);

        SetHighlight();
    }

    private void SetHighlight()
    {
        _selectHighlight.transform.position = _buttonObjects[_curScrollIndex].transform.position;
        _buttonSelected?.Invoke(_curScrollIndex);
    }

    private bool IsOutOfBox(bool above, Vector3[] corners)
    {
        if(above)
        {
            return corners[1].y > _rectTop;
        }
        else
        {
            return corners[0].y < _rectBot;
        }
    }

    public void ToggleEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public IEnumerator ToggleEnabledEndOfFrame(bool enabled)
    {
        yield return new WaitForEndOfFrame();

        _enabled = enabled;
    }
}
