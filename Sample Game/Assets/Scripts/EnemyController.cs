using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy attributes")]
    public float lookRadius = 10f;
    public int maxHealth;
    int currentHealth;

    public ParticleSystem bloodEffects;
    public Animator characterAnimator;
    public Collider playerCollider;

    bool alive = true;



    Transform target;
    NavMeshAgent agent;
    List<Vector3> hitPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PlayHitAnimation();

        if (currentHealth <= 0)
            StartCoroutine(Die());
        
    }

    public void BleedAtPosition(Vector3 pos)
    {
        hitPositions.Add(pos);
        ParticleSystem ps = Instantiate(bloodEffects, pos, transform.rotation); // Spawn in the particle system to make it look like player is bleeding.
        ps.transform.SetParent(transform);
    }

    public void Chase(bool YN)
    {
        if (characterAnimator != null)
            characterAnimator.SetBool("Running", YN);
    }

    public void Attack(bool YN)
    {
        if (characterAnimator != null)
            characterAnimator.SetBool("Attacking", YN);
    }

    IEnumerator Die()
    {
        print("Enemy has died");
        if (characterAnimator != null && alive)
        {
            alive = false;
            agent.isStopped = true;
            Destroy(playerCollider);
            Destroy(gameObject.GetComponent<Rigidbody>());
            characterAnimator.SetTrigger("Dead");
            yield return new WaitForSeconds(characterAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
    }




    IEnumerator PlayHitAnimation()
    {
        if (characterAnimator != null && alive)
        {
            characterAnimator.SetTrigger("Hit");
            agent.isStopped = true;
            yield return new WaitForSeconds(characterAnimator.GetCurrentAnimatorStateInfo(0).length);
            agent.isStopped = false;
        }
    }

    public void PermanentStopEnemy()
    {
        print("Stopping enemy permanently");
        // Triggered by an animation event on the enemy character's death animation.
        this.enabled = false;
    }

    private void Update()
    {
        if (alive)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius)
            {
                agent.SetDestination(target.position);
                Chase(true); // Start the chasing animation.

                if (distance <= agent.stoppingDistance)
                {
                    FaceTarget();
                    Attack(true);
                }
                else Attack(false);
            }
            else
            {
                Chase(false); // Stop the chasing animation.
            }
        }
    }



    void FaceTarget()
    {
        if (alive)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, lookRadius);

        foreach (var pos in hitPositions)
        {
            Gizmos.DrawSphere(pos, .05f);
        }
    }
}



