namespace StreetPerformers
{
    /// <summary>
    /// This struct contains the information of a single enemy.
    /// It holds the enemy name used for instantiation and the level of the enemy.
    /// </summary>
    [System.Serializable]
    public struct PartyData
    {
        private int _level;
        public int level
        {
            get { return _level; }
        }

        private int _exp;
        public int exp
        {
            get { return _exp; }
        }

        private int _partyOrder;
        public int partyOrder
        {
            get { return _partyOrder; }
        }

        public PartyData(int level, int exp, int partyOrder)
        {
            _level = level;
            _exp = exp;
            _partyOrder = partyOrder;
        }
    }
}
