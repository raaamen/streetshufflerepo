using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class ResetSaves : MonoBehaviour
    {
        void Start()
        {
            ProgressionHandler.Instance.Destroy();
            BattleHandler.Instance.Destroy();
        }

    }
}

