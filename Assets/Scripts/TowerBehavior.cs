using UnityEngine;

public class TowerAI : MonoBehaviour
{
    public float fireRate = 1f; // Time between shots
    public float range = 5f; // Target detection range
    public Transform firePoint; // Where the projectile is fired from

    public float fireCooldown = 0f;
    [SerializeField] private ObjectPool projectilePool;

    private void Start()
    {
        // Find the object pool in the scene
        projectilePool = FindObjectOfType<ObjectPool>();
        if (projectilePool == null)
        {
            Debug.LogError("No ObjectPool found in the scene!");
        }
    }

    private void Update()
    {
        //GameObject targetEnemy = FindClosestEnemy();

        //if (targetEnemy != null)
        //{
        //    // Rotate to face the enemy (optional for 2D towers)
        //    //Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
        //    //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //    //transform.rotation = Quaternion.Euler(0, 0, angle);

        //    // Shoot if cooldown is over
        //    if (fireCooldown <= 0f)
        //    {
        //        Shoot(targetEnemy);
        //        fireCooldown = 1f / fireRate;
        //    }
        //}

        fireCooldown -= Time.deltaTime; // Cooldown timer

        if (fireCooldown <= 0f)
        {
            Debug.Log("Shoot");
            Shoot(FindClosestEnemy());
            fireCooldown = 1f / fireRate;
        }
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
        //if (projectilePool == null) return;

        // Fetch a projectile from the pool
        GameObject projectile = projectilePool.GetPooledObject();
        //projectile.transform.position = firePoint.position;
        //projectile.transform.rotation = firePoint.rotation;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectile.SetActive(true);
            projectile.transform.position = transform.position;
            //projectileScript.Initialize(projectilePool); // Assign the pool to the projectile
            projectileScript.SetTarget(target); // Assign the target to the projectile
          
        }
    }
}
