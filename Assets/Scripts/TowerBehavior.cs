using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerBehavior : MonoBehaviour

{
      public float fireRate = 1f; // Time between shots
    public float range = 5f; // Target detection range
    public GameObject projectilePrefab; // Projectile to fire
    public Transform firePoint; // Where the projectile is fired from

    private float fireCooldown = 0f;

    private void Update()
    {
        // Find the closest enemy
        GameObject targetEnemy = FindClosestEnemy();
        
        if (targetEnemy != null)
        {
            // Rotate to face the enemy (optional for 2D towers)
            Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Shoot if cooldown is over
            if (fireCooldown <= 0f)
            {
                Shoot(targetEnemy);
                fireCooldown = 1f / fireRate;
            }
        }

        fireCooldown -= Time.deltaTime; // Cooldown timer
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance && distance <= range)
            {
                shortestDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    private void Shoot(GameObject target)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetTarget(target);
        }
    }
}


