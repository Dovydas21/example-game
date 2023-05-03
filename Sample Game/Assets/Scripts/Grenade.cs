using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{

    [SerializeField] private float throwForce = 100f;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float mass;
    [SerializeField] private float drag;
    
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            StartCoroutine(ThrowGrenade());
        }
    }

    private void FixedUpdate()
    {
        // Create a Raycast sphere to find all enemies within the pull radius
        Collider[] enemiesInRadius = Physics.OverlapSphere(transform.position, explosionRadius, Physics.AllLayers);

        // apply pull force to each enemy
        foreach (Collider enemy in enemiesInRadius)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                if (enemy.transform.tag == "Enemy")
                {
                    //enemyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    enemy.GetComponent<EnemyController>().StartCoroutine(enemy.GetComponent<EnemyController>().Die());

                    Vector3 pullDirection = (transform.position - enemy.transform.position).normalized;
                    enemyRb.AddForce(transform.position * explosionForce);
                }
            }
        }
    }

    public IEnumerator ThrowGrenade()
    {
        gameObject.transform.parent = null;

        rb.isKinematic = false;
        rb.mass = mass;
        rb.drag = drag;

        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse); // Throw the gun with some force rather than just letting it drop.
        rb.AddTorque(transform.right * throwForce, ForceMode.VelocityChange);

        yield return null;
    }
}
