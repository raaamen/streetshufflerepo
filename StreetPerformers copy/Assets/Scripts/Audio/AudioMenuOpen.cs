using UnityEngine;

public class AudioMenuOpen : MonoBehaviour
{
    public float pitchValue;

    public float pitchMax;
    public float pitchValueIncreaser;

    public static bool menuOpen;

    private void Awake()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("InMenu", pitchValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        pitchValue = 0;
        menuOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (menuOpen == true)
        {
            if (pitchValue <= pitchMax)
            {
                pitchValue = pitchValue + pitchValueIncreaser;
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("InMenu", pitchValue);
            }
        }

        if (menuOpen == false)
        {
            if (pitchValue > 0)
            {
                pitchValue = pitchValue - pitchValueIncreaser;
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("InMenu", pitchValue);
            }
        }
    }

    public void InMenu()
    {
        menuOpen = true;
    }

    public void OutMenu()
    {
        menuOpen = false;
    }
}
