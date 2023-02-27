using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    public KeyCode flashLightToggleKey = KeyCode.F;
    bool flashlightOn;

    private void Start()
    {
        ToggleFlashlight(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(flashLightToggleKey))
            ToggleFlashlight(!flashlightOn);
    }

    void ToggleFlashlight(bool state)
    {
        GetComponent<Light>().enabled = state;
        flashlightOn = state;
    }
}
