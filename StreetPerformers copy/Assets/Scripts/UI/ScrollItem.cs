using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class ScrollItem : MonoBehaviour
    {
        protected int _scrollIndex;
        protected ScrollBox _scrollBox;

        public virtual void Initialize(int index, ScrollBox box)
        {
            _scrollIndex = index;
            _scrollBox = box;
        }

        public virtual void ActivateToggle()
        {

        }
    }
}