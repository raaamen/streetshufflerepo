using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Holds data for which set of enemies to fight next.
    /// </summary>
    public class DemoManager : MonoBehaviour
    {
        [Serializable]
        public enum DemoType
        {
            FULL,
            BATTLE,
            TUTORIAL
        }

        public bool _isDemo = false;
        public DemoType _demoType = DemoType.FULL;

        private List<BattleConfigData> _fullBattles = new List<BattleConfigData>();
        private List<BattleConfigData> _tutorialBattles = new List<BattleConfigData>();
        private List<BattleConfigData> _battleBattles = new List<BattleConfigData>();

        [SerializeField] private Sprite _cornerStoreBackground = null;
        [SerializeField] private Color _cornerStoreColor = Color.white;
        [SerializeField] private Sprite _pierBackground = null;
        [SerializeField] private Color _pierColor = Color.white;
        [SerializeField] private Sprite _subwayBackground = null;
        [SerializeField] private Color _subwayColor = Color.white;
        [SerializeField] private Sprite _alleyBackground = null;
        [SerializeField] private Color _alleyColor = Color.white;

        private void Awake()
        {
            ProgressionHandler progress = ProgressionHandler.Instance;

            progress._isDemo = _isDemo;
            progress._demoType = _demoType;

            _fullBattles.Add(new BattleConfigData("Corner Store", 0, _cornerStoreBackground, null, _cornerStoreColor, true));
            _fullBattles.Add(new BattleConfigData("Pier", 0, _pierBackground, null, _pierColor, false));
            _fullBattles.Add(new BattleConfigData("Pier", 1, _pierBackground, null, _pierColor, false));
            _fullBattles.Add(new BattleConfigData("Subway", 0, _subwayBackground, null, _subwayColor, false));
            _fullBattles.Add(new BattleConfigData("Corner Store", 2, _cornerStoreBackground, null, _cornerStoreColor, false));
            _fullBattles.Add(new BattleConfigData("Subway", 1, _subwayBackground, null, _subwayColor, false));
            _fullBattles.Add(new BattleConfigData("Pier", 2, _pierBackground, null, _pierColor, false));
            _fullBattles.Add(new BattleConfigData("Corner Store", 3, _cornerStoreBackground, null, _cornerStoreColor, false));

            _tutorialBattles.Add(new BattleConfigData("Corner Store", 0, _cornerStoreBackground, null, _cornerStoreColor, true));
            _tutorialBattles.Add(new BattleConfigData("Pier", 0, _pierBackground, null, _pierColor, false));
            _tutorialBattles.Add(new BattleConfigData("Subway", 0, _subwayBackground, null, _subwayColor, false));

            _battleBattles.Add(new BattleConfigData("Graffiti", 6, _alleyBackground, null, _alleyColor, false));

            progress.LoadDemoFullBattles(_fullBattles);
            progress.LoadDemoTutorialBattles(_tutorialBattles);
            progress.LoadDemoBattleBattles(_battleBattles);
        }
    }
}
