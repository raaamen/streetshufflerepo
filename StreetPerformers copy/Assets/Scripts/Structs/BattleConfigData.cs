using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// This struct contains the information of a single enemy.
    /// It holds the enemy name used for instantiation and the level of the enemy.
    /// </summary>
    [System.Serializable]
    public struct BattleConfigData
    {
        private string _hubName;
        private int _battleIndex;
        private Sprite _background;
        private Sprite _frontground;
        private Color _crowdColor;
        private bool _preBattleAnimation;

        public BattleConfigData(string hubName, int battleIndex, Sprite background, Sprite frontground, Color crowdColor, bool preBattleAnimation)
        {
            _hubName = hubName;
            _battleIndex = battleIndex;
            _background = background;
            _frontground = frontground;
            _crowdColor = crowdColor;
            _preBattleAnimation = preBattleAnimation;
        }

        public void SetBattle()
        {
            BattleHandler.Instance.SetBattle(_hubName, _battleIndex, _background, _frontground, _crowdColor, _preBattleAnimation);
        }
    }
}
