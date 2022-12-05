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
    public Transform cameraHolder;

    // Locals
    Vector3 enemyPos;
    Rigidbody rb;
    Vector3 hitPos;
    EnemyController enemyController;
    Quaternion cameraRot;
    Quaternion targetCameraRot;
    //float cameraReturnAmount = 10f;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        cameraRot = cameraHolder.rotation;
    }

    //private void Update()
    //{
    //    targetCameraRot = Quaternion.Slerp(targetCameraRot, cameraRot, Time.deltaTime * cameraReturnAmount);
    //    cameraHolder.rotation = targetCameraRot;
    //}

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

            //Quaternion affectedCamRot = Quaternion.Euler(Random.Range(0f, enemyController.enemyDamage), Random.Range(0f, enemyController.enemyDamage), Random.Range(0f, enemyController.enemyDamage));
            //print("Camera movement being applied to simulate being hit by enemy, angles are: " + affectedCamRot);
            //targetCameraRot *= affectedCamRot; // Add the rotation to the usual rotation of the camera.
        }
    }

    GameObject ReturnParent(GameObject obj) // Returns the ultimate parent of a gameobject.
    {
        GameObject go = obj.transform.parent.gameObject;
        if (go.transform.parent != null)
            go = ReturnParent(go);
        return go;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        print("Player taken damage, current health = " + currentHealth);
        Vector3 directionOfAttack = (enemyPos - gameObject.transform.position).normalized;
        rb.AddForceAtPosition(directionOfAttack * enemyController.enemyKnockback, hitPos, ForceMode.Impulse);
        Debug.DrawRay(enemyPos, directionOfAttack, Color.red, 20f);

        if(currentHealth <= 0) // Player dead.
        {
            // Do something to kill the player...
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPos, .1f);
    }
}
