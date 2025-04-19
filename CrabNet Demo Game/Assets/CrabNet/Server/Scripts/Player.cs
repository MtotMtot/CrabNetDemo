using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // id and user name reference
    public int id;
    public string username;

    // player collider reference
    public CapsuleCollider capsuleCollider;

    /// <summary>
    /// disables capsule collider of server instance of client for the host.
    /// </summary>
    void Start()
    {
        if (NetworkManager.instance != null)
        {
            capsuleCollider.enabled = false;
        }
    }

    /// <summary>
    /// initialize client data
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_username"></param>
    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        foreach (GameObject enemy in EnemyManager.enemies.Values)
        {
            // Send all enemies to new client
            if (enemy.GetComponent<EnemyAI>().agent.name == "Enemy")
            {
                ServerSend.SpawnEnemy(id, enemy.GetComponent<EnemyAI>().id, enemy.transform.position);
                Debug.Log("Sent spawn enemy");
            }
            else if (enemy.GetComponent<EnemyAI>().agent.name == "Boss")
            {
                ServerSend.SpawnBoss(id, enemy.GetComponent<EnemyAI>().id, enemy.transform.position);
                Debug.Log("Sent spawn boss");
            }
        }
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        Move();
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    private void Move()
    {
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_position">The new position.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(Vector3 _position, Quaternion _rotation)
    {
        transform.position = _position;
        transform.rotation = _rotation;
    }

    /// <summary>
    /// Tells all players this client is shooting.
    /// </summary>
    /// <param name="_camPos"></param>
    /// <param name="_camRot"></param>
    public void Shoot(Vector3 _camPos, Quaternion _camRot)
    {
        ServerSend.PlayerShoot(id, _camPos, _camRot);
    }

    /// <summary>
    /// Tells all players this client damaged an enemy.
    /// </summary>
    /// <param name="_enemyId"></param>
    /// <param name="_damage"></param>
    public void SetEnemyDamaged(int _enemyId, float _damage)
    {
        ServerSend.EnemyDamaged(id, _enemyId, _damage);
    }
}
