using DG.Tweening;
using System;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Plays the drip menu animation, then activates/deactivates the contents of the menu.
    /// </summary>
    public class DripMenu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField][Tooltip("Menu that opens/closes after animation completes")]
        private GameObject _dripPanel;

        [Header("Conditions")]
        [SerializeField][Tooltip("Set to true if time scale is set to 0 while the menu is open")]
        private bool _changeTimeScale;

        //Action that gets called upon animation completion
        [HideInInspector]
        public Action OnAnimationFinish;

        /// <summary>
        /// Starts the drip animation and invokes the OnAnimationFinish event upon completion
        /// </summary>
        public void OpenDripMenu()
        {
            //FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            if (_changeTimeScale)
            {
                Time.timeScale = 0f;
            }

            this.gameObject.SetActive(true);

            //Start the drip animation
            Animator dripAnim = _dripPanel.GetComponentInChildren<Animator>();
            dripAnim.enabled = true;
            dripAnim.Play("Base Layer.DripSheet");

            //Start the drip movement down the screen
            _dripPanel.transform.position = transform.Find("StartPosition").position;
            _dripPanel.transform.DOMove(transform.Find("EndPosition").position, 1.2f).SetUpdate(true).SetEase(Ease.OutSine).OnComplete(
                delegate
                {
                    dripAnim.enabled = false;
                    OnAnimationFinish?.Invoke();
                });

            if(this.gameObject.name == "HistoryPanel")
            {
                FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_History_Popup);
            }
        }

        /// <summary>
        /// Starts the drip menu movement back up
        /// </summary>
        public void CloseDripMenu()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Card_Histroy_Popout);

            //Starts the movement of the drip menu back up.
            _dripPanel.transform.DOKill();
            _dripPanel.transform.DOMove(transform.Find("StartPosition").position, .5f).SetUpdate(true).OnComplete(
                delegate
                {
                    if(_changeTimeScale)
                    {
                        Time.timeScale = 1f;
                    }
                    
                    this.gameObject.SetActive(false);
                });
        }


        private void Update()
        {
            if(Input.GetButtonDown("Back"))
            {
                CloseDripMenu();
            }
        }
    }
}