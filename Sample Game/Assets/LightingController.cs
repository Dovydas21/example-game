using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : MonoBehaviour
{
    public float turnOffRange = 1f;
    public LayerMask playerLayer;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Create a Raycast sphere to find all enemies within the pull radius
        Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, turnOffRange, playerLayer);

        gameObject.GetComponentInChildren<Light>().enabled = objectsInRadius.Length > 0;

    }
}
