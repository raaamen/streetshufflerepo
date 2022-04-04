using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlossaryHowToPlay : MonoBehaviour
{
    [SerializeField] private GameObject _healthArmor = null;
    [SerializeField] private GameObject _drafting = null;
    [SerializeField] private GameObject _statusEffects = null;
    [SerializeField] private GameObject _energy = null;
    [SerializeField] private GameObject _attackOrder = null;
    [SerializeField] private GameObject _leveling = null;
    [SerializeField] private GameObject _rewards = null;

    private string _curId = "";

    public void OnButtonClicked(string buttonId)
    {
        switch(buttonId)
        {
            case "HealthArmor":
            case "Drafting":
            case "StatusEffects":
            case "Energy":
            case "AttackOrder":
            case "Leveling":
            case "Rewards":
                ChangePage(buttonId);
                break;
        }
    }

    private void ChangePage(string id)
    {
        if(_curId.Equals(id))
        {
            return;
        }

        _healthArmor.SetActive(false);
        _drafting.SetActive(false);
        _statusEffects.SetActive(false);
        _energy.SetActive(false);
        _attackOrder.SetActive(false);
        _leveling.SetActive(false);
        _rewards.SetActive(false);

        switch(id)
        {
            case "HealthArmor":
                _healthArmor.SetActive(true);
                break;
            case "Drafting":
                _drafting.SetActive(true);
                break;
            case "StatusEffects":
                _statusEffects.SetActive(true);
                break;
            case "Energy":
                _energy.SetActive(true);
                break;
            case "AttackOrder":
                _attackOrder.SetActive(true);
                break;
            case "Leveling":
                _leveling.SetActive(true);
                break;
            case "Rewards":
                _rewards.SetActive(true);
                break;
        }
    }
}
