using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector1DoorLogic : MonoBehaviour
{
    public HashSet<GameObject> activeEnemies = new HashSet<GameObject>();

    public static Sector1DoorLogic instance;

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

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any previously tracked enemies are no longer active
        activeEnemies.RemoveWhere(enemy => enemy == null || !enemy.activeInHierarchy);

        if (activeEnemies.Count == 0 && LogicManager.instance != null)
        {
            LogicManager.instance.Sector1Clear = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("enemy entered collider");
    }
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("enemy stay in collider");
        // Check if the colliding object is an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (activeEnemies.Contains(collision.gameObject))
            {
                return;
            }
            activeEnemies.Add(collision.gameObject);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("enemy exit collider");
        // Remove enemy when it exits the collision zone
        if (collision.gameObject.CompareTag("Enemy"))
        {
            activeEnemies.Remove(collision.gameObject);
        }
    }    
}
