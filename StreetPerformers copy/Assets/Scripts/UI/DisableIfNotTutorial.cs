using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class DisableIfNotTutorial : MonoBehaviour
    {
        private void Awake()
        {
            if(!ProgressionHandler.Instance._isDemo)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

}