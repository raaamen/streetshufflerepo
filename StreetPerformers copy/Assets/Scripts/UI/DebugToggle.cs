using UnityEngine;
using System.Collections.Generic;

public class DebugToggle : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _toggleObjects = null;

    bool _active = true;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            _active = !_active;
            foreach(GameObject obj in _toggleObjects)
            {
                obj.SetActive(_active);
            }
        }
    }
}
