using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem muzzleFlash;

    [SerializeField]
    private Transform bulletSpawnPoint;

    [SerializeField]
    private ParticleSystem impactParticleSystem;

    [SerializeField]
    private TrailRenderer bulletTrail;

    [SerializeField]
    private LayerMask mask;

    private Animator animator;
    private float lastShootTime;

    private void Awake()
    {
        // get animator component.
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Shoot bullets, with (shootdelay).
    /// </summary>
    /// <param name="shootDelay"></param>
    public void Shoot(float shootDelay)
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            animator.SetBool("IsShooting", true);
            Instantiate(muzzleFlash, bulletSpawnPoint.position, bulletSpawnPoint.transform.rotation);

            Vector3 direction = transform.forward;

            // if successful collider hit on raycast, render bullet trail.
            if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, mask))
            {
                TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, hit));

                lastShootTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Bullet trail for successful collider hits, creates trail from gun to hit location, then creates splash at hit location.
    /// </summary>
    /// <param name="trail"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startpPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startpPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        animator.SetBool("IsShooting", false);
        trail.transform.position = hit.point;
        Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

}
