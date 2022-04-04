using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Swaps out the enemy rig based on their level. This is currently only applicable to the generic enemies
    /// like the singer, clown, and musician
    /// </summary>
    public class EnemyRigSwap : MonoBehaviour
    {
        //Struct that holds a rig and its associated level
        [Serializable]
        struct RigLevel
        {
            public int _level;
            public GameObject _rig;
        }

        [Header("Rigs")]
        [SerializeField][Tooltip("List of rigs and their associated level")]
        private List<RigLevel> _rigLevels;

        private void Start()
        {
            //Select the right rig based off the current level of the enemy
            int level = GetComponent<EnemyTurn>().GetLevel();
            SetRig(level);
        }

        public void SetRig(int level)
        {
            RigLevel rigToUse = _rigLevels[0];
            foreach(RigLevel rl in _rigLevels)
            {
                if(level >= rl._level)
                {
                    rigToUse = rl;
                }
                else
                {
                    break;
                }
            }

            Transform visual = transform.Find("Visual");
            Vector3 pos = visual.position;
            Vector3 scale = visual.localScale;

            Destroy(visual.gameObject);

            GameObject rig = Instantiate(rigToUse._rig, transform);
            rig.transform.position = pos;
            rig.transform.localScale = scale;
            rig.name = "Visual";

            //This applies a random speed to the animation so multiple of the same enemies are slightly offset
            rig.GetComponent<Animator>().speed = UnityEngine.Random.Range(.95f, 1f);
        }
    }

}