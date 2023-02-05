using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float pullRadius = 40f;      // the radius within which to pull enemies
    public float pullForce = 1000f;     // the strength of the pull force
    public float lifeTime = 30f;        // the number of seconds until the black hole despawns.

    private void Start()
    {
        StartCoroutine(DestroyBlackHole());
    }

    void FixedUpdate()
    {
        // Create a Raycast sphere to find all enemies within the pull radius
        Collider[] enemiesInRadius = Physics.OverlapSphere(transform.position, pullRadius, Physics.AllLayers);

        // apply pull force to each enemy
        foreach (Collider enemy in enemiesInRadius)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                if (enemy.transform.tag == "Enemy")
                {
                    //enemy.GetComponent<EnemyController>().StartCoroutine(enemy.GetComponent<EnemyController>().Die());
                    enemy.GetComponent<EnemyController>().ToggleRagdoll(true);
                }

                Vector3 pullDirection = (transform.position - enemy.transform.position).normalized;
                enemyRb.AddForce(pullDirection * pullForce);
            }
        }
    }

    public IEnumerator DestroyBlackHole()
    {
        yield return new WaitForSeconds(lifeTime);
        print("Destroying black hole...");

        Collider[] enemiesInRadius = Physics.OverlapSphere(transform.position, pullRadius, Physics.AllLayers);
        foreach (Collider enemy in enemiesInRadius)
        {
            if (enemy.transform.tag == "Enemy")
            {
                //enemy.GetComponent<EnemyController>().StartCoroutine(enemy.GetComponent<EnemyController>().Die());
                enemy.GetComponent<EnemyController>().ToggleRagdoll(false);
            }
        }
        Destroy(gameObject); // Destroy the parent object.
    }
}
