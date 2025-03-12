using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //enemy distioanry reference
    public static Dictionary<int, GameObject> enemies = new Dictionary<int, GameObject>();

    // EnemyManager instance
    public static EnemyManager instance;

    // enemy prefab reference
    public GameObject enemyPrefab;

    // spawn points array
    public Transform[] spawnPoints;

    public bool isHost = false;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnEnemy(int _id, Vector3 _spawnPos)
    {
        GameObject enemy = Instantiate(enemyPrefab, _spawnPos, Quaternion.identity);
        enemy.GetComponent<EnemyAI>().id = _id;
        enemy.GetComponent<EnemyAI>().isHost = isHost;
        enemies.Add(_id, enemy);
    }
    public void SpawnEnemies()
    {
        int i = 0;
        foreach (Transform spawnPoint in spawnPoints)
        {
            // spawn enemy at every spawn point and assign ID.
            SpawnEnemy(i++, spawnPoint.position);

            // Send newly spawned enemy to all clients
            ServerSend.SpawnEnemy(Client.instance.myId, i, spawnPoint.position);
        }
    }
}
