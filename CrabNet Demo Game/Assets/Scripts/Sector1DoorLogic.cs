using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector1DoorLogic : MonoBehaviour
{
    //HashSet of active enemies.
    public HashSet<GameObject> activeEnemies = new HashSet<GameObject>();

    //Instance reference of the door logic object.
    public static Sector1DoorLogic instance;

    //Instance of the door logic object.
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
        // Find all enemies in the scene.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // check layer of each enemy, if layer is 9 (relevant to game logic), add to active enemies.
        foreach (GameObject enemy in enemies)
        {
            if (enemy.layer == 9)
            {
                activeEnemies.Add(enemy);
            }
        }
    }

    //Used to destroy the door logic object to stop the logic manager from checking for enemies.
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if any previously tracked enemies are no longer active

        activeEnemies.RemoveWhere(enemy => enemy == null || !enemy.activeInHierarchy);

        // if hashset is empty and logic manager exists, set sector 1 clear to true.
        if (activeEnemies.Count == 0 && LogicManager.instance != null)
        {
            LogicManager.instance.Sector1Clear = true;
        }
    }
}
