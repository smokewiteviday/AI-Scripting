using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerBehavior : MonoBehaviour

{
      public float fireRate = 1f; // Time between shots
    public Transform firePoint; // Where the projectile will spawn
    public GameObject projectilePrefab; // Prefab of the projectile to spawn

    private float fireCountdown = 0f;

    void Update()
    {
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}


