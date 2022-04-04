using UnityEditor;
using UnityEngine;
using UnityEngine.Advertisements;

namespace StreetPerformers
{
    public class InitializeAds : MonoBehaviour
    {
#if UNITY_IOS
        private string _gameId = "4083782";
#else
        private string _gameId = "4083783";
#endif

        public bool _testMode = false;

        private static string _placementId = "video";

        // Start is called before the first frame update
        void Start()
        {
            Advertisement.Initialize(_gameId, _testMode);
        }

        //Only called on mobile.
        public static void ShowAd()
        {
            if(ProgressionHandler.Instance._adCooldownTimer <= 0f)
            {
                Advertisement.Show(_placementId);
                ProgressionHandler.Instance.StartAdCooldown(4f * 60f, 6f * 60f);
            }
            else
            {
                ProgressionHandler.Instance._adCooldownTimer -= 30f;
            }
        }

        public static void AttemptShowAd()
        {
            if(ProgressionHandler.Instance._adCooldownTimer <= 0f && ProgressionHandler.Instance._adSuccessCooldownTimer <= 0f)
            {
                Advertisement.Show(_placementId);
                ProgressionHandler.Instance.StartAdCooldown(4f * 60f, 6f * 60f);
            }
        }
    }
}

