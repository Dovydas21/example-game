using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeLitWindowController : MonoBehaviour
{
    public GameObject WindowPane;
    public GameObject FakeWindowPane;
    public float ToggleRange = 100f;
    public LayerMask PlayerLayer;

    private void FixedUpdate()
    {
        // Create a Raycast sphere to find all enemies within the pull radius
        Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, ToggleRange, PlayerLayer);

        bool playerInRadius = objectsInRadius.Length > 0;
        WindowPane.SetActive(playerInRadius);
        FakeWindowPane.SetActive(!playerInRadius);

    }
}
