using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : MonoBehaviour
{

    
    private void OnTriggerEnter(Collider triggerObj) // Using collider triggers as these are more efficient than using Update() or FixedUpdate()
    {
        if (triggerObj.tag == "Player")
        {
            gameObject.GetComponentInChildren<Light>().enabled = true; // Enable the lights when the player moves into the collider.
        }
    }

    private void OnTriggerExit(Collider triggerObj) // Using collider triggers as these are more efficient than using Update() or FixedUpdate()
    {
        if (triggerObj.tag == "Player")
        {
            gameObject.GetComponentInChildren<Light>().enabled = false; // Disable the lights when the player moves out of the collider.
        }
    }
}
