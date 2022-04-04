using StreetPerformers;
using UnityEngine;

public class HubMenu : MonoBehaviour
{
    [SerializeField]
    private HubBackground _hubBackground;

    public void CloseMenu()
    {
        FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Backward);
        _hubBackground._menuOpen = false;
        this.gameObject.SetActive(false);
    }
}
