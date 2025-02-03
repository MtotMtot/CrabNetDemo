using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastShoot : MonoBehaviour
{
    // Bullet Trail script reference
    [SerializeField]
    private BulletTrail bulletTrail;

    [SerializeField]
    private Transform bulletSpawnPoint;
    [SerializeField]
    private float shootDelay;
    [SerializeField]
    private float damage;

    private float lastShootTime;

    [SerializeField]
    private LayerMask playerLayer;
    

    public void Shoot()
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            bulletTrail.Shoot(shootDelay);

            lastShootTime = Time.time;

            if (Physics.Raycast(bulletSpawnPoint.position, bulletSpawnPoint.forward, out RaycastHit hit)) // && hit.transform.CompareTag("EnemyCollider"))
            {
                // do on hit effects (damage etc)
            }
        } 
    }
}
