using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // instance reference.
    public static UIManager instance;

    // start menu and input field reference.
    public GameObject startMenu;
    public GameObject pauseMenu;
    public InputField usernameField;
    public InputField ipField;
    public InputField delayField;

    // delay var for simulating lag.
    public float delay = 0;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            ChangedCursor();
        }
    }

    /// <summary>
    /// Sets the ip entered into text field to the target IP for TCP and UDP clients.
    /// </summary>
    public void SetIP()
    {
        Debug.Log(ipField.text.ToString());
        Client.instance.SetIP(ipField.text.ToString());
    }

    /// <summary>
    /// disables UI and connects to target IP (server).
    /// </summary>
    public void ConnectToServer()   
    {
        usernameField.interactable = false;
        ipField.interactable = false;

        // set menu to inactive, connect to server
        SetIP();
        Client.instance.ConnectToServer();

        // disable startMenu and InputField
        startMenu.SetActive(false);

        // Create LogicManager and enable related objects for Client here
        NetworkManager.instance.InstantiateLogicManager();
        EnemyManager.instance.EnableLogicManagers();

    }

    /// <summary>
    /// Host Server button fucntion
    /// disables UI, starts the server, spawns enemies and connect this client to the server.
    /// </summary>
    public void HostServer()
    {
        usernameField.interactable = false;
        ipField.interactable = false;

        // start server, set isHost to true
        NetworkManager.instance.StartServer();
        NetworkManager.instance.isHost = true;

        // tell this enemyManager its on host, spawn all enemies
        EnemyManager.instance.isHost = true;
        EnemyManager.instance.SpawnEnemies();

        // connect to server (this client)
        Client.instance.ConnectToServer();

        // disable startMenu and InputField
        startMenu.SetActive(false);
    }

    /// <summary>
    /// Leaves the server, if host: stops the server.
    /// Reloads scene to allow new connetions.
    /// </summary>
    public void LeaveServer()
    {   
        // Properly Handle Disconnecting and Stopping Server.
        Client.instance.Disconnect();
        // Stop the server. Try and catch to handle any errors (if the client is not host
        try
        {
            Server.Stop();
        }
        catch (Exception e)
        {
            Debug.LogError("Error stopping server: " + e.Message);
        }
        ChangedCursor();

        // Reload the current scene to reset.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// Closes pause menu
    /// </summary>
    public void Resume()
    {
        // disable pauseMenu
        pauseMenu.SetActive(false);
        ChangedCursor();
    }

    /// <summary>
    /// Changes cursor when entering and exiting pause menu.
    /// </summary>
    private void ChangedCursor()
    {
        if (pauseMenu.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Sets delay from inputField (ms).
    /// </summary>
    public void SetDelay()
    {
        // temp var for getting float value from inputField.
        float temp;
        float.TryParse(delayField.text.ToString(), out temp);

        // temp / 1000 since delay is in ms.
        delay = temp/1000;
    }
}
