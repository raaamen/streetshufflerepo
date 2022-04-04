using System.Collections.Generic;
using TMPro;

namespace StreetPerformers
{
    public enum BonusLevelRequirement
    {
        NONE,
        MAGICIAN_DEFEATED,
        MASCOT_MAX,
        CONTORTIONIST_MAX,
        MIME_MAX
    }
    /// <summary>
    /// Holds information pertaining to a single battle in the game. This includes a list of enemy structs (with name and level),
    /// the before and after cutscenes (if applicable), the completion and attempted state of the battle, and the index in the hub
    /// that this battle is.
    /// </summary>
    [System.Serializable]
    public class BattleData
    {
        //List of enemies in this battle (contains both enemy names and levels)
        public List<EnemyStruct> _enemyList;
        //Level of the hub world required for this battle to appear
        public int _requiredLevel;
        public BonusLevelRequirement _bonusRequirement;


        //Set to true if this battle has already been defeated
        public bool _defeated;
        //Set to true if this battle is still playable (i.e. not a defeated party member battle)
        public bool _playable;
        //Set to true if this battle has been attempted (even if quit out right after)
        public bool _attempted;

        //Contains the string id of the before battle cutscene
        public string _beforeCutscene;
        //Contains the string id of the after battle cutscene
        public string _afterCutscene;

        //The index id of this battle in the hub area it's contained in
        public int _hubIndex;

        public string _musicTrack = "";
        
        public BattleData(int level, EnemyStruct enemy1)
        {
            _bonusRequirement = BonusLevelRequirement.NONE;

            Initialize(level);

            _enemyList = new List<EnemyStruct>()
            {
                enemy1 
            };
        }

        public BattleData(int level, BonusLevelRequirement requirement, EnemyStruct enemy1)
        {
            _bonusRequirement = requirement;

            Initialize(level);

            _enemyList = new List<EnemyStruct>
            {
                enemy1
            };
        }

        public BattleData(int level, EnemyStruct enemy1, EnemyStruct enemy2)
        {
            _bonusRequirement = BonusLevelRequirement.NONE;

            Initialize(level);

            _enemyList = new List<EnemyStruct>()
            { 
                enemy1,
                enemy2
            };
        }

        public BattleData(int level, BonusLevelRequirement requirement, EnemyStruct enemy1, EnemyStruct enemy2)
        {
            _bonusRequirement = requirement;

            Initialize(level);

            _enemyList = new List<EnemyStruct>()
            {
                enemy1,
                enemy2
            };
        }

        public BattleData(int level, EnemyStruct enemy1, EnemyStruct enemy2, EnemyStruct enemy3)
        {
            _bonusRequirement = BonusLevelRequirement.NONE;

            Initialize(level);

            _enemyList = new List<EnemyStruct>()
            {
                enemy1,
                enemy2,
                enemy3
            };
        }

        public BattleData(int level, BonusLevelRequirement requirement, EnemyStruct enemy1, EnemyStruct enemy2, EnemyStruct enemy3)
        {
            _bonusRequirement = requirement;

            Initialize(level);

            _enemyList = new List<EnemyStruct>
            {
                enemy1,
                enemy2,
                enemy3
            };
        }

        private void Initialize(int level)
        {
            _requiredLevel = level;
            _defeated = false;
            _playable = true;
            _attempted = false;
            _hubIndex = 0;
        }

        /// <summary>
        /// Sets the before cutscene equal to the given string
        /// </summary>
        /// <param name="cutscene"></param> String id of the cutscene
        public void AddBeforeCutscene(string cutscene)
        {
            _beforeCutscene = cutscene;
        }

        /// <summary>
        /// Sets the after cutscene equal to the given string
        /// </summary>
        /// <param name="cutscene"></param> String id of the cutscene
        public void AddAfterCutscene(string cutscene)
        {
            _afterCutscene = cutscene;
        }

        public bool BonusRequirementFulfilled()
        {
            switch(_bonusRequirement)
            {
                case BonusLevelRequirement.MAGICIAN_DEFEATED:
                    return ProgressionHandler.Instance.IsAreaCompleted("Magician Tent");
                case BonusLevelRequirement.MASCOT_MAX:
                    return PartyHandler.Instance._partyMembers.Contains("Mascot") && PartyHandler.Instance._characterLevels["Mascot"] >= 10;
                case BonusLevelRequirement.CONTORTIONIST_MAX:
                    return PartyHandler.Instance._partyMembers.Contains("Contortionist") && PartyHandler.Instance._characterLevels["Contortionist"] >= 10;
                case BonusLevelRequirement.MIME_MAX:
                    return PartyHandler.Instance._partyMembers.Contains("Mime") && PartyHandler.Instance._characterLevels["Mime"] >= 10;
            }
            return true;
        }

        public void SetMusicTrack(string musicPath)
        {
            _musicTrack = musicPath;
        }
    }
}
