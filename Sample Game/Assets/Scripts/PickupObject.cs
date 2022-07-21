using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public Collider triggerCollider;
    public GameObject gunHolder;

    private void Start()
    {
        gunHolder = GameObject.FindGameObjectWithTag("GunHolder");
    }

    private void OnTriggerEnter(Collider triggerCollider)
    {
        print("Pickup triggered.");
        if (triggerCollider.tag == "Player") // Trigger a pick-up if you are the player.
        {
            gameObject.transform.SetParent(gunHolder.transform);
            gameObject.transform.position = gunHolder.transform.position;
            gameObject.transform.rotation = gunHolder.transform.rotation;
            // Destroy(gameObject.GetComponent<Rigidbody>()); // Disable the rigidbody on the object.

            gameObject.GetComponent<Rigidbody>().useGravity = false; // Disable the gravity on the rigidbody on the object.
            triggerCollider.enabled = false; // Disable the trigger on the object.
        }
    }
}
