using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class SpriteFlash : MonoBehaviour
    {
        private Color _flashColor;

        private float _flashSpeed;
        private List<SpriteRenderer> _spriteRens;
        //private SpriteRenderer _spriteRen;
        private Color _normalColor;

        private float _flashTimer;
        public void Initialize(float flashSpeed, float flashDuration)
        {
            Initialize(flashSpeed);

            Invoke("Destroy", flashDuration);
        }

        public void Initialize(float flashSpeed)
        {
            _spriteRens = new List<SpriteRenderer>();
            Transform spriteParent = transform.Find("Visual");
            if(spriteParent.childCount == 0)
            {
                _spriteRens.Add(spriteParent.GetComponent<SpriteRenderer>());
            }
            else
            {
                for(int i = 0; i < spriteParent.childCount; i++)
                {
                    SpriteRenderer rend = spriteParent.GetChild(i).GetComponent<SpriteRenderer>();
                    if(rend != null)
                    {
                        _spriteRens.Add(rend);
                    }
                }
            } 

            //_spriteRen = GetComponentInChildren<SpriteRenderer>();
            _normalColor = _spriteRens[0].color;

            _flashSpeed = flashSpeed;
            _flashTimer = 0f;

            _flashColor = new Color(.5f, .5f, .5f);
        }

        public void Destroy()
        {
            foreach(SpriteRenderer rend in _spriteRens)
            {
                rend.color = _normalColor;
            }
            //_spriteRen.color = _normalColor;
       
            Destroy(this);
        }

        private void Update()
        {
            _flashTimer += _flashSpeed * Time.deltaTime;
            float x = ((2f * _flashTimer) + 3f) * (Mathf.PI / 2f);
            Color lerpedColor = Color.Lerp(_normalColor, _flashColor, (Mathf.Sin(x) + 1f) / 2f);
            foreach(SpriteRenderer rend in _spriteRens)
            {
                rend.color = lerpedColor;
            }
            //_spriteRen.color = Color.Lerp(_normalColor, _flashColor, (Mathf.Sin(x) + 1f) / 2f);
        }
    }

}
