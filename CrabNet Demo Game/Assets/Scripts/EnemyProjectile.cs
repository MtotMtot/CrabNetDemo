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

    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
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
