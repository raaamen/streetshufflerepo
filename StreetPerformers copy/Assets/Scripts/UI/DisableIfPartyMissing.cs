using StreetPerformers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class DisableIfPartyMissing : MonoBehaviour
    {
        [SerializeField] private string _partyMember = "";

        private void Awake()
        {
            if (!PartyHandler.Instance._partyMembers.Contains(_partyMember))
            {
                this.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (!PartyHandler.Instance._partyMembers.Contains(_partyMember))
            {
                this.gameObject.SetActive(false);
            }
        }
    }

}