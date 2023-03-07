using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class FakeLitWindowController : MonoBehaviour
{
    public GameObject WindowPane;
    public GameObject FakeWindowPane;
    public float ToggleRange = 100f;
    public LayerMask PlayerLayer;

    private void OnTriggerEnter(Collider triggerObj) // Using collider triggers as these are more efficient than using Update() or FixedUpdate()
    {
        Profiler.BeginSample("FakeLitWindowController: OnTriggerEnter()");
        if (triggerObj.tag == "Player")
        {
            ToggleWindows(true); // Enable the real windows when the player moves out of the collider.
        }
        Profiler.EndSample();
    }

    private void OnTriggerExit(Collider triggerObj) // Using collider triggers as these are more efficient than using Update() or FixedUpdate()
    {
        Profiler.BeginSample("FakeLitWindowController: OnTriggerExit()");
        if (triggerObj.tag == "Player")
        {
            ToggleWindows(false); // Disable the real windows when the player moves out of the collider.
        }
        Profiler.EndSample();
    }

    void ToggleWindows(bool toggle)
    {
        WindowPane.SetActive(toggle);
        FakeWindowPane.SetActive(!toggle);
    }
}
