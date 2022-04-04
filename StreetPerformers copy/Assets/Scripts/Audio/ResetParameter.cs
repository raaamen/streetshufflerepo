using UnityEngine;

public class ResetParameter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Slow Music", 0);
    }
}
