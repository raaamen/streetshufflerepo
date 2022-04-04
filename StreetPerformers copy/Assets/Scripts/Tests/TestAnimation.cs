using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimation : MonoBehaviour
{
    public Sprite _sprite;
    public Sprite _default;
    private void Start()
    {
        transform.DOMove(new Vector3(0, 1, 0), 1f).OnComplete(Buffer);
    }

    private void Buffer()
    {
        Invoke("SwapSprite", .5f);
    }

    private void SwapSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = _sprite;
        Invoke("SwapBack", 1f);
    }

    private void SwapBack()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = _default;
        Invoke("MoveBack", 1f);
    }

    private void MoveBack()
    {
        transform.DOMove(new Vector3(-7, 1, 0), 1f);
    }
}
