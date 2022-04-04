using UnityEngine;
using UnityEngine.SceneManagement;

namespace StreetPerformers
{
    /// <summary>
    /// Handles the lose battle menu, including displaying images only for party members currently in the party.
    /// </summary>
    public class LoseUI : MonoBehaviour
    {
        /// <summary>
        /// Enables the lose image for any party member in the party.
        /// </summary>
        private void Start()
        {
            foreach(string name in PartyHandler.Instance._partyMembers)
            {
                transform.Find("Images").Find(name).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Restarts the current battle from the beginning, shows ad if applicable.
        /// </summary>
        public void RetryBattle()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Level_Select);

            BattleHandler.Instance.ResetBattle();

            Time.timeScale = 1f;

#if UNITY_STANDALONE
            SceneManager.LoadScene("BattleSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
            SceneManager.LoadScene("BattleScene");
            InitializeAds.ShowAd();
#endif
        }

        /// <summary>
        /// Exits to the hub world from battle, shows ad if applicable.
        /// </summary>
        public void ExitToHubWithAd()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);

            Time.timeScale = 1f;

#if UNITY_STANDALONE
            SceneManager.LoadScene("HubWorldSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
            SceneManager.LoadScene("HubWorldScene");
            InitializeAds.ShowAd();
#endif
        }
    }
}

