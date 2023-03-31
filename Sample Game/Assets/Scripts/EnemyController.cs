using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[RequireComponent(typeof(Rigidbody))] // Require a Rigidbody.
[RequireComponent(typeof(NavMeshAgent))] // Require a NavMeshAgent.
[RequireComponent(typeof(CapsuleCollider))] // Require a capsule collider.
[RequireComponent(typeof(Animator))] // Require an animator component.

public class EnemyController : MonoBehaviour
{
    [Header("Enemy attributes")]
    public float lookRadius;
    public int maxHealth;
    public int enemyDamage;
    public int enemyKnockback;
    public int minCoinsToDrop, maxCoinsToDrop;
    public float enemySpeed = 10f;
    public enum EnemyType { Grower, Duper };
    public EnemyType type;
    public DupeAttributes dupeAttributes;
    public GameObject labelPrefab;
    GameObject enemyLabel;
    TMPro.TextMeshPro labelTMP;

    [Header("References")]
    public ParticleSystem bloodEffects;
    public Animator characterAnimator;
    public PlayerDamage playerDamage;
    public GameManager gameManager;
    public GameObject coin;



    // Locals
    bool alive = true;
    bool frozen = false;
    public bool ragdolled = false;
    public float currentHealth;
    Transform target;
    Rigidbody rb;
    NavMeshAgent agent;
    Vector3 agentPos;
    Vector3 originalPos;
    Transform[] originalPositions;
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

        enemyLabel = Instantiate(labelPrefab, transform.position + Vector3.up * 5f, Quaternion.identity, transform);
        labelTMP = enemyLabel.GetComponent<TMPro.TextMeshPro>();
        labelTMP.text = gameObject.name;

        // Original positions.
        int childrenCount = transform.childCount; // Remember how many child objects there are so we can loop through them and reset the positions when we disable the ragdoll.
        originalPositions = new Transform[childrenCount];
        if (childrenCount > 0) // Remember all of the ememy object's local positions when disabling the ragdoll effect.
        {

            for (int i = 0; i < childrenCount; i++)
            {
                originalPositions[i] = transform.GetChild(i).transform; // Set the position of the object to the original position.
                print("START: " + transform.GetChild(i).transform.name + " position =  " + transform.GetChild(i).transform.localPosition);
            }
        }

        // Set the speed of the enemy.
        //gameObject.GetComponent<EnemyController>().agent.speed = enemySpeed;

        // Grab the components
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        characterAnimator = GetComponent<Animator>();

        // Set the tag of the gameobject to be "Enemy"
        gameObject.tag = "Enemy";
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        target = playerObj.transform;
        playerDamage = playerObj.GetComponent<PlayerDamage>();
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
        ParticleSystem ps = Instantiate(bloodEffects, pos, transform.rotation); // Spawn in the particle system to make it look like enemy is bleeding.
        ps.transform.SetParent(transform);
        particles.Add(ps);
        Destroy(ps, 2f);
    }

    public void StopBleed()
    {
        foreach (var ps in particles)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Destroy(ps);
        }
    }

    public void Chase(bool YN)
    {
        if (YN)
            agent.SetDestination(target.position); // Set the enemy to move towards the player's current position.

        if (characterAnimator != null)
            characterAnimator.SetBool("Running", YN);
    }

    public void AttackConnected() // Triggered by animation event trigger.
    {
        float distance = Vector3.Distance(target.position, transform.position); // Calculate the distance from the enemy to the player.

        if (distance <= lookRadius)
            playerDamage.TakeDamage(enemyDamage); // Inflict damage on the player if they are still within range when the enemy attacks.
    }

    public void Attack(bool YN)
    {
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("Attacking", YN);
            StartCoroutine(WaitForCurrentAnimation());
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

        if (currentHealth <= 0f)
            StartCoroutine(Die());
        else
            PlayHitAnimation();
        //EnemyTypeBehaviour();
    }

    IEnumerator WaitForCurrentAnimation()
    {
        yield return new WaitForSeconds(characterAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    public IEnumerator FreezeEnemy(float duration)
    {
        agent.isStopped = true;
        agent.enabled = false;
        GetComponent<Animator>().enabled = false;
        frozen = true;
        rb.isKinematic = true;

        yield return new WaitForSeconds(duration);


        agent.enabled = true;
        agent.isStopped = false;
        GetComponent<Animator>().enabled = true;
        frozen = false;
        rb.isKinematic = false;
    }

    public IEnumerator Die()
    {
        // Somewhere in here there is an error occurring which means that the enemy never really 'dies'.

        if (alive)
        {
            alive = false;
            Ragdoll(); // enable the ragdoll on death.
            DropCoins(); // Drop the enemies loot.
            gameManager.KillEnemy(); // Remove 1 from the number of enemies that are spawned in this round.
            Destroy(gameObject, 30f); // Despawn enemy after some time has elapsed.
            labelTMP.text = gameObject.name + " DEAD";
            yield return null;
        }
    }

    public void DropCoins()
    {
        int coinsToDrop = (int)Random.Range(minCoinsToDrop, maxCoinsToDrop);
        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(0f, 5f), 1f, Random.Range(0f, 5f));
            Instantiate(coin, spawnPos, Quaternion.identity);
        }
    }

    public void Ragdoll()
    {
        int childrenCount = transform.childCount; // Remember how many child objects there are so we can loop through them and reset the positions when we disable the ragdoll.

        try
        {
            Destroy(gameObject.GetComponent<NavMeshAgent>());
            //agentPos = agent.transform.position;
            //agent.enabled = false; // Enable / disable the enemy's Nav Mesh Agent.
            //agent.isStopped = true; // Toggle the enemy movement.
            //agent.ResetPath(); // Un-set the target destination (will re-set in Update() if they are being un-ragdolled)

            StopBleed(); // Despawn all of the particle effects that make the enemy look like he's bleeding.
            characterAnimator.StopPlayback(); // Stop the animator.
            GetComponent<Animator>().enabled = false; // Toggle the animator component.

            ToggleEnemyColliders(true); // Toggle the colliders.
            ToggleEnemyRigidbodies(true); // Toggle the rigidbody state.
            ragdolled = true; // Update the bool flag so we know the enmy is ragdolled.
        }
        catch { Destroy(gameObject); };
    }

    void ToggleEnemyColliders(bool state)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = state;
        }
        gameObject.GetComponent<Collider>().enabled = !state;
    }

    void ToggleEnemyRigidbodies(bool state)
    {
        Rigidbody[] rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = !state;
        }
        gameObject.GetComponent<Rigidbody>().isKinematic = state;
    }

    public void PermanentStopEnemy() // Triggered by an animation event on the enemy character's death animation.
    {
        this.enabled = false;
    }

    public void StopEnemy(bool stop)
    {
        agent.isStopped = stop;
        agent.enabled = !stop;
        //characterAnimator.enabled = stop;

        if (stop)
            agentDisableLocations.Add(transform.position);
        else agentEnableLocations.Add(transform.position);
    }

    private void Update()
    {
        Profiler.BeginSample("EnemyController: Update()");
        if (alive) // Enemy must be alive....
        {
            float distance = Vector3.Distance(target.position, transform.position); // Calculate the distance from the enemy to the player.
            if (distance - 1 <= lookRadius && !frozen && !ragdolled) // If the player is within the look radius (minus 1 because there's sime jitter)...
            {
                FaceTarget();   // Face the player.
                Chase(true);    // Chase the player

                if (agent.remainingDistance + .5f <= agent.stoppingDistance && agent.hasPath) // Check if the distance that the enemy still has to run is less than or equal to the stopping distance and stop them if it is.
                    StopEnemy(true);
                else if (agent.remainingDistance >= agent.stoppingDistance)
                    StopEnemy(false);

                if (distance - 2 <= agent.stoppingDistance) // If the enemy is within attacking range then face the target and 
                {
                    Chase(false);   // Stop chasing the player.
                    Attack(true);   // Attack the player.
                }
                else
                {
                    Attack(false);  // Stop attacking the player.
                }
            }
            else Chase(false); // Stop the chasing animation.
        }
        Profiler.EndSample();
    }

    void FaceTarget() // Makes the enemy face towards the player.
    {
        if (alive && !ragdolled)
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
        Gizmos.DrawSphere(transform.position, .5f); // Enemy
        Gizmos.DrawWireSphere(gameObject.transform.position, lookRadius); // Sphere where the enemy can see and detect the player.
        foreach (var pos in hitPositions)
        {
            Gizmos.DrawSphere(pos, .05f);
        }

        // Green
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(agent.destination, .7f);
        Gizmos.DrawWireSphere(gameObject.transform.position, agent.remainingDistance); // Sphere where the enemy can see and detect the player.

        // Blue
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(target.position, .7f);
        Gizmos.DrawWireSphere(target.transform.position, agent.stoppingDistance); // Sphere where the enemy can see and detect the player.

        // Black
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(originalPos, .7f);
        Gizmos.DrawSphere(agentPos, .7f);
        Gizmos.DrawSphere(transform.position, .7f);

        // Grey
        Gizmos.color = Color.grey;
        foreach (var pos in originalPositions)
        {
            Gizmos.DrawSphere(pos.position, .7f);
        }
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