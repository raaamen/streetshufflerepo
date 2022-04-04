using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class CutsceneManager : MonoBehaviour
    {
        [Header("Intro cutscene")]
        [SerializeField] private GameObject _bus = null;
        [SerializeField] private Transform _busSpawnPosition = null;
        [SerializeField] private Transform _busStopPosition = null;
        [SerializeField] private Transform _busExitPosition = null;

        [SerializeField] private GameObject _ace = null;
        [SerializeField] private Transform _aceBusPosition = null;
        [SerializeField] private Transform _aceSpawnPosition = null;
        [SerializeField] private PlayerTurn _aceTurn = null;
        [SerializeField] private Crowd _crowd = null;
        [SerializeField] private SpriteRenderer _background = null;

        [SerializeField] private BattleAgents _battleAgents = null;
        [SerializeField] private BattleManager _battleManager = null;
        [SerializeField] private TurnManager _turnManager = null;

        [SerializeField] private DialogueManager _dialogueManager = null;
        [SerializeField] private GameObject _dialogue1 = null;
        [SerializeField] private GameObject _performanceDialogue1 = null;
        [SerializeField] private GameObject _performanceDialogue2 = null;
        [SerializeField] private GameObject _performanceDialogue3 = null;
        [SerializeField] private GameObject _performanceDialogue4 = null;

        private bool _continue = false;
        private Color _backgroundStartColor = Color.white;
        private Color _backgroundEndColor = Color.white;

        private void Start()
        {
            if(!BattleHandler.Instance._preBattleAnimation)
            { return; }

            _crowd.SetAlpha(0, CrowdPosition.ALL);

            _backgroundStartColor = BattleHandler.Instance._backgroundColor;
            _backgroundEndColor = Color.white;
            _background.color = _backgroundStartColor;

            StartCoroutine(StartCutscene());
        }

        private IEnumerator StartCutscene()
        {
            yield return new WaitForSeconds(.5f);

            GameObject bus = Instantiate(_bus, _busSpawnPosition.position, Quaternion.identity);

            float busInTime = 2f;
            bus.transform.DOMove(_busStopPosition.position, busInTime).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(busInTime);

            _ace.transform.position = _aceBusPosition.position;

            yield return new WaitForSeconds(1.5f);

            float busOutTime = 2f;
            bus.transform.DOMove(_busExitPosition.position, busOutTime).SetEase(Ease.InQuad);

            yield return new WaitForSeconds(busOutTime);

            Destroy(bus);

            //Start dialogue 1
            _dialogueManager.StartConversation(_dialogue1, delegate { StartCoroutine(StartAfterDialogueCutscene()); });
        }

        private IEnumerator StartAfterDialogueCutscene()
        {
            //Show Ace performing tricks for a second
            yield return new WaitForSeconds(1.5f);

            //Dialogue pops up "I've gotta step it up a notch"
            yield return WaitUntilDialogueFinished(_performanceDialogue1);

            float fadeTime = 4f;

            //Show Front crowd fading in and background slowly changing color
            _crowd.LerpAlpha(1, 2f, CrowdPosition.FRONT);
            Color backgroundColor = Color.Lerp(_backgroundStartColor, _backgroundEndColor, .33f);
            _background.DOColor(backgroundColor, fadeTime);
            yield return new WaitForSeconds(fadeTime);

            //Dialogue pops up "And if you look under your shoe, you'll find..."
            yield return WaitUntilDialogueFinished(_performanceDialogue2);

            //Show Middle crowd fading in and background slowly changing color
            _crowd.LerpAlpha(1, 2f, CrowdPosition.MIDDLE);
            backgroundColor = Color.Lerp(_backgroundStartColor, _backgroundEndColor, .67f);
            _background.DOColor(backgroundColor, fadeTime);
            yield return new WaitForSeconds(fadeTime);

            //Dialogue pops up "My hands are completely empty now!" "*crowd applauds*"
            yield return WaitUntilDialogueFinished(_performanceDialogue3);

            //Show Back crowd fading in
            _crowd.LerpAlpha(1, 2f, CrowdPosition.BACK);
            _background.DOColor(_backgroundEndColor, fadeTime);
            yield return new WaitForSeconds(fadeTime);

            //Dialogue pops up "Now for my final act I'll need a volunteer!"
            yield return WaitUntilDialogueFinished(_performanceDialogue4);

            //Mascot enters
            float enemyMoveTime = 3f;
            _battleAgents.MoveEnemiesToStart(enemyMoveTime);
            yield return new WaitForSeconds(enemyMoveTime);

            _aceTurn.PlayerStartAnimation();

            //yield return new WaitForSeconds(3f);
        }

        private IEnumerator WaitUntilDialogueFinished(GameObject dialogue)
        {
            _dialogueManager.StartConversation(dialogue, Continue);
            _continue = false;
            yield return new WaitUntil(CanContinue);
        }

        public void Continue()
        {
            _continue = true;
        }

        private bool CanContinue()
        {
            return _continue;
        }
    }
}