using UnityEngine;
using UnityEngine.SceneManagement;

namespace StreetPerformers
{
    /// <summary>
    /// Handles anything to do with the photograph at the end of the game.
    /// </summary>
    public class EndMenu : MonoBehaviour
    {
        private bool _canContinue = false;

        private void Start()
        {
            Invoke("CanContinue", 2f);
        }

        private void CanContinue()
        {
            _canContinue = true;
        }

        private void Update()
        {
            if(!_canContinue)
            { return; }

            if(Input.anyKeyDown)
            {
#if UNITY_STANDALONE
                SceneManager.LoadScene("TitleSceneLandscape");
#elif UNITY_IOS || UNITY_ANDROID
                SceneManager.LoadScene("TitleScene");
#endif
            }
        }
    }
}
