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

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider enemyCollider)
    {
        print("Collider triggered on player");
        GameObject collidedGO = ReturnParent(enemyCollider.gameObject);
        if (enemyCollider.tag == "Enemy")
        {
            print("Enemy attacked player");
            EnemyController enemyController = collidedGO.GetComponent<EnemyController>();
            TakeDamage(enemyController.enemyDamage);
        }
    }

    GameObject ReturnParent(GameObject obj)
    {
        GameObject go = obj.transform.parent.gameObject;

        if (go.transform.parent.gameObject != null)
            go = ReturnParent(go);

        return go;
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        print("Player taken damage, current health = " + currentHealth);
    }
}
