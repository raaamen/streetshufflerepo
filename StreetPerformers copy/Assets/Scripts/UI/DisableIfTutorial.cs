using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class DisableIfTutorial : MonoBehaviour
    {
        [SerializeField] private bool _specificTutorialType = false;
        [SerializeField] private DemoManager.DemoType _demoType = DemoManager.DemoType.FULL;

        private void Awake()
        {
            if(ProgressionHandler.Instance._isDemo)
            {
                if(!_specificTutorialType || ProgressionHandler.Instance._demoType == _demoType)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

}