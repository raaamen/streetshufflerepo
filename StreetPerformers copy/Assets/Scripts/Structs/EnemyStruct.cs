namespace StreetPerformers
{
    /// <summary>
    /// This struct contains the information of a single enemy.
    /// It holds the enemy name used for instantiation and the level of the enemy.
    /// </summary>
    [System.Serializable]
    public struct EnemyStruct
    {
        public string _enemyName
        {
            get;
            private set;
        }
        public int _enemyLevel
        {
            get;
            private set;
        }
        
        public EnemyStruct(string enemyName, int enemyLevel)
        {
            _enemyName = enemyName;
            _enemyLevel = enemyLevel;
        }
    }
}
