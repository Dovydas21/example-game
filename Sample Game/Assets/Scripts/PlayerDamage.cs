using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [Header("Damage attributes")]
    public int currentHealth;
    public int maxHealth = 100;

    [Header("References")]
    public Collider playerDamageHitbox;

    // Locals
    Vector3 enemyPos;
    Rigidbody rb;
    Vector3 hitPos;
    EnemyController enemyController;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider enemyCollider)
    {
        hitPos = enemyCollider.transform.position;
        print("Collider triggered on player");
        GameObject collidedGO = ReturnParent(enemyCollider.gameObject);
        enemyPos = collidedGO.transform.position;
        if (enemyCollider.tag == "Enemy")
        {
            print("Enemy attacked player");
            enemyController = collidedGO.GetComponent<EnemyController>();
            TakeDamage(enemyController.enemyDamage);
        }
    }

    GameObject ReturnParent(GameObject obj) // Returns the ultimate parent of a gameobject.
    {
        GameObject go = obj.transform.parent.gameObject;
        if (go.transform.parent != null)
            go = ReturnParent(go);
        return go;
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        print("Player taken damage, current health = " + currentHealth);
        Vector3 directionOfAttack = (enemyPos - gameObject.transform.position).normalized;
        rb.AddForceAtPosition(directionOfAttack * enemyController.enemyKnockback, hitPos, ForceMode.Impulse);
        Debug.DrawRay(enemyPos, directionOfAttack, Color.red, 20f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPos, .1f);
    }
}
