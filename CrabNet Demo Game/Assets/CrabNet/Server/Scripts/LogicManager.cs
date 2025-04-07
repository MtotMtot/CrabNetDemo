using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    public static LogicManager instance;

    public bool Sector1Clear = false;

    [SerializeField]
    private DoorMove door1;

    // instance logic manager
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

    private void Start()
    {
        door1 = GameObject.FindWithTag("Sector1Door").GetComponent<DoorMove>();
    }

    void Update()
    {
        if (Sector1Clear)
        {
            ServerSend.Sector1State(Sector1Clear);
            Debug.Log("Sector 1 clear Sent To Logic Server");
        }
    }

    // open sector 1 door, called by logic server in server handle
    public void OpenSector1Door()
    {
        Debug.Log("Opening sector 1 door");

        Sector1DoorLogic.instance.DestroySelf();
        door1.OpenDoor();

    }
}
