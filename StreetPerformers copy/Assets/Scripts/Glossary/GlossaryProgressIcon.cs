using StreetPerformers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlossaryProgressIcon : MonoBehaviour
{
    public string _hubString;

    private void OnEnable()
    {
        HubAreaData hub = ProgressionHandler.Instance._hubAreas[_hubString];

        if(!hub._active)
        {
            gameObject.SetActive(false);
            return;
        }

        int totalBattles = hub._battles.Count;
        int wonBattles = hub._battleLevel;
        float percentageWon = (float)wonBattles / (float)totalBattles;

        transform.Find("Icon").GetComponent<Image>().fillAmount = percentageWon;
        transform.Find("Percentage").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(percentageWon * 100f) + "%";
    }
}
