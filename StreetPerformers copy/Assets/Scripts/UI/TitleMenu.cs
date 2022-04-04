using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEditor;

namespace StreetPerformers
{
    public class TitleMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _creditPanel;
        [SerializeField]
        private GameObject _titlePanel;
        [SerializeField]
        private GameObject _titleLogo;
        [SerializeField]
        private GameObject _savePanel;
        [SerializeField]
        private GameObject _demoBattlePanel = null;

        [SerializeField]
        private Sprite _storeBackground;
        [SerializeField]
        private Sprite _storeFrontground;
        [SerializeField]
        private Color _storeCrowd;

        private GameObject _startButton;
        private GameObject _creditButton;

        public void StartGame()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Level_Select);

            if(ProgressionHandler.Instance._hubAreas.Count > 0 && !ProgressionHandler.Instance._hubAreas["Corner Store"]._battles[0]._defeated)
            {
                if(!ProgressionHandler.Instance._encounteredEnemies.Contains("Mascot"))
                {
                    ProgressionHandler.Instance._encounteredEnemies.Add("Mascot");
                }
                BattleHandler.Instance.SetBattle("Corner Store", 0, _storeBackground, _storeFrontground, _storeCrowd, true);
                #if UNITY_STANDALONE
                    SceneManager.LoadScene("BattleSceneLandscape");
                #elif UNITY_IPHONE || UNITY_ANDROID
                    SceneManager.LoadScene("BattleScene");
                #endif
            }
            else
            {
                #if UNITY_STANDALONE
                    SceneManager.LoadScene("HubWorldSceneLandscape");
                #elif UNITY_IPHONE || UNITY_ANDROID
                    SceneManager.LoadScene("HubWorldScene");
                #endif
            }
        }

        public void Credits()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            SetButtonsActive(false);

            _creditPanel.SetActive(true);
            _creditPanel.GetComponent<Credits>().StartScrolling();

        }

        public void QuitGame()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);
            Application.Quit();
        }

        public void OpenLoadFileScreen()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            if(ProgressionHandler.Instance._isDemo)
            {
                switch(ProgressionHandler.Instance._demoType)
                {
                    case DemoManager.DemoType.FULL:
                    case DemoManager.DemoType.TUTORIAL:
                        GameObject.FindGameObjectWithTag("Save").GetComponent<LoadSaveFiles>().LoadDemoFile();
                        ProgressionHandler.Instance.LoadNextDemoBattle();
                        return;
                    case DemoManager.DemoType.BATTLE:
                        _demoBattlePanel.SetActive(true);

                        _titleLogo.SetActive(false);
                        _titlePanel.SetActive(false);
                        return;
                }
            }

            _titleLogo.SetActive(false);
            _titlePanel.SetActive(false);
            _savePanel.SetActive(true);
        }

        public void CloseLoadFileScreen()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);

            _titleLogo.SetActive(true);
            _titlePanel.SetActive(true);
            _savePanel.SetActive(false);
        }

        private void Start()
        {
            _startButton = GameObject.Find("StartButton");
            _creditButton = GameObject.Find("CreditsButton");
            //_creditPanel = GameObject.Find("CreditPanel");
            //_creditPanel.SetActive(false);
        }

        private void Update()
        {
#if UNITY_STANDALONE
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SteamManager.UnlockAchievement("FINISHED_TUTORIAL");
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                Steamworks.SteamUserStats.ClearAchievement("FINISHED_TUTORIAL");
            }
#endif
        }

        public void CloseCredits() 
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);

            SetButtonsActive(true);

            _creditPanel.SetActive(false);
            _creditPanel.GetComponent<Credits>().EndScrolling();
            //_creditPanel.SetActive(false);
        }

        private void SetButtonsActive(bool active) 
        {
            _startButton.SetActive(active);
            _creditButton.SetActive(active);
        }

        public void ResetDemo()
        {
            ProgressionHandler.Instance.ResetDemo();
        }
    }

}
