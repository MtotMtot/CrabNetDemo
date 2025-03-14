using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //enemy distioanry reference
    public static Dictionary<int, GameObject> enemies = new Dictionary<int, GameObject>();

    // instance reference.
    public static EnemyManager instance;

    // enemy prefab reference
    public GameObject enemyPrefab;

    // spawn points array reference.
    public Transform[] spawnPoints;

    public bool isHost = false;

    /// <summary>
    /// Singleton this.
    /// </summary>
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

    /// <summary>
    /// Used by connected clients to spawn in all enemies from host.
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_spawnPos"></param>
    public void SpawnEnemy(int _id, Vector3 _spawnPos)
    {
        // if enemy id alrady exists in dictinoary, dont spawn
        if (!enemies.ContainsKey(_id))
        {
            GameObject enemy = Instantiate(enemyPrefab, _spawnPos, Quaternion.identity);
            enemy.GetComponent<EnemyAI>().id = _id;
            enemy.GetComponent<EnemyAI>().isHost = isHost;
            enemies.Add(_id, enemy);
        }
        
    }

    /// <summary>
    /// Used by host to spawn all enemies initially.
    /// </summary>
    public void SpawnEnemies()
    {
        int i = 1;
        foreach (Transform spawnPoint in spawnPoints)
        {
            // spawn enemy at every spawn point and assign ID.
            SpawnEnemy(i++, spawnPoint.position);
        }
    }
}
