using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StreetPerformers
{
    public class TestBattleSetup : MonoBehaviour
    {
        [SerializeField]
        private List<string> _enemies = new List<string>();
        [SerializeField]
        private List<int> _enemyLevels = new List<int>();

        [SerializeField]
        private int _aceLevel = 1;
        [SerializeField]
        private List<string> _party = new List<string>();
        [SerializeField]
        private List<int> _partyLevels = new List<int>();

        private void Awake()
        {
            List<EnemyStruct> enemies = new List<EnemyStruct>();
            for(int i = 0; i < _enemies.Count; i++)
            {
                enemies.Add(new EnemyStruct(_enemies[i], _enemyLevels[i]));
            }
            BattleHandler.Instance.SetBattle(enemies);

            PartyHandler.Instance.SetParty(_aceLevel, _party, _partyLevels);

            SceneManager.LoadScene("TestBattleScene");
        }
    }
}