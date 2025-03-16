using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    // instance reference.
    public static UIManager instance;

    // start menu and input field reference.
    public GameObject startMenu;
    public InputField usernameField;

    /// <summary>
    /// Singleton this.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    /// <summary>
    /// disables UI and connects to target IP (server).
    /// </summary>
    public void ConnectToServer()   
    {
        // set menu to inactive, connect to server
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    public void HostServer()
    {
        // start server, set isHost to true
        NetworkManager.instance.StartServer();
        NetworkManager.instance.isHost = true;

        // tell this enemyManager its on host, spawn all enemies
        EnemyManager.instance.isHost = true;
        EnemyManager.instance.SpawnEnemies();

        // disable startMenu and InputField
        startMenu.SetActive(false);
        usernameField.interactable = false;

        // connect to server (this client)
        Client.instance.ConnectToServer();

    }
}
