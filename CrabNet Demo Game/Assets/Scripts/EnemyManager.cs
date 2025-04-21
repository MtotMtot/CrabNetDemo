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

    // boss prefab reference
    public GameObject bossPrefab;

    // spawn points array reference.
    public GameObject[] spawnPoints;

    public bool isHost = false;

    public GameObject[] LogicManagers;

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

    private void Start()
    {
        // if spawn points are not set, find all enemy spawns in the scene.
        if (spawnPoints.Length == 0)
        {
            spawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
        }
    }

    /// <summary>
    /// Used by connected clients to spawn in all enemies from host.
     /// </summary>
    /// <param name="_id"></param>
    /// <param name="_spawnPos"></param>
    public GameObject SpawnEnemy(int _id, Vector3 _spawnPos)
    {
        // if enemy id alrady exists in dictinoary, dont spawn
        if (!enemies.ContainsKey(_id))
        {
            GameObject enemy = Instantiate(enemyPrefab, _spawnPos, Quaternion.identity);
            enemy.GetComponent<EnemyAI>().id = _id;
            enemy.GetComponent<EnemyAI>().isHost = isHost;
            enemies.Add(_id, enemy);
            return enemy;
        }
        return null;
    }

    /// <summary>
    /// Spawns the boss enemy.
    /// </summary>
    /// <param name="_id">The ID of the boss.</param>
    /// <param name="_spawnPos">The position to spawn the boss.</param>
    /// <returns>The spawned boss game object.</returns>
    public GameObject SpawnBoss(int _id, Vector3 _spawnPos)
    {
        if (!enemies.ContainsKey(_id))
        {
            GameObject boss = Instantiate(bossPrefab, _spawnPos, Quaternion.identity);
            boss.GetComponent<EnemyAI>().id = enemies.Count + 1;
            boss.GetComponent<EnemyAI>().isHost = isHost;
            enemies.Add(_id, boss);
            return boss;
        }
        return null;
    }

    /// <summary>
    /// Used by host to spawnall enemies initially.
    /// </summary>
    public void SpawnEnemies()
    {
        int i = 0;
        foreach (GameObject spawnPoint in spawnPoints)
        {
            // spawn enemy at every spawn point and assign ID.
            GameObject enemy = SpawnEnemy(i++, spawnPoint.transform.position);
            if (enemy != null)
            {
                enemy.layer = spawnPoint.layer;
            }
        }
        SpawnBoss(enemies.Count + 1, GameObject.FindWithTag("BossSpawn").transform.position);

        // activate all logic managers.
        foreach (GameObject logicManager in LogicManagers)
        {
            logicManager.SetActive(true);
        }
    }
}
