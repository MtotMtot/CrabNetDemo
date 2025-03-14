using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // projectile damage
    public float damage;

    // projectile life time
    [SerializeField]
    private float lifeTime;

    /// <summary>
    /// reduce lifetime of projectile by time passed, destroy when <= 0.
    /// </summary>
    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Check when collider enters this trigger.
    /// </summary>
    /// <param name="coll"></param>
    private void OnTriggerEnter(Collider coll)
    {
        // if playerObj collider, do damage to player then destroy self, if anything else destroy self.
        if (coll.gameObject.tag == "PlayerObj")
        {
            PlayerHealth playerHealth = coll.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damage);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
