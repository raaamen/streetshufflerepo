using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SaveTestSceneControl : MonoBehaviour
{
    private int _levelNumber;
    private Button _btnBattle;
    private Button _btnToSaveScene;
    private Button _saveButton;
    //private SaveData<string> saveString;

    // Start is called before the first frame update
    private void Awake()
    {
        //GameObject.Find("testSaveText").GetComponent<Text>().text = SaveManager.GetOrCreateString("saveString").value;
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
        SceneManager.LoadScene("QiScene");
    }
    private void SaveFromInput()
    {
        string tempString = GameObject.Find("InputField").GetComponent<InputField>().text;
        SaveInput(tempString);
    }
    public void SaveInput(string strToSave)
    {
        //LevelChoose.saveFile = SaveManager.GetOrCreateLoadFile("saveFile");

        //LevelChoose.saveFile.value = new LoadFile(strToSave);

    }
    // Update is called once per frame
    void Update()
    {

    }
}
