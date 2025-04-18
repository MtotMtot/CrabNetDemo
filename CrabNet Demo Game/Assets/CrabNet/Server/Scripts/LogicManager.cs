using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    //  Instance of the logic manager variable.
    public static LogicManager instance;

    // sector 1 and 2 clear bool.
    public bool Sector1Clear = false;
    public bool Sector2Clear = false;

    // The sector 1 door.
    [SerializeField]
    private DoorMove door1;

    // The sector 2 door.
    [SerializeField]
    private DoorMove door2;

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
    
    // Find the sector 1 door and assign it to the door1 variable.
    private void Start()
    {
        door1 = GameObject.FindWithTag("Sector1Door").GetComponent<DoorMove>();
        door2 = GameObject.FindWithTag("Sector2Door").GetComponent<DoorMove>();
    }

    void Update()
    {
        // If the sector 1 is clear, send the state to the logic server.
        if (Sector1Clear)
        {
            ServerSend.Sector1State(Sector1Clear);
        }
        if (Sector2Clear)
        {
            ServerSend.Sector2State(Sector2Clear);
        }
    }

    // open sector 1 door, called by logic server in server handle
    public void OpenSector1Door()
    {
        // Destroy the sector 1 door logic instance and open the door.
        Sector1DoorLogic.instance.DestroySelf();
        door1.OpenDoor();

    }

    // open sector 2 door, called by logic server in server handle
    public void OpenSector2Door()
    {
        Sector2DoorLogic.instance.DestroySelf();
        door2.OpenDoor();
    }
}
