using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StreetPerformers
{
    /// <summary>
    /// Holds a list of party members, their levels, and who is currently in the active party.
    /// </summary>
    [System.Serializable]
    public class PartyHandler : Singleton<PartyHandler>
    {
        //Dictionary mapping character names to their levels
        [HideInInspector]
        public Dictionary<string, int> _characterLevels;
        [HideInInspector]
        public Dictionary<string, int> _characterExp;
        //Create a dictionary of a list of upgraded cards. Upgraded cards then pull from a separate folder and are replaced upon instantiation
        [HideInInspector]
        public List<string> _upgradedCards = new List<string>();

        //List of all party members
        [HideInInspector]
        public List<string> _partyMembers;
        [HideInInspector]
        public List<string> _excludedPartyMembers = new List<string>();

        [HideInInspector]
        public int _addedMaxHealth;

        public void LoadData(LoadFile saveFile)
        {
            _partyMembers = new List<string>();
            if(saveFile != null && saveFile.GetLevel().ContainsKey("Ace"))
            {
                _characterLevels = saveFile.GetLevel();
                _characterExp = saveFile.GetExp();
                _partyMembers = saveFile.GetMembers();
                _addedMaxHealth = saveFile._addedMaxHealth;
                _upgradedCards = saveFile.GetUpgradedCards();
            }
            else
            {
                _characterLevels = new Dictionary<string, int>();
                _characterLevels.Add("Ace", 1);

                _characterExp = new Dictionary<string, int>();
                _characterExp.Add("Ace", 0);
                _addedMaxHealth = 0;

                _upgradedCards = new List<string>();
            }
        }

        public void ResetValues()
        {
            _partyMembers = new List<string>();

            _characterLevels = new Dictionary<string, int>();
            _characterLevels.Add("Ace", 1);

            _characterExp = new Dictionary<string, int>();
            _characterExp.Add("Ace", 0);

            _addedMaxHealth = 0;
        }

        public void SetParty(int aceLevel, List<string> party, List<int> partyLevels)
        {
            _partyMembers = new List<string>();
            _characterLevels = new Dictionary<string, int>();
            _characterExp = new Dictionary<string, int>();

            _characterLevels["Ace"] = aceLevel;
            _characterExp["Ace"] = 0;

            for(int i = 0; i < party.Count; i++)
            {
                _partyMembers.Add(party[i]);
                _characterLevels[party[i]] = partyLevels[i];
                _characterExp[party[i]] = 0;
            }
        }

        /// <summary>
        /// Levels up the given character by one level.
        /// </summary>
        /// <param name="name"></param>
        public void LevelUp(string name)
        {
            if(!_characterLevels.ContainsKey(name))
            {
                return;
            }

            _characterExp[name] -= ExperienceToLevel(_characterLevels[name]);
            _characterLevels[name]++;
        }

        public void AddPartyMember(string name)
        {
            if(_characterLevels.ContainsKey(name))
            { return; }

            _characterLevels.Add(name, 1);
            _characterExp.Add(name, 0);
            _partyMembers.Add(name);
        }
        
        public int ExperienceToLevel(int level)
        {
            int[] levels =
            {
                100,
                200,
                300,
                500,
                700,
                1000,
                1300,
                1700,
                2200
            };
            if(level <= 9)
            {
                return levels[level - 1];
            }
            return 2200 + (500 * level - 9);
        }

        public bool AddExperience(string name, int experience)
        {
            bool leveled = false;
            _characterExp[name] += experience;
            while(_characterExp[name] >= ExperienceToLevel(_characterLevels[name]))
            {
                int expToLevel = ExperienceToLevel(_characterLevels[name]);
                if(_characterExp[name] >= expToLevel)
                {
                    leveled = true;
                    LevelUp(name);
                }
            }
            return leveled;
        }

        public Dictionary<string, int> GetLevel() {
            return _characterLevels;
        }

        public Dictionary<string, int> GetExp() {
            return _characterExp;
        }

        public List<string> GetMembers() {
            return _partyMembers;
        }

        private void Update()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if(sceneName == "TitleScene" || sceneName == "TitleSceneLandscape")
            { return; }

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                AddExperience("Ace", ExperienceToLevel(_characterLevels["Ace"]));
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                if(_partyMembers[0] != null)
                {
                    AddExperience(_partyMembers[0], ExperienceToLevel(_characterLevels[_partyMembers[0]]));
                }
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                if(_partyMembers[1] != null)
                {
                    AddExperience(_partyMembers[1], ExperienceToLevel(_characterLevels[_partyMembers[1]]));
                }
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                if(_partyMembers[2] != null)
                {
                    AddExperience(_partyMembers[2], ExperienceToLevel(_characterLevels[_partyMembers[2]]));
                }
            }
        }

        public void Reorder(List<string> reorderedList)
        {
            _partyMembers = reorderedList;
        }

        public void RemoveParty(string partyName)
        {
            if(_partyMembers.Contains(partyName))
            {
                _excludedPartyMembers.Add(partyName);
            }
        }

        public void ResetExcludedParty()
        {
            _excludedPartyMembers.Clear();
        }
    }
}
