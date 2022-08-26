using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy attributes")]
    public float lookRadius = 10f;
    public int maxHealth;
    public enum EnemyType { Grower, Duper };
    public EnemyType type;
    public DupeAttributes dupeAttributes;

    [Header("References")]
    public ParticleSystem bloodEffects;
    public Animator characterAnimator;

    // Locals
    bool alive = true;
    public float currentHealth;
    Transform target;
    NavMeshAgent agent;
    List<ParticleSystem> particles = new List<ParticleSystem>();

    // Debugging vars
    List<Vector3> hitPositions = new List<Vector3>();
    List<Vector3> agentEnableLocations = new List<Vector3>();
    List<Vector3> agentDisableLocations = new List<Vector3>();

    private void OnValidate()
    {
        if (type == EnemyType.Grower)
            dupeAttributes = new DupeAttributes();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Enemy";
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.baseOffset = 0;
        currentHealth = maxHealth;
        dupeAttributes.duplicated = false;
        agent.ResetPath();
    }

    public void EnemyTypeBehaviour()
    {
        Vector3 scale = gameObject.transform.localScale;

        if (type == EnemyType.Grower) // Grower makes the enemy grow each time.
        {
            float growFactor = .2f;
            scale += new Vector3(growFactor, growFactor, growFactor);
            gameObject.transform.localScale = scale;
        }
        else if (type == EnemyType.Duper && !dupeAttributes.duplicated && dupeAttributes.maxDupes > dupeAttributes.dupeCount) // Duper makes the enemy duplicate each time
        {
            // Adjust the scale
            scale *= dupeAttributes.scaleFactor;
            gameObject.transform.localScale = scale;

            // Set the duplicated flag for this script so we don't dupe it over and over.
            dupeAttributes.duplicated = true;
            dupeAttributes.dupeCount++;

            // Duplicate the enemy
            GameObject dupe = Instantiate(gameObject, transform.position, Quaternion.identity);
            // Set the attributes like speed, health and the dupe count so we can keep count of how many we have duplicated so far.
            dupe.GetComponent<EnemyController>().currentHealth *= dupeAttributes.healthFactor;
            dupe.GetComponent<EnemyController>().dupeAttributes.dupeCount = dupeAttributes.dupeCount;
            dupe.GetComponent<EnemyController>().dupeAttributes.duplicated = false;
            dupe.GetComponent<EnemyController>().agent.speed *= dupeAttributes.speedFactor;
        }
    }

    public void BleedAtPosition(Vector3 pos)
    {
        hitPositions.Add(pos);
        ParticleSystem ps = Instantiate(bloodEffects, pos, transform.rotation); // Spawn in the particle system to make it look like player is bleeding.
        ps.transform.SetParent(transform);
        particles.Add(ps);
    }

    public void StopBleed()
    {
        foreach (var ps in particles)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void Chase(bool YN)
    {
        if (YN)
            agent.SetDestination(target.position); // Set the enemy to move towards the player's current position.

        if (characterAnimator != null)
            characterAnimator.SetBool("Running", YN);
    }

    public void Attack(bool YN)
    {
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("Attacking", YN);
            // StopEnemy(true);
            print("Stopped player to play the attack animation.");
            StartCoroutine(WaitForCurrentAnimation());
            print("Un-stopped player becayse the attack animation has finished.");
            // StopEnemy(false);
        }
    }

    public void PlayHitAnimation()
    {
        if (characterAnimator != null && alive)
        {
            characterAnimator.SetTrigger("Hit");
            agent.isStopped = true;
            StartCoroutine(WaitForCurrentAnimation());
            agent.isStopped = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PlayHitAnimation();

        if (currentHealth <= 0)
            StartCoroutine(Die());

        EnemyTypeBehaviour();
    }

    IEnumerator WaitForCurrentAnimation()
    {
        print("Waiting for current animation to finish.");
        yield return new WaitForSeconds(characterAnimator.GetCurrentAnimatorStateInfo(0).length);
        print("Current animation finished.");
    }

    IEnumerator Die()
    {
        print("Enemy has died");
        if (characterAnimator != null && alive)
        {
            alive = false;
            agent.isStopped = true;
            StopBleed();
            Destroy(gameObject.GetComponent<Rigidbody>());
            characterAnimator.SetTrigger("Dead");
            yield return new WaitForSeconds(30);
            Destroy(gameObject);
        }
    }

    public void PermanentStopEnemy() // Triggered by an animation event on the enemy character's death animation.
    {
        print("Stopping enemy permanently");
        this.enabled = false;
    }

    public void StopEnemy(bool stop)
    {
        agent.isStopped = stop;
        agent.enabled = !stop;

        if (stop)
            agentDisableLocations.Add(transform.position);
        else agentEnableLocations.Add(transform.position);
    }

    private void Update()
    {
        if (alive) // Enemy must be alive....
        {
            float distance = Vector3.Distance(target.position, transform.position); // Calculate the distance from the enemy to the player.
            if (distance - 1 <= lookRadius) // If the player is within the look radius (minus 1 because there's sime jitter)...
            {
                FaceTarget();   // Face the player.
                Chase(true);    // Chase the player
                print("remaining dist = " + agent.remainingDistance);

                if (agent.remainingDistance + .5f <= agent.stoppingDistance && agent.hasPath) // Check if the distance that the enemy still has to run is less than or equal to the stopping distance and stop them if it is.
                    StopEnemy(true);
                else if(agent.remainingDistance >= agent.stoppingDistance)
                    StopEnemy(false);

                if (distance - 2 <= agent.stoppingDistance) // If the enemy is within attacking range then face the target and 
                {
                    Chase(false);   // Stop chasing the player.
                    Attack(true);   // Attach the player.
                }
                else
                {
                    Attack(false);  // Stop attacking the player.
                }
            }
            else Chase(false); // Stop the chasing animation.
        }
    }

    void FaceTarget() // Makes the enemy face towards the player.
    {
        if (alive)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }



    private void OnDrawGizmosSelected() // Debugging gizmos for where the enemy was hit (world space) and where the enemy can see.
    {
        // Red
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .7f); // Enemy
        Gizmos.DrawWireSphere(gameObject.transform.position, lookRadius); // Sphere where the enemy can see and detect the player.
        foreach (var pos in hitPositions)
        {
            Gizmos.DrawSphere(pos, .05f);
        }

        // foreach (var pos in agentDisableLocations)
           // Gizmos.DrawSphere(pos, .1f);    

        // Green
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(agent.destination, .7f);
        Gizmos.DrawWireSphere(gameObject.transform.position, agent.remainingDistance); // Sphere where the enemy can see and detect the player.


        // foreach (var pos in agentEnableLocations)
            // Gizmos.DrawSphere(pos, .1f);

        // Blue
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(target.position, .7f);
        Gizmos.DrawWireSphere(target.transform.position, agent.stoppingDistance); // Sphere where the enemy can see and detect the player.
    }
}

[System.Serializable]
public struct DupeAttributes
{
    public int maxDupes;
    public int dupeCount;
    public bool duplicated;
    public float scaleFactor;
    public float speedFactor;
    public float healthFactor;
}