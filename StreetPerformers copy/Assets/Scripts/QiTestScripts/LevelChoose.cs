using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
    public class LevelChoose : MonoBehaviour
    {
        private int _levelNumber;
        private Button _btnBattle;
        private Button _btnToSaveScene;
        private Button _saveButton;
    //public static SaveData<LoadFile> saveFile;
        public static SaveDataGeneric<LoadFile> saveFile = null;
        //private SaveDataGeneric<string> stringTest = null;
        
    // Start is called before the first frame update
        private void Awake()
        {
        if (saveFile == null)
        {
            saveFile = new SaveDataGeneric<LoadFile>("saveFile", new LoadFile("default"));
        }

        Debug.Log(saveFile.Value.getString());
        //GameObject.Find("testSaveText").GetComponent<Text>().text = SaveManager.GetOrCreateLoadFile("saveFile").value.getString();
        GameObject.Find("testSaveText").GetComponent<Text>().text = saveFile.Value.getString();
            //}

            //stringTest = new SaveDataGeneric<string>("string test", "new string");
            _btnBattle = GameObject.Find("Level1Btn").GetComponent<Button>();
            _btnToSaveScene = GameObject.Find("Level2Btn").GetComponent<Button>();
            _saveButton = GameObject.Find("SaveButton").GetComponent<Button>();


            _btnBattle.onClick.AddListener(ToBattleScene);
            _btnToSaveScene.onClick.AddListener(ToSaveScene);
            _saveButton.onClick.AddListener(SaveFromInput);
        }
        void Start()
        {

        }
        private void ToBattleScene()
        {
            SceneManager.LoadScene("MenuTest2");
        }

        private void ToSaveScene()
        {
            SceneManager.LoadScene("SavingTest");
        }
        private void SaveFromInput()
        {
            LoadFile fileToSave = new LoadFile(GameObject.Find("InputField").GetComponent<InputField>().text);
            SaveInput(fileToSave);
        }
        public void SaveInput(LoadFile fileToSave)
        {

            //stringTest.Value = GameObject.Find("InputField").GetComponent<InputField>().text;
            saveFile.Value = fileToSave;

        }
        // Update is called once per frame
        void Update()
        {

        }
    }

*/