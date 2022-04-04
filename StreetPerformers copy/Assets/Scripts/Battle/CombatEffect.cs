namespace StreetPerformers
{
    public class CombatEffect
    {
        public bool _active;

        public bool _resetOnUse;
        public bool _resetOnStart;
        public bool _resetOnEnd;

        public int _value;

        public CombatEffect(bool onUse, bool onStart, bool onEnd)
        {
            _active = false;

            _resetOnUse = onUse;
            _resetOnStart = onStart;
            _resetOnEnd = onEnd;

            _value = 0;
        }

        public void SetStatus(bool active)
        {
            _active = active;
        }
    }
}

