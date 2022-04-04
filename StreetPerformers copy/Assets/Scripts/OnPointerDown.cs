using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnPointerDown : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent _clicked;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _clicked?.Invoke();
    }
}
