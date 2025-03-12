using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // id reference from enemy manager
    public int id = 0;

    // target player id
    public int targetId = 0;

    // is this client host
    public bool isHost = false;

    //nav mesh agent
    public NavMeshAgent agent;

    //player transform
    public Transform player;

    //layer masks
    public LayerMask whatIsGround, whatIsPlayer;

    //health
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
        // get navmesh agent
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // if player is in range, send their tagert id to all clients for their enemy instance.
        if (playerInSightRange && NetworkManager.instance.isHost){
            // get player id and send to server if host client
            GetPlayerId();
            ServerSend.EnemyTarget(id, targetId);
        }

        // enemy behaviour states.
        if (!playerInSightRange && !playerInAttackRange && NetworkManager.instance.isHost){
            Patroling();
        }
        if (playerInSightRange && !playerInAttackRange && targetId != 0){
            ChasePlayer();
        }
        if (playerInAttackRange && playerInSightRange && targetId != 0)
        {
            AttackPlayer();
        }

        // Debug ray
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.yellow);

        ServerSend.EnemyPosition(Client.instance.myId, id, transform.position);
        ServerSend.EnemyRotation(Client.instance.myId, id, transform.rotation);
    }

    // move to walkpoints.
    private void Patroling()
    {
        if (!walkPointSet || walkTimeOut <= 0)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        walkTimeOut -= Time.deltaTime;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    // search for new a reachable walkpoint.
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // check if walkpoint is on ground.
        if ( Physics.Raycast(walkPoint, -transform.up, 1.8f, whatIsGround) )
        {
            walkPointSet = true;
            walkTimeOut = 5f;
        }
            
    }

    //check colliders in sight range for PlayerObj, get ID of player from parent.
    private void GetPlayerId()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, whatIsPlayer);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("PlayerObj"))
            {
                targetId = collider.gameObject.GetComponentInParent<PlayerManager>().id;
            }
        }
    }

    /// <summary>
    /// 
    /// can have target player function here, can be done simply by closest player...
    /// For more complexity can add agro number, player actions add to this number for each player. highest number is target.
    /// (e.g. closest current player adds +10 every 1s. player hitting adds +20.    checks highest agro number every 5 second to avoid jittering between targets)
    /// 
    /// </summary>

    // chase after target player
    private void ChasePlayer()
    {
        // #### will need to change to chase a chosen player from player list to function correctly in multipayer ####
        agent.SetDestination(GameManager.players[targetId].transform.position);
        // #### also needs to change to attack chosen player from player list. ####
        transform.LookAt(GameManager.players[targetId].transform);
    }


    // attack player when in range
    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(GameManager.players[targetId].transform);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            StartCoroutine(BurstAttack());
        }
    }

    // busrt attack for AttackPlayer(), shoots 3 projectiles per attack.
    private IEnumerator BurstAttack()
    {
        for (int i = 0; i < 3; i++)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 80f, ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);
        }

        // use invoke to have cooldown between bursts.
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // Take damage from player
    public void TakeDamage(float damage)
    {
        health -= damage;

        // if health <= 0, destroy self.
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    // Destroy self on death
    private void DestroyEnemy()
    {
        Destroy(this.gameObject);
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
