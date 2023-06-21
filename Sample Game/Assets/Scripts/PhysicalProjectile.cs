using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]

public class PhysicalProjectile : MonoBehaviour
{
    public GameObject bulletHoleDecal;
    public GunInfo gunInfo;
    private int bounceCount = 0;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    //public void EnableColliders()
    //{
    //    col.enabled = true;
    //}

    void OnCollisionEnter(Collision objectHit)
    {
        col.enabled = true;
        Transform objectTransform = objectHit.transform;

        foreach (ContactPoint contact in objectHit.contacts)
        {
            bounceCount++;
            Debug.DrawLine(contact.point, contact.point + contact.normal, Color.green, 2f, false);
            GameObject bulletHole = Instantiate(bulletHoleDecal, contact.point + contact.normal * .001f, Quaternion.identity); // Spawn the bullethole decal.

            bulletHole.transform.localScale = new Vector3(.1f, .1f, .1f);
            bulletHole.transform.parent = objectTransform; // Parent the decal to the object that was hit.
            bulletHole.transform.LookAt(contact.point + contact.normal); // Reposition the decal to be oriented on the surface of the hit object.

            if (objectTransform.tag == "Projectile")
            {
                // Go to next itteration if the bullet has collided with another bullet.
                bounceCount--;
                continue;
            }

            if (objectTransform.CompareTag("Enemy"))
            {
                EnemyController ec = objectTransform.gameObject.GetComponent<EnemyController>();

                if (ec != null)
                {
                    ec.TakeDamage(gunInfo.damage);
                }
                Destroy(gameObject);
                return;
            }

            Destroy(bulletHole, 5f);
        }

        // Destroy the physical projectile if it has bounced more times than it is allowed to.
        if (gunInfo.maxProjectileBounces <= bounceCount)
        {
            Destroy(gameObject);
        }
    }
}
