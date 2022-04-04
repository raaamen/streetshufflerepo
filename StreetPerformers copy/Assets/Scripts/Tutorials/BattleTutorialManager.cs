using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class BattleTutorialManager : MonoBehaviour
    {
        [SerializeField] private Image _tutorialScrim = null;
        [SerializeField] private Button _tutorialScrimButton = null;

        [Header("Mascot Battle")]
        [SerializeField] private Transform _cardPanel = null;
        [SerializeField] private Transform _energyPanel = null;
        [SerializeField] private PlayerHand _playerHand = null;

        [SerializeField] private GameObject _cardTutorialPanel = null;
        [SerializeField] private GameObject _energyTutorialPanel = null;

        [Header("Contortionist Battle")]
        [SerializeField] private GameObject _indicatorTutorialPanel = null;
        [SerializeField] private Image _indicatorImage = null;
        [SerializeField] private RectTransform _indicatorPos = null;
        [SerializeField] private RectTransform _indicatorArrowPos = null;
        [SerializeField] private BattleAgents _battleAgents = null;

        [Header("Mime Battle")]
        [SerializeField] private GameObject _cardHistoryTutorialPanel = null;
        [SerializeField] private Transform _cardHistoryTransform = null;
        [SerializeField] private Button _cardHistoryButton = null;

        private bool _continue = false;

        public void Initialize(string enemy)
        {
            this.gameObject.SetActive(true);

            switch (enemy)
            {
                case "Mascot":
                    StartCoroutine(StartMascotTutorial());
                    break;
                case "Contortionist":
                    StartCoroutine(StartContortionistTutorial());
                    break;
                case "Mime":
                    StartCoroutine(StartMimeTutorial());
                    break;
            }
        }

        private IEnumerator StartMascotTutorial()
        {
            //Hiighlight cards
            Transform cardPanelParent = _cardPanel.parent;
            int cardPanelSiblingIndex = _cardPanel.GetSiblingIndex();
            _cardPanel.SetParent(transform);
            _cardTutorialPanel.gameObject.SetActive(true);

            //Wait until a card has been clicked on
            yield return new WaitUntil(delegate { return _playerHand._cardSelected; });

            _cardTutorialPanel.gameObject.SetActive(false);
            _cardPanel.SetParent(cardPanelParent);
            _cardPanel.SetSiblingIndex(cardPanelSiblingIndex);
            _tutorialScrim.enabled = false;

            //Wait until a card has been played
            yield return new WaitUntil(delegate { return _playerHand._firstCardPlayed; });

            //Highlight energy
            _tutorialScrim.enabled = true;

            Transform energyPanelParent = _energyPanel.parent;
            int energyPanelSiblingIndex = _energyPanel.GetSiblingIndex();
            _energyPanel.SetParent(transform);
            _energyTutorialPanel.gameObject.SetActive(true);

            //Wait until screen is clicked
            yield return new WaitForSeconds(.3f);
            _continue = false;
            yield return new WaitUntil(delegate { return _continue || Input.anyKeyDown; });

            _energyTutorialPanel.gameObject.SetActive(false);
            _energyPanel.SetParent(energyPanelParent);
            _energyPanel.SetSiblingIndex(energyPanelSiblingIndex);

            this.gameObject.SetActive(false);
        }

        private IEnumerator StartContortionistTutorial()
        {
            //Highlight enemy turn indicator
            EnemyTurn enemyTurn = _battleAgents._enemyList[0].GetComponent<EnemyTurn>();
            _indicatorImage.sprite = enemyTurn.GetActiveIndicator();
            Vector3 indicatorScreenPos = Camera.main.WorldToScreenPoint(enemyTurn.GetActiveIndicatorPos());
            Vector3 arrowOffset = _indicatorArrowPos.position - _indicatorPos.position;
            _indicatorPos.position = indicatorScreenPos;
            _indicatorArrowPos.position = _indicatorPos.position + arrowOffset;
            _indicatorTutorialPanel.SetActive(true);

            //Wait until screen clicked
            yield return new WaitForSeconds(.3f);
            _continue = false;
            yield return new WaitUntil(delegate { return _continue || Input.anyKeyDown; });

            //Once clicked, disable
            _indicatorTutorialPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }

        private IEnumerator StartMimeTutorial()
        {
            //Show card history panel
            Transform historyPanelParent = _cardHistoryTransform.parent;
            int historyPanelSiblingIndex = _cardHistoryTransform.GetSiblingIndex();
            _cardHistoryTransform.SetParent(transform);
            _cardHistoryTutorialPanel.gameObject.SetActive(true);

            //Wait until card history button is clicked
            _continue = false;
            _cardHistoryButton.onClick.AddListener(Continue);
            _tutorialScrimButton.interactable = false;
            yield return new WaitUntil(delegate { return _continue; });
            _cardHistoryButton.onClick.RemoveListener(Continue);
            _tutorialScrimButton.interactable = true;   

            //Once clicked, disable
            _cardHistoryTutorialPanel.SetActive(false);
            _cardHistoryTransform.SetParent(historyPanelParent);
            _cardHistoryTransform.SetSiblingIndex(historyPanelSiblingIndex);

            this.gameObject.SetActive(false);
        }

        public void Continue()
        {
            _continue = true;
        }
    }

}
