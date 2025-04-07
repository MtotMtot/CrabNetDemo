using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    //  Instance of the logic manager variable.
    public static LogicManager instance;

    // sector 1 is clear bool variable.
    public bool Sector1Clear = false;

    // The sector 1 door.
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
    
    // Find the sector 1 door and assign it to the door1 variable.
    private void Start()
    {
        door1 = GameObject.FindWithTag("Sector1Door").GetComponent<DoorMove>();
    }

    void Update()
    {
        // If the sector 1 is clear, send the state to the logic server.
        if (Sector1Clear)
        {
            ServerSend.Sector1State(Sector1Clear);
        }
    }

    // open sector 1 door, called by logic server in server handle
    public void OpenSector1Door()
    {
        // Destroy the sector 1 door logic instance and open the door.
        Sector1DoorLogic.instance.DestroySelf();
        door1.OpenDoor();

    }
}
