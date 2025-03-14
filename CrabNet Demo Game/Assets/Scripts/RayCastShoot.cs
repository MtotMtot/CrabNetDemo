using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastShoot : MonoBehaviour
{
    // Bullet Trail script reference
    [SerializeField]
    private BulletTrail bulletTrail;

    // bullet spawn reference.
    [SerializeField]
    private Transform bulletSpawnPoint;

    // shoot delay serialized field.
    [SerializeField]
    private float shootDelay;

    // damage serialized field.
    [SerializeField]
    private float damage;

    // lastShootTime reference
    private float lastShootTime;

    //  ClientSend variable
    public Transform hitTransform;
    
    /// <summary>
    /// Shoot raycast, call trail render script for visual effects.
    /// </summary>
    public void Shoot()
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            bulletTrail.Shoot(shootDelay);

            lastShootTime = Time.time;

            // if successful hit, check if enemy.
            if (Physics.Raycast(bulletSpawnPoint.position, bulletSpawnPoint.forward, out RaycastHit hit) && hit.transform.CompareTag("Enemy"))
            {
                // deal damage to hit enemy locally.
                EnemyAI enemy = hit.transform.gameObject.GetComponentInParent<EnemyAI>();
                enemy.TakeDamage(damage);

                // Send deal damage for enemy(id) to server.
                ClientSend.EnemyDamaged(enemy.id, damage);
            }
        } 
    }
}
