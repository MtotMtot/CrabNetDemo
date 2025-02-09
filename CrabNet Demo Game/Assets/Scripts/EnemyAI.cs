using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    // #### needs to be list of players for multiplayer ####
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    private float walkTimeOut;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        // #### Will need to add to list forEach player in scene (3 for now) ####
        player = GameObject.FindWithTag("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        // Debug ray
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.yellow);
    }

    private void Patroling()
    {
        if (!walkPointSet || walkTimeOut <= 0) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        walkTimeOut -= Time.deltaTime;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if ( Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && Physics.Linecast(transform.position, walkPoint, whatIsGround))
            walkPointSet = true;
            walkTimeOut = 5f;
    }

    /// <summary>
    /// 
    /// Will need target player function here, can be done simply by closest player...
    /// For more complexity can add agro number, player actions add to this number for each player. highest number is target.
    /// (e.g. closest current player adds +10 every 1s. player hitting adds +20.    checks highest agro number every 5 second to avoid jittering between targets)
    /// 
    /// </summary>

    private void ChasePlayer()
    {
        // #### will need to change to chase a chosen player from player list to function correctly in multipayer ####
        agent.SetDestination(player.position);
        // #### also needs to change to attack chosen player from player list. ####
        transform.LookAt(player);
    }


    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        // #### also needs to change to attack chosen player from player list. ####
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            alreadyAttacked = true;
            StartCoroutine(BurstAttack());
            //rb.AddForce(transform.up * 4f, ForceMode.Impulse);
            ///End of attack code
        }
    }

    private IEnumerator BurstAttack()
    {
        for (int i = 0; i < 3; i++)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 80f, ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);
        }

        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    // visualises behaviour sphere sizes. Helps debug + tweak
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
