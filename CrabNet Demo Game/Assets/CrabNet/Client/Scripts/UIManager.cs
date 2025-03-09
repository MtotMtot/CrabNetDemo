using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;

    private void Awake()    //set instance to this, destroy copies.
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

    public void ConnectToServer()   //disables UI and connects to target IP (server).
    {
        //NetworkManager.instance.gameObject.SetActive(false);
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    public void HostServer()
    {
        NetworkManager.instance.StartServer();
        NetworkManager.instance.isHost = true;
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
        for (int i = 0; i < EnemyManager.enemies.Count; i++)
        {
            EnemyManager.enemies[i].GetComponent<EnemyAI>().isHost = true;
        }
    }
}
