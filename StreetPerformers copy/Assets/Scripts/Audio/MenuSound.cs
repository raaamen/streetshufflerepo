using UnityEngine;

public class MenuSound : MonoBehaviour
{
    public void MenuPopOut()
    {
        FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Menu_Popout);
    }
}
