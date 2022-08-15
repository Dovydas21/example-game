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
    public Animator characterAnimation;
    public Animation deathAnimation;


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
        if (currentHealth <= 0)
            Die();
        StartCoroutine(PlayHitAnimation());
    }

    public void BleedAtPosition(Vector3 pos)
    {
        hitPositions.Add(pos);
        ParticleSystem ps = Instantiate(bloodEffects, pos, transform.rotation); // Spawn in the particle system to make it look like player is bleeding.
        ps.transform.SetParent(transform);
        // ps.Play();
    }

    public void Chase(bool YN)
    {
        if (characterAnimation != null)
            characterAnimation.SetBool("Running", YN);
    }

    public void Attack(bool YN)
    {
        if (characterAnimation != null)
            characterAnimation.SetBool("Attacking", YN);
    }

    public void Die()
    {
        if (characterAnimation != null)
        {
            characterAnimation.SetTrigger("Dead");
            if (deathAnimation != null)
                StartCoroutine(WaitForAnimation(deathAnimation));
        }
        agent.isStopped = true;
    }

    IEnumerator PlayHitAnimation()
    {
        if (characterAnimation != null)
        {
            characterAnimation.SetTrigger("Hit");
            agent.isStopped = true;
            while (!characterAnimation.GetCurrentAnimatorStateInfo(0).IsName("HitReaction"))
                yield return new WaitForSeconds(.01f);
            agent.isStopped = false;
        }
    }

    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return new WaitForSeconds(.1f);
        } while (animation.isPlaying);
    }

    private void Update()
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
                // Attack player.
            }
            else Attack(false);
        }
        else
        {
            Chase(false); // Stop the chasing animation.
        }
    }



    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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
