using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading;

public class CardBurn : MonoBehaviour
{
    private RectMask2D _cardMask = null;
    private RectTransform _burnTransform = null;

    private bool _burnComplete = false;

    public void Initialize(RectMask2D cardMask, RectTransform burnTransform)
    {
        _cardMask = cardMask;
        _burnTransform = burnTransform;

        _burnTransform.gameObject.SetActive(true);

        StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        Vector4 paddingVector = _cardMask.padding;
        RectTransform maskTransform = _cardMask.GetComponent<RectTransform>();

        float totalPaddingChange = 200f * maskTransform.lossyScale.y;
        float paddingRate = 2f * maskTransform.lossyScale.y;
        float paddingChange = 0;

        Vector2 burnPos = _burnTransform.localPosition;
        float burnStartY = _burnTransform.localPosition.y;
        float burnTargetY = 110f * _burnTransform.lossyScale.y;


        while(_cardMask.padding.y < totalPaddingChange)
        {
            //Debug.Log(_cardMask.padding.y);
            paddingVector.y += paddingRate;
            _cardMask.padding = paddingVector;
            paddingChange += paddingRate;

            burnPos.y = Mathf.Lerp(burnStartY, burnTargetY, paddingChange / totalPaddingChange);
            _burnTransform.localPosition = burnPos;
            yield return new WaitForEndOfFrame();
        }

        _burnComplete = true;

        Destroy(this.gameObject);
    }

    public bool BurnComplete()
    {
        return _burnComplete;
    }
}
