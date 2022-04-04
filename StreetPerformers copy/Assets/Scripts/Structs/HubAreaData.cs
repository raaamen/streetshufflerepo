using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StreetPerformers
{
    /// <summary>
    /// Contains information for the hub areas on the hubworld scene.
    /// This includes the list of all battles in the area, it's completion status, whether it was just recently
    /// unlocked (for animation purposes), and a string id of the hub area unlocked by this area (if applicable)
    /// </summary>
    [System.Serializable]
    public class HubAreaData
    {
        //List of the battles that occur in this hub area
        public List<BattleData> _battles;
        //Set to true if this area is playable
        public bool _active;
        public int _nonBonusBattleCount = 0;

        //Current battle level of this area
        public int _battleLevel;
        //String id of the next area unlocked after completing this area (if one exists)
        public string _nextArea;
        //Set to true if this area was just unlocked by the most recent battle completion. Is used for the newly unlocked animation
        public bool _justUnlocked = false;
        
        //Event called when this area completes all its battles for the first time
        public UnityAction OnComplete;

        public HubAreaData(bool active, int battleLevel, int nonBonusBattleCount)
        {
            _battles = new List<BattleData>();
            _active = active;
            _battleLevel = battleLevel;
            _justUnlocked = false;
            _nonBonusBattleCount = nonBonusBattleCount;
        }

        public HubAreaData(bool active, int battleLevel, string nextArea, int nonBonusBattleCount)
        {
            _battles = new List<BattleData>();
            _active = active;
            _battleLevel = battleLevel;

            _nextArea = nextArea;
            _justUnlocked = false;
            _nonBonusBattleCount = nonBonusBattleCount;
        }

        /// <summary>
        /// Adds the given cutscene before the battle at the given index
        /// </summary>
        /// <param name="index"></param> Index of the battle the cutscene will be placed before
        /// <param name="cutscene"></param> String id of the cutscene to play
        public void AddBeforeCutscene(int index, string cutscene)
        {

            _battles[index].AddBeforeCutscene(cutscene);
        }

        /// <summary>
        /// Adds the given cutscene after the battle at the given index
        /// </summary>
        /// <param name="index"></param> Index of the battle the cutscene will be placed after
        /// <param name="cutscene"></param> String id of the cutscene to play
        public void AddAfterCutscene(int index, string cutscene)
        {
            _battles[index].AddAfterCutscene(cutscene);
        }

        /// <summary>
        /// Called after completing a new battle in this area. Increases battle level and checks if this area has all of its
        /// battles completed.
        /// </summary>
        public void LevelUp()
        {
            _battleLevel++;
            if(_battleLevel >= _nonBonusBattleCount)
            {
                OnComplete?.Invoke();
            }
        }
       
    }
}
