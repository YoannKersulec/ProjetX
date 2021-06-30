using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fullscreen : MonoBehaviour
{
    int _fullscreen = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(_fullscreen == 0)
            {
                Screen.SetResolution(800, 600, false);
                _fullscreen++;
            }
            else if (_fullscreen == 1)
            {
                Screen.SetResolution(1920, 1080, false);
                _fullscreen++;

            }
            else
            {
                Screen.SetResolution(1920, 1080, true);
                _fullscreen =0;

            }
        }
    }
}
