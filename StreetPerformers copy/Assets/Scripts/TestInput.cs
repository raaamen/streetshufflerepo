using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    private void Update()
    {
        Debug.Log("HORIZTONAL " + Input.GetAxis("Horizontal"));
        Debug.Log("VERTICAL " + Input.GetAxis("Vertical"));
        Debug.Log("SELECTED " + Input.GetAxis("Select"));
        Debug.Log("BACK " + Input.GetAxis("Back"));
    }
}
