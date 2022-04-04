using StreetPerformers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmDeleteScreen : MonoBehaviour
{
    [SerializeField]
    private SaveHolder _saveHolder = null;
    [SerializeField]
    private TextMeshProUGUI _deleteText = null;
    [SerializeField] private ScrollBox _scrollBox = null;
    [SerializeField] private ButtonListener _startButtonListener = null;
    [SerializeField] private ButtonListener _backButtonListener = null;

    private int _deleteIndex = 0;

    public void Initialize(int index)
    {
        _deleteIndex = index;
        _deleteText.text = "Delete Save File #" + (_deleteIndex + 1) + "?";
    }

    public void OnButtonClicked(string id)
    {
        switch(id)
        {
            case "Delete":
                _scrollBox.ToggleEnabled(true);
                _startButtonListener.ToggleEnabled(true);
                _backButtonListener.ToggleEnabled(true);

                _saveHolder.DeleteSave(_deleteIndex);
                this.gameObject.SetActive(false);
                break;
            case "Cancel":
                _scrollBox.ToggleEnabled(true);
                _startButtonListener.ToggleEnabled(true);
                _backButtonListener.ToggleEnabled(true);

                this.gameObject.SetActive(false);
                break;
        }
    }
}
