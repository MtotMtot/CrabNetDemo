using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // id reference from enemy manager
    public int id = 0;

    // target player id reference.
    public int targetId = 0;

    // Is this client host reference.
    public bool isHost = false;

    // nav mesh agent reference.
    public NavMeshAgent agent;

    // player transform reference.
    public Transform player;

    // layer masks reference.
    public LayerMask whatIsGround, whatIsPlayer;

    // health reference.
    public float health;

    // Patroling reference.
    public Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField]
    float enemyHeight;
    public float walkPointRange;
    private float walkTimeOut;

    // Attacking reference.
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States reference.
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
        if (playerInSightRange && NetworkManager.instance.isHost)
        {
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

        // Send Enemy Movement to Server every (input)ms
        Invoke("SendEnemyMovement", UIManager.instance.delay);
    }

    /// <summary>
    /// move to walkpoints.
    /// </summary>
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

    /// <summary>
    /// search for new a reachable walkpoint.
    /// </summary>
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // check if walkpoint is on ground.
        if ( Physics.Raycast(walkPoint, -transform.up, enemyHeight, whatIsGround) )
        {
            walkPointSet = true;
            walkTimeOut = 5f;
        }
            
    }

    /// <summary>
    /// check colliders in sight range for PlayerObj, get ID of player from parent.
    /// </summary>
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
    /// chase after target player
    /// </summary>
    private void ChasePlayer()
    {
        agent.SetDestination(GameManager.players[targetId].transform.position);
        transform.LookAt(GameManager.players[targetId].transform);
    }


    /// <summary>
    /// attack player when in range
    /// </summary>
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

    /// <summary>
    /// busrt attack for AttackPlayer(), shoots 3 projectiles per attack.
    /// </summary>
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

    /// <summary>
    /// Reset attack
    /// </summary>
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    /// <summary>
    /// Take damage from player
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        health -= damage;

        // if health <= 0, destroy self.
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    /// <summary>
    /// Destroy self on death
    /// </summary>
    private void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// visualises behaviour sphere sizes. Helps debug + tweak
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void SendEnemyMovement()
    {
        ServerSend.EnemyPosition(Client.instance.myId, id, transform.position);
        ServerSend.EnemyRotation(Client.instance.myId, id, transform.rotation);
    }
}
