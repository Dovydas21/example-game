using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float pullRadius; // the radius within which to pull enemies
    public float pullForce; // the strength of the pull force
    List<Vector3> hitPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // Create a Raycast sphere to find all enemies within the pull radius
        Collider[] enemiesInRadius = Physics.OverlapSphere(transform.position, pullRadius, Physics.AllLayers);

        // apply pull force to each enemy
        foreach (Collider enemy in enemiesInRadius)
        {
            hitPositions.Add(enemy.transform.position);
            print("hit.collider.name = " + enemy.transform.name);
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if(enemyRb != null)
            {
                Vector3 pullDirection = (transform.position - enemy.transform.position).normalized;
                enemyRb.AddForce(pullDirection * pullForce);
            }
        }
        //hitPositions.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, pullRadius);

        foreach (var hit in hitPositions)
        {
            Gizmos.DrawSphere(hit, 1f);
        }
    }
}
