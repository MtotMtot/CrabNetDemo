using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    [SerializeField]
    private Sector1DoorLogic sector1DoorLogic;

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

    void Update()
    {
        if (Sector1Clear)
        {
            // server send sector 1 clear to logic server
        }
    }

    // open sector 1 door, called by logic server in server handle
    public void OpenSector1Door()
    {
        door1.OpenDoor();
    }
}
