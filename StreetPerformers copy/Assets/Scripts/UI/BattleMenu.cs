using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class BattleMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenu = null;
        [SerializeField] private GameObject _menuContents = null;
        [SerializeField] private Image _mainMenuImage = null;
        [SerializeField] private Sprite _mainMenuStill = null;
        [SerializeField] private List<Sprite> _mainMenuInFrames = new List<Sprite>();
        [SerializeField] private List<Sprite> _mainMenuOutFrames = new List<Sprite>();

        [SerializeField] private GameObject _partyMenu = null;
        [SerializeField] private GameObject _partyContents = null;
        [SerializeField] private Image _partyImage = null;
        [SerializeField] private Sprite _partyMenuStill = null;
        [SerializeField] private List<Sprite> _partyMenuInFrames = new List<Sprite>();
        [SerializeField] private List<Sprite> _partyMenuOutFrames = new List<Sprite>();

        [SerializeField]
        private GameObject _menuButton;

        [SerializeField]
        private GameObject _glossary;

        [SerializeField] private GameObject _closeButton = null;


        public void ExitToHub()
        {
            Time.timeScale = 1f;

            if(ProgressionHandler.Instance._isDemo)
            {
                ProgressionHandler.Instance.LoadNextDemoBattle();
                return;
            }

#if UNITY_STANDALONE
            SceneManager.LoadScene("HubWorldSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
            SceneManager.LoadScene("HubWorldScene");
#endif
        }

        public void ExitWithAttemptAd()
        {
            Time.timeScale = 1f;

#if UNITY_STANDALONE
            SceneManager.LoadScene("HubWorldSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
            InitializeAds.AttemptShowAd();
            SceneManager.LoadScene("HubWorldScene");
#endif
        }

        public void QuitBattle()
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>().EndBattleAnaltyics("Battle Quit", true);
            //ExitToHub();
        }

        public void QuitToTitle()
        {
            Time.timeScale = 1f;
#if UNITY_STANDALONE
            SceneManager.LoadScene("TitleSceneLandscape");
#elif UNTIY_IOS || UNITY_ANDROID
            SceneManager.LoadScene("TitleScene");
#endif
        }

        public void CloseMenu()
        {
            Time.timeScale = 1f;
            this.gameObject.SetActive(false);

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);
        }

        public void OpenMenu()
        {
            Time.timeScale = 0f;
            this.gameObject.SetActive(true);

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);
        }

        private IEnumerator MenuAnimation(Image image, List<Sprite> sprites, Action completeCallback)
        {
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(.06f);

            for(int i = 0; i < sprites.Count; i++)
            {
                image.sprite = sprites[i];
                yield return wait;
            }

            completeCallback?.Invoke();
        }

        public void OpenMenuAnimation()
        {
            Time.timeScale = 0f;
            _menuButton.SetActive(false);
            this.gameObject.SetActive(true);

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Menu_Popup);

            StartCoroutine(MenuAnimation(_mainMenuImage, _mainMenuInFrames, delegate { MainMenuOpened(); }));
        }

        public void CloseMenuAnimation()
        {
            if(transform.Find("PartyStatus") != null)
            {
                transform.Find("PartyStatus").gameObject.SetActive(false);
            }
            
            _menuContents.SetActive(false);

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);

            StartCoroutine(MenuAnimation(_mainMenuImage, _mainMenuOutFrames, delegate { MainMenuClosed(); }));
        }

        public void OpenPartyMenuAnimation()
        {
            _closeButton.SetActive(false);

            _partyMenu.SetActive(true);

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Menu_Popup);

            StartCoroutine(MenuAnimation(_partyImage, _partyMenuInFrames, delegate { PartyMenuOpened(); }));
        }

        public void ClosePartyMenuAnimation()
        {
            _partyContents.SetActive(false);

            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);

            StartCoroutine(MenuAnimation(_partyImage, _partyMenuOutFrames, delegate { PartyMenuClosed(); }));
        }

        public void MainMenuOpened()
        {
            _menuContents.SetActive(true);
            _mainMenuImage.sprite = _mainMenuStill;
        }

        public bool IsMainMenuOpen()
        {
            return _menuContents.activeInHierarchy;
        }

        public void MainMenuClosed()
        {
            _menuButton.SetActive(true);
            Time.timeScale = 1f;
            this.gameObject.SetActive(false);
        }

        public void PartyMenuOpened()
        {
            _partyContents.SetActive(true);
            _partyImage.sprite = _partyMenuStill;
        }

        public bool IsPartyMenuOpened()
        {
            return _partyContents.activeInHierarchy;
        }

        public void PartyMenuClosed()
        {
            _partyMenu.SetActive(false);
            _closeButton.SetActive(true);
        }

        public void OpenGlossary()
        {
            _glossary.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);
        }

        public void CloseGlossary()
        {
            _glossary.SetActive(false);
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);
        }
    }
}

