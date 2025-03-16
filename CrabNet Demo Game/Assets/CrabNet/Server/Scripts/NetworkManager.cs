using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // instance reference
    public static NetworkManager instance;

    // Server instance prefab reference
    public GameObject playerPrefab;

    // isHost bool for enemyManager
    public bool isHost = false;
    //playerSpawn list
    public GameObject[] playerSpawns;

    // singleton network manager
    private void Awake()
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

    // find all player spawn points if not added to list manually
    private void Start()
    {
        if (playerSpawns.Length == 0)
        {
            playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        }
    }

    // start server with target framrate, with (player count, port)
    public void StartServer()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        Server.Start(50, 26950);
    }

    // stop server when closing app
    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    // instantiate player prefab
    public Player InstantiatePlayer()
    {
        if(playerSpawns.Length == 0)
        {
            return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        }
        else
        {
            return Instantiate(playerPrefab, playerSpawns[Random.Range(0,playerSpawns.Length)].transform.position, Quaternion.identity).GetComponent<Player>();
        }
    }
}
