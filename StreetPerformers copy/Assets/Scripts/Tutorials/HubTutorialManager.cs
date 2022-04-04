using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class HubTutorialManager : MonoBehaviour
    {
        [SerializeField] private Button _scrimButton = null;

        [Header("First Iteration")]
        [SerializeField] private GameObject _firstTutorialHighlight = null;
        [SerializeField] private Transform _pierTransform = null;
        [SerializeField] private Button _pierButton = null;

        [Header("Second Iteration")]
        [SerializeField] private BattleMenu _menuScript = null;
        [SerializeField] private GameObject _menuOpenPanel = null;
        [SerializeField] private Transform _menuButtonTransform = null;
        [SerializeField] private Button _menuButton = null;

        [SerializeField] private GameObject _partyMenuOpenPanel = null;
        [SerializeField] private Transform _partyButtonTransform = null;
        [SerializeField] private Button _partyButton = null;

        [SerializeField] private GameObject _reorderPanel = null;
        [SerializeField] private Transform _reorderButtonTransform = null;
        [SerializeField] private Button _reorderButton = null;

        [SerializeField] private GameObject _startReorderPanel = null;
        [SerializeField] private List<Transform> _characterButtonTransforms = new List<Transform>();
        [SerializeField] private List<Button> _characterButtons = new List<Button>();
        [SerializeField] private Transform _aceHead = null;

        [SerializeField] private GameObject _returnPanel = null;
        [SerializeField] private Transform _returnButtonTransform = null;
        [SerializeField] private Button _returnButton = null;

        [SerializeField] private GameObject _exitPanel = null;
        [SerializeField] private Transform _exitButtonTransform = null;
        [SerializeField] private Button _exitButton = null;

        [SerializeField] private GameObject _finalHubDialoguePanel = null;

        [SerializeField] private GameObject _finalHubPanel = null;
        [SerializeField] private List<Transform> _hubButtonTransforms = new List<Transform>();
        [SerializeField] private List<Button> _hubButtons = new List<Button>();

        private bool _continue = false;

        public void Initialize(int iteration)
        {
            this.gameObject.SetActive(true);

            switch (iteration)
            {
                case 1:
                    StartCoroutine(StartHighlightHub());
                    break;
                case 2:
                    StartCoroutine(StartHighlightParty());
                    break;
            }
        }

        private IEnumerator StartHighlightHub()
        {
            //Highlight the pier
            Transform pierParent = _pierTransform.parent;
            int pierSiblingIndex = _pierTransform.GetSiblingIndex();
            _pierTransform.SetParent(transform);
            _firstTutorialHighlight.SetActive(true);

            //Wait until pier is clicked
            _continue = false;
            _pierButton.onClick.AddListener(Continue);
            _scrimButton.interactable = false;

            yield return new WaitUntil(CanContinue);

            _pierButton.onClick.RemoveListener(Continue);
            _scrimButton.interactable = true;

            //End highlight
            _pierTransform.SetParent(pierParent);
            _pierTransform.SetSiblingIndex(pierSiblingIndex);
            _firstTutorialHighlight.SetActive(false);

            this.gameObject.SetActive(false);
        }

        private IEnumerator StartHighlightParty()
        {
            _scrimButton.interactable = false;

            //Open menu
            Transform menuParent = _menuButtonTransform.parent;
            int menuSiblingIndex = _menuButtonTransform.GetSiblingIndex();
            _menuButtonTransform.SetParent(transform);
            _menuOpenPanel.SetActive(true);

            _continue = false;
            _menuButton.onClick.AddListener(Continue);

            yield return new WaitUntil(CanContinue);

            _menuButton.onClick.RemoveListener(Continue);

            _menuButtonTransform.SetParent(menuParent);
            _menuButtonTransform.SetSiblingIndex(menuSiblingIndex);
            _menuOpenPanel.SetActive(false);

            //Open party menu
            Transform partyParent = _partyButtonTransform.parent;
            int partySiblingIndex = _partyButtonTransform.GetSiblingIndex();
            _partyButtonTransform.SetParent(transform);
            _partyMenuOpenPanel.SetActive(true);

            _continue = false;
            _partyButton.onClick.AddListener(Continue);

            yield return new WaitUntil(CanContinue);

            _partyButton.onClick.RemoveListener(Continue);

            _partyButtonTransform.SetParent(partyParent);
            _partyButtonTransform.SetSiblingIndex(partySiblingIndex);
            _partyMenuOpenPanel.SetActive(false);

            //Reorder button
            Transform reorderParent = _reorderButtonTransform.parent;
            int reorderSiblingIndex = _reorderButtonTransform.GetSiblingIndex();
            _reorderButtonTransform.SetParent(transform);
            _reorderPanel.SetActive(true);

            _continue = false;
            _reorderButton.onClick.AddListener(Continue);

            yield return new WaitUntil(CanContinue);

            _reorderButton.onClick.RemoveListener(Continue);

            _reorderButtonTransform.SetParent(reorderParent);
            _reorderButtonTransform.SetSiblingIndex(reorderSiblingIndex);
            _reorderPanel.SetActive(false);

            //Reorder party buttons
            Transform characterParent = _characterButtonTransforms[0].parent;
            List<int> siblingIndexes = new List<int>();

            int clickedNum = 0;
            for(int i = 0; i < _characterButtonTransforms.Count; i++)
            {
                int index = i;
                siblingIndexes.Insert(index, _characterButtonTransforms[index].GetSiblingIndex());
                _characterButtonTransforms[index].SetParent(transform);
                _characterButtons[index].onClick.AddListener(delegate
                {
                    _characterButtonTransforms[index].SetParent(characterParent);
                    _characterButtonTransforms[index].SetSiblingIndex(siblingIndexes[index]);
                    _characterButtons[index].onClick.RemoveAllListeners();
                    clickedNum++;
                });
            }
            _startReorderPanel.SetActive(true);

            yield return new WaitUntil(delegate { return clickedNum == _characterButtonTransforms.Count; });

            _startReorderPanel.SetActive(false);

            StartCoroutine(StartExitParty());
        }

        private IEnumerator StartExitParty()
        {
            //Exit party menu
            Transform returnParent = _returnButtonTransform.parent;
            int returnSiblingIndex = _returnButtonTransform.GetSiblingIndex();
            _returnButtonTransform.SetParent(transform);
            _returnPanel.SetActive(true);

            _continue = false;
            _returnButton.onClick.AddListener(Continue);

            yield return new WaitUntil(CanContinue);

            _returnButton.onClick.RemoveListener(Continue);

            _returnButtonTransform.SetParent(returnParent);
            _returnButtonTransform.SetSiblingIndex(returnSiblingIndex);
            _returnPanel.SetActive(false);

            //Exit main menu
            Transform exitParent = _exitButtonTransform.parent;
            int exitSiblingIndex = _exitButtonTransform.GetSiblingIndex();
            _exitButtonTransform.SetParent(transform);
            _exitPanel.SetActive(true);

            _continue = false;
            _exitButton.onClick.AddListener(Continue);

            yield return new WaitUntil(CanContinue);

            _exitButton.onClick.RemoveListener(Continue);

            _exitButtonTransform.SetParent(exitParent);
            _exitButtonTransform.SetSiblingIndex(exitSiblingIndex);
            _exitPanel.SetActive(false);

            //Show final dialogue
            _finalHubDialoguePanel.SetActive(true);

            _continue = false;
            _scrimButton.interactable = true;
            yield return new WaitUntil(CanContinue);
            _scrimButton.interactable = false;

            _finalHubDialoguePanel.SetActive(false);

            //Highlight hubs
            Transform hubParent = _hubButtonTransforms[0].parent;
            List<int> hubIndexes = new List<int>();
            for(int i = 0; i < _hubButtonTransforms.Count; i++)
            {
                hubIndexes.Insert(i, _hubButtonTransforms[i].GetSiblingIndex());
                _hubButtonTransforms[i].SetParent(transform);
                _hubButtons[i].onClick.AddListener(Continue);
            }

            Transform aceParent = _aceHead.parent;
            int aceIndex = _aceHead.GetSiblingIndex();
            _aceHead.SetParent(transform);

            _finalHubPanel.SetActive(true);

            _continue = false;

            yield return new WaitUntil(CanContinue);

            for(int i = 0; i < _hubButtonTransforms.Count; i++)
            {
                _hubButtonTransforms[i].SetParent(hubParent);
                _hubButtonTransforms[i].SetSiblingIndex(hubIndexes[i]);
                _hubButtons[i].onClick.RemoveListener(Continue);
            }

            _aceHead.SetParent(aceParent);
            _aceHead.SetSiblingIndex(aceIndex);

            _finalHubPanel.SetActive(false);

            ProgressionHandler.Instance._officialTutorialFinished = true;
            ProgressionHandler.Instance.SaveCurrentProgression();
            this.gameObject.SetActive(false);
        }

        private bool CanContinue()
        {
            return _continue;
        }

        public void Continue()
        {
            _continue = true;
        }
    }

}
