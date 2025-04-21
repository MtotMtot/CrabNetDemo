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
    /// disables UI and connects to target IP (server).
    /// </summary>
    public void ConnectToServer()   
    {
        usernameField.interactable = false;
        ipField.interactable = false;

        // set menu to inactive, connect to server
        Client.instance.SetIP(ipField.text);
        Client.instance.ConnectToServer();

        // disable startMenu and InputField
        startMenu.SetActive(false);
        
    }

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

    public void Resume()
    {
        // disable pauseMenu
        pauseMenu.SetActive(false);
        ChangedCursor();
    }

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
}
