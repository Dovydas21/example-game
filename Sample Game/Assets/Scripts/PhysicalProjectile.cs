using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]

public class PhysicalProjectile : MonoBehaviour
{
    public GameObject bulletHoleDecal;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision objectHit)
    {
        //// 50% chance to despawn the bullet. 50% chance to ricochet.
        //if (Random.Range(0f, 1f) > .5f)
        //    Destroy(gameObject);

        foreach (ContactPoint contact in objectHit.contacts)
        {
            Debug.DrawLine(contact.point, contact.point + contact.normal, Color.green, 2, false);
            GameObject bulletHole = Instantiate(bulletHoleDecal, contact.point + contact.normal * .001f, Quaternion.identity); // Spawn the bullethole decal.

            Destroy(bulletHole, 5f);
        }
    }
}
