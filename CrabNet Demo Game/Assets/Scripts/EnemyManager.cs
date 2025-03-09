using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static Dictionary<int, GameObject> enemies = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            // add all enemies in scene to the dictionary
            enemies.Add(i++, enemy);
            enemy.GetComponent<EnemyAI>().id = i;
        }
    }
}
