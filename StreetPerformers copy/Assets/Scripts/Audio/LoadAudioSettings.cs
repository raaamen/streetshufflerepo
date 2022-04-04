using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Loads the Music and SFX volume settings from player prefs at the start of the game.
    /// </summary>
    public class LoadAudioSettings : MonoBehaviour
    {
        private void Awake()
        {
            FMODUnity.RuntimeManager.GetBus("bus:/Master/Music").setVolume(PlayerPrefs.GetFloat("Music"));
            FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX").setVolume(PlayerPrefs.GetFloat("Sound"));
        }
    }
}