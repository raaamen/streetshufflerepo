using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StreetPerformers
{
    public class SaveFileButton : ScrollItem
    {
        [SerializeField]
        private GameObject _emptyFile;
        [SerializeField]
        private GameObject _filledFile;

        [SerializeField]
        private TextMeshProUGUI _lastPlayed;
        [SerializeField]
        private TextMeshProUGUI _totalTime;

        [SerializeField]
        private GameObject _ace;
        [SerializeField]
        private GameObject _mascot;
        [SerializeField]
        private GameObject _contortionist;
        [SerializeField]
        private GameObject _mime;

        [SerializeField]
        private TextMeshProUGUI _number;

        private LoadSaveFiles _loadSaveFiles;
        private SaveHolder _saveHolder = null;

        [SerializeField] private ButtonListener _buttonListener = null;

        public override void Initialize(int index, ScrollBox box)
        {
            base.Initialize(index, box);

            _number.text = "" + (_scrollIndex + 1);
        }

        public void Populate(LoadFile file, LoadSaveFiles loadSaveFiles, SaveHolder saveHolder)
        {
            _loadSaveFiles = loadSaveFiles;
            _saveHolder = saveHolder;

            if(file.GetDefeatedEnemies().Count <= 0)
            {
                _emptyFile.SetActive(true);
                _filledFile.SetActive(false);
                return;
            }
            else
            {
                _emptyFile.SetActive(false);
                _filledFile.SetActive(true);
            }

            DateTime date = file.GetLastPlayed();
            _lastPlayed.text = date.ToShortDateString() + " " + date.ToShortTimeString();

            TimeSpan time = file.GetPlayTime();
            _totalTime.text = time.Hours + " h. " + time.Minutes + " min.";

            Dictionary<string, int> partyLevels = file.GetLevel();

            int aceLevel = partyLevels["Ace"];
            int aceHealth = 12 + (aceLevel * 3);
            string aceText = "Lv. " + aceLevel + "\n" + aceHealth + "HP";
            _ace.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = aceText;

            if(partyLevels.ContainsKey("Mascot"))
            {
                PopulateParty(_mascot, partyLevels["Mascot"]);
            }
            else
            {
                _mascot.SetActive(false);
            }

            if(partyLevels.ContainsKey("Contortionist"))
            {
                PopulateParty(_contortionist, partyLevels["Contortionist"]);
            }
            else
            {
                _contortionist.SetActive(false);
            }

            if(partyLevels.ContainsKey("Mime"))
            {
                PopulateParty(_mime, partyLevels["Mime"]);
            }
            else
            {
                _mime.SetActive(false);
            }
        }

        private void PopulateParty(GameObject partyMember, int level)
        {
            string levelText = "" + level;
            if(level == 10)
            {
                levelText = "MAX";
            }

            partyMember.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = levelText;
        }

        public void SelectSaveFile(LoadSaveFiles loadSaveFiles)
        {
            _scrollBox?.SetHighlight(_scrollIndex);
            loadSaveFiles?.LoadNewIndex(_scrollIndex);
        }

        public void SelectSaveFile()
        {
            _scrollBox?.SetHighlight(_scrollIndex);
            _loadSaveFiles?.LoadNewIndex(_scrollIndex);
        }

        public void OpenConfirmDelete()
        {
            _saveHolder.OpenConfirmDelete(_scrollIndex);
        }

        public void DeleteSave()
        {
            _loadSaveFiles.DeleteSave(_scrollIndex);
            _emptyFile.SetActive(true);
            _filledFile.SetActive(false);
            _loadSaveFiles.LoadNewIndex(_scrollIndex);
        }

        public override void ActivateToggle()
        {
            if(_saveHolder.ConfirmDeleteOpen())
            {
                _saveHolder.CloseConfirmDelete();
            }
            else
            {
                OpenConfirmDelete();
            }
        }
    }
}
