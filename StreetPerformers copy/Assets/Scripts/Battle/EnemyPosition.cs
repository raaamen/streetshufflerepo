using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class EnemyPosition : MonoBehaviour
    {
        //Starting spot for all enemies off screen
        public Transform _spawnPos;
        [SerializeField]
        private List<Transform> _spawnPosList;

        //List of positions when there are 3 enemies
        private List<Transform> _position3;
        //List of positions when there are 2 enemies
        private List<Transform> _position2;
        //List of positions when there is 1 enemy
        private List<Transform> _position1;

        private int _deadIndex = 0;

        private List<GameObject> _deadEnemies = new List<GameObject>();

        private void Awake()
        {
            PopulateList(ref _position1, 1);
            PopulateList(ref _position2, 2);
            PopulateList(ref _position3, 3);
        }

        /// <summary>
        /// Finds all the positions for the given index and adds them to the positionList
        /// </summary>
        /// <param name="positionList"></param> Empty list to populate
        /// <param name="index"></param> Number of transforms to add to the list
        private void PopulateList(ref List<Transform> positionList, int index)
        {
            positionList = new List<Transform>();
            Transform pos = transform.Find(index + "-Enemy");
            for(int i = 0; i < index; i++)
            {
                positionList.Add(pos.Find("Enemy" + (i + 1)));
            }
        }

        /// <summary>
        /// Checks the number of enemies passed in and calls the SetSpawn function using the
        /// positionList that matches the number of enemies left.
        /// </summary>
        /// <param name="enemies"></param> List of enemies still alive in the game
        public void SetSpawn(List<GameObject> enemies)
        {
            if(enemies.Count == 3)
            {
                SetSpawn(enemies, _spawnPosList);
            }
            else if(enemies.Count == 2)
            {
                SetSpawn(enemies, _spawnPosList);
            }
            else if(enemies.Count == 1)
            {
                SetSpawn(enemies, _spawnPosList);
            }
        }

        public void MoveSpawn(List<GameObject> enemies, float time)
        {
            if(enemies.Count == 3)
            {
                MoveSpawn(enemies, _spawnPosList, time);
            }
            else if(enemies.Count == 2)
            {
                MoveSpawn(enemies, _spawnPosList, time);
            }
            else if(enemies.Count == 1)
            {
                MoveSpawn(enemies, _spawnPosList, time);
            }
        }

        public Vector3 GetSpawnPos(int index)
        {
#if UNITY_STANDALONE
            return _spawnPosList[index].position;
#elif UNITY_IOS || UNITY_ANDROID
            return _spawnPos.position;
#endif
        }

        /// <summary>
        /// Sets the beginning position off screen for all enemies
        /// </summary>
        /// <param name="enemies"></param> List of enemies to set
        /// <param name="positions"></param> List of positions to set the enemies at
        private void SetSpawn(List<GameObject> enemies, List<Transform> positions)
        {
            for(int i = 0; i < enemies.Count; i++)
            {
#if UNITY_STANDALONE
                enemies[i].transform.position = positions[i].position;
#elif UNITY_IOS || UNITY_ANDROID
                enemies[i].transform.position = new Vector3(_spawnPos.position.x, positions[i].position.y, 0f);
#endif
            }
        }

        public void MoveSpawn(List<GameObject> enemies, List<Transform> positions, float time)
        {
            for(int i = 0; i < enemies.Count; i++)
            {
                enemies[i].transform.DOMove(positions[i].position, time);
            }
        }

        /// <summary>
        /// Moves the given enemy offscreen before destroying it.
        /// </summary>
        /// <param name="enemy"></param> Enemy to move
        public void MoveOffScreen(GameObject enemy)
        {
            enemy.transform.DOKill();
            _deadEnemies.Add(enemy);
#if UNITY_STANDALONE
            enemy.transform.DOMove(_spawnPosList[2 - _deadIndex].position, Random.Range(.7f, 1.2f));
            _deadIndex++;
#elif UNITY_IOS || UNITY_ANDROID
            Vector3 offScreenPos = new Vector3(_spawnPos.position.x, enemy.transform.position.y, 0f);
            enemy.transform.DOMove(offScreenPos, Random.Range(.7f, 1.2f)).OnComplete(
                delegate
                {
                    Destroy(enemy);
                });
#endif
        }

        /// <summary>
        /// Checks how many enemies in the list are not null, and calls the MoveToBattlePos function on the non-null list of enemies
        /// </summary>
        /// <param name="enemies"></param> List of enemies
        /// <returns></returns> Max time it takes to move enemies into position
        public float MoveEnemies(List<GameObject> enemies)
        {
            //Filter out any null enemies and fill the enemiesNoNull list with the remaining
            List<GameObject> enemiesNoNull = new List<GameObject>();
            for(int i = 0; i < enemies.Count; i++)
            {
                GameObject enemy = enemies[i];

                if(enemy == null)
                { continue; }

                enemiesNoNull.Add(enemy);
            }

            //Depending on the number of non-null enemies, move into the corresponding position
            float time = 0f;
            if(enemiesNoNull.Count == 3)
            {
                time = MoveToBattlePos(enemiesNoNull, _position3);
            }
            else if(enemiesNoNull.Count == 2)
            {
                time = MoveToBattlePos(enemiesNoNull, _position2);
            }
            else if(enemiesNoNull.Count == 1)
            {
                time = MoveToBattlePos(enemiesNoNull, _position1);
            }
            return time;
        }

        /// <summary>
        /// Moves the enemies in the given list into their corresponding positions
        /// </summary>
        /// <param name="enemies"></param> List of enemies (non-null)
        /// <param name="positions"></param> List of positions for those enemies
        /// <returns></returns> Max time it takes to move the enemies into position
        public float MoveToBattlePos(List<GameObject> enemies, List<Transform> positions)
        {
            float time = 0f;
            for(int i = 0; i < enemies.Count; i++)
            {
                if(_deadEnemies.Contains(enemies[i]))
                { continue; }

                float randTime = Random.Range(.75f, 1.5f);
                enemies[i].transform.DOMove(positions[i].position, randTime);

                if(randTime > time)
                {
                    time = randTime;
                }
            }
            return time;
        }
    }
}
