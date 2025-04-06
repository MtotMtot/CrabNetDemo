using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector1DoorLogic : MonoBehaviour
{
    private BoxCollider triggerZone;
    public HashSet<GameObject> activeEnemies = new HashSet<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Get or add BoxCollider component
        triggerZone = GetComponent<BoxCollider>();
        if (triggerZone == null)
        {
            triggerZone = gameObject.AddComponent<BoxCollider>();
            triggerZone.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any previously tracked enemies are no longer active
        activeEnemies.RemoveWhere(enemy => enemy == null || !enemy.activeInHierarchy);

        if (activeEnemies.Count == 0)
        {
            Debug.Log("No active enemies in sector 1");
            LogicManager.instance.Sector1Clear = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is an enemy
        if (other.CompareTag("Enemy"))
        {
            activeEnemies.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Remove enemy when it exits the trigger zone
        if (other.CompareTag("Enemy"))
        {
            activeEnemies.Remove(other.gameObject);
        }
    }    
}
