using StreetPerformers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glossary : MonoBehaviour
{
    [SerializeField]
    private BattleMenu _battleMenu = null;

    [SerializeField]
    private GameObject _status = null;
    [SerializeField]
    private GameObject _characters = null;
    [SerializeField]
    private GameObject _progress = null;
    [SerializeField]
    private GameObject _tutorial = null;

    private string _curId = "";

    private void OnEnable()
    {
        OnButtonClicked("Characters");
    }

    public void OnButtonClicked(string id)
    {
        switch(id)
        {
            case "Status":
            case "Characters":
            case "Progress":
            case "Tutorial":
                ChangePage(id);
                break;
            case "Quit":
                _battleMenu.CloseGlossary();
                break;
        }
    }

    private void ChangePage(string id)
    {
        if(_curId.Equals(id))
        {
            return;
        }

        _status.SetActive(false);
        _characters.SetActive(false);
        _progress.SetActive(false);
        _tutorial.SetActive(false);

        switch(id)
        {
            case "Status":
                _status.SetActive(true);
                break;
            case "Characters":
                _characters.SetActive(true);
                break;
            case "Progress":
                _progress.SetActive(true);
                break;
            case "Tutorial":
                _tutorial.SetActive(true);
                break;
        }
    }
}
