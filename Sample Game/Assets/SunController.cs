using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    public KeyCode sunToggleKey = KeyCode.L;
    public GameObject sun;
    bool sunState = false;

    private void Awake()
    {
        sun.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(sunToggleKey)) { 
            sun.SetActive(!sunState);
            sunState = !sunState;
        }
    }
}
