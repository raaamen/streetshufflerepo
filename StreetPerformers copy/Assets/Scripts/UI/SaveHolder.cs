using StreetPerformers;
using System.Collections.Generic;
using UnityEngine;

public class SaveHolder : MonoBehaviour
{
    [SerializeField]
    private GameObject _container;
    [SerializeField]
    private TitleMenu _titleMenu;
    [SerializeField]
    private ConfirmDeleteScreen _confirmDelete = null;
    [SerializeField] private ScrollBox _scrollBox = null;
    [SerializeField] private ButtonListener _startButtonListener = null;
    [SerializeField] private ButtonListener _backButtonListener = null;

    private LoadSaveFiles _loadSaveFiles;

    private List<SaveFileButton> _saveButtons;

    private void Start()
    {
        _loadSaveFiles = GameObject.FindGameObjectWithTag("Save").GetComponent<LoadSaveFiles>();

        _saveButtons = new List<SaveFileButton>();
        int buttonIndex = 0;
        for(int i = 0; i < _container.transform.childCount; i++)
        {
            GameObject obj = _container.transform.GetChild(i).gameObject;
            if(obj.tag != "LoadFileButton")
            { continue; }

            _saveButtons.Add(obj.GetComponent<SaveFileButton>());

            _saveButtons[buttonIndex].Populate(_loadSaveFiles.GetSaveAtIndex(buttonIndex), _loadSaveFiles, this);
            _saveButtons[buttonIndex].transform.Find("FilledFile").Find("Number").SetParent(_container.transform);
            buttonIndex++;
        }

        GetComponent<ScrollBox>()._buttonSelected += ButtonSelected;

        _saveButtons[_loadSaveFiles.GetLastSave()].GetComponent<SaveFileButton>().SelectSaveFile();
    }

    private void ButtonSelected(int index)
    {
        _loadSaveFiles.LoadNewIndex(index);
    }

    public void StartGame()
    {
        ProgressionHandler.Instance.LoadData(_loadSaveFiles.curSaveFile);
        PartyHandler.Instance.LoadData(_loadSaveFiles.curSaveFile);

        _loadSaveFiles.StartGame();
        _titleMenu.StartGame();
    }

    public void OpenConfirmDelete(int index)
    {
        _scrollBox.ToggleEnabled(false);
        _startButtonListener.ToggleEnabled(false);
        _backButtonListener.ToggleEnabled(false);

        _confirmDelete.gameObject.SetActive(true);
        _confirmDelete.Initialize(index);
    }

    public void CloseConfirmDelete()
    {
        _scrollBox.ToggleEnabled(true);
        _startButtonListener.ToggleEnabled(true);
        _backButtonListener.ToggleEnabled(true);

        _confirmDelete.gameObject.SetActive(false);
    }

    public bool ConfirmDeleteOpen()
    {
        return _confirmDelete.gameObject.activeInHierarchy;
    }

    public void DeleteSave(int index)
    {
        _saveButtons[index].GetComponent<SaveFileButton>().DeleteSave();
    }
}
