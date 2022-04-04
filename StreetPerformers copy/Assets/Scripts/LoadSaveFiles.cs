using SaveManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StreetPerformers
{
    public class LoadSaveFiles : MonoBehaviour
    {
        private int _curFileIndex = 0;
        private SaveDataGeneric<LoadFile[]> _save;
        private SaveDataGeneric<int> _lastSave;
        private SaveDataGeneric<LoadFile> _demoSave;
        private SaveDataGeneric<int> _demoIndex;

        private DateTime _startOfSession;

        private LoadFile _curSaveFile;
        public LoadFile curSaveFile
        {
            get
            {
                if(_curSaveFile != null)
                { return _curSaveFile; }

                return _save.Value[_curFileIndex];
            }
        }

        private KeyCode[] _keyCodes =
        {
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };

        private void Start()
        {
            _save = new SaveDataGeneric<LoadFile[]>("SaveFiles", new LoadFile[10]);
            _lastSave = new SaveDataGeneric<int>("LastSave", 0);
            _demoSave = new SaveDataGeneric<LoadFile>("DemoSave");
            _demoIndex = new SaveDataGeneric<int>("DemoIndex", 0);

            if(SaveManager.GetData("save") != null)
            {
                _save.Value[0] = (LoadFile)SaveManager.GetData("save");
                SaveManager.RemoveData("save");

                SaveManager.EnableWriting();
                SaveManager.SaveToFile();
                SaveManager.DisableWriting();
            }

#if UNITY_IOS || UNITY_ANDROID
            _lastSave.Value = 0;
#endif
            if(ProgressionHandler.Instance._isDemo)
            {
                LoadDemoFile();
            }
            else
            {
                LoadNewIndex(_lastSave.Value);
            }
        }

        private void Update()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if(sceneName != "TitleScene" && sceneName != "TitleSceneLandscape")
            {
                return;
            }

            for(int i = 0; i < _keyCodes.Length; i++)
            {
                if(Input.GetKeyDown(_keyCodes[i]))
                {
                    LoadNewIndex(i);
                }
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                /*ProgressionHandler.Instance.LoadData(_curSaveFile);
                PartyHandler.Instance.LoadData(_curSaveFile);

                GameObject.Find("Canvas").GetComponent<TitleMenu>().StartGame();*/
            }
        }

        public void StartGame()
        {
            _startOfSession = DateTime.Now;
        }

        public void LoadNewIndex(int index)
        {
            _curFileIndex = index;
            _curSaveFile = _save.Value[_curFileIndex];
            if(_curSaveFile == null)
            {
                _curSaveFile = new LoadFile();
            }

#if UNITY_IOS || UNITY_ANDROID
            ProgressionHandler.Instance.LoadData(_curSaveFile);
            PartyHandler.Instance.LoadData(_curSaveFile);

            StartGame();
#endif
        }

        public void LoadDemoFile()
        {
            _curSaveFile = _demoSave.Value;
            if(_curSaveFile == null)
            {
                _curSaveFile = new LoadFile();
            }
            ProgressionHandler.Instance.LoadData(_curSaveFile);
            ProgressionHandler.Instance.SetDemoIndex(_demoIndex.Value);
            PartyHandler.Instance.LoadData(_curSaveFile);
        }

        public LoadFile GetSaveAtIndex(int index)
        {
            LoadFile file = _save.Value[index];
            if(file == null)
            {
                file = new LoadFile();
            }
            return file;
        }

        public void Save(LoadFile file)
        {
            SaveManager.EnableWriting();

            if (ProgressionHandler.Instance._isDemo)
            {
                _demoIndex.Value = ProgressionHandler.Instance._demoIndex;
                _demoSave.Value = file;
            }
            else
            {
                _lastSave.Value = _curFileIndex;
                file.SetPlayTime(DateTime.Now, _startOfSession);
                _save.Value[_curFileIndex] = file;
            }
            
            SaveManager.SaveToFile();
            SaveManager.DisableWriting();
        }

        public int GetLastSave()
        {
            return _lastSave.Value;
        }

        public void DeleteSave(int index)
        {
            SaveManager.EnableWriting();
            
            _save.Value[index] = new LoadFile();

            SaveManager.SaveToFile();
            SaveManager.DisableWriting();
        }
    }

}
