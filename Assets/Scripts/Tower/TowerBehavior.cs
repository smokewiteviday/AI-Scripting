using System;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    enum TowerState { Idle, Attack, Upgrade }
    TowerState state;
    bool stateComplete;
    public float fireRate = 1f; // Time between shots
    public float range = 5f; // Target detection range
    public float fireCooldown = 0f;
    public int enemiesLeftToUpgrade = 10;
    private int enemiesKilled;
    [SerializeField] private ObjectPool projectilePool;

    // Damage settings
    private float baseDamage = 20f; // Base damage of the projectile
    private float damageIncreasePerKill = 5f; // Damage increase per enemy killed

    private void Start()
    {
        // Find the object pool in the scene
        projectilePool = FindObjectOfType<ObjectPool>();
        if (projectilePool == null)
        {
            Debug.LogError("No ObjectPool found in the scene!");
        }
        enemiesKilled = 0; // Initialize enemies killed
    }

    private void Update()
    {
        if (stateComplete)
        {
            SelectState();
        }
        UpdateState();
    }

    protected GameObject FindClosestEnemy()
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
        // Fetch a projectile from the pool
        GameObject projectile = projectilePool.GetPooledObject();
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectile.SetActive(true);
            projectile.transform.position = transform.position;
            // Calculate the current damage based on enemies killed
            float currentDamage = baseDamage + (enemiesKilled * damageIncreasePerKill);
            projectileScript.SetTargetAndDamage(target, currentDamage); // Pass the damage to the projectile
            Debug.Log($"Tower at {transform.position} shoots with damage: {currentDamage}");
        }
        else
        {
            projectile.SetActive(false);
        }
    }

    // Called by the projectile when it kills an enemy
    public void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Tower at {transform.position} has killed {enemiesKilled} enemies.");
    }

    void SelectState()
    {
        stateComplete = false;
        if (FindClosestEnemy() != null)
        {
            state = TowerState.Attack;
            StartAttack();
        }
        if (enemiesLeftToUpgrade <= 0)
        {
            state = TowerState.Upgrade;
            StartUpgrade();
        }
        else
        {
            state = TowerState.Idle;
            StartIdle();
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case TowerState.Idle:
                UpdateIdle();
                break;
            case TowerState.Attack:
                UpdateAttack();
                break;
            case TowerState.Upgrade:
                UpdateUpgrade();
                break;
        }
    }

    private void StartIdle()
    {
    }

    private void StartAttack()
    {
        GameObject targetEnemy = FindClosestEnemy();
        fireCooldown -= Time.deltaTime; // Cooldown timer
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
    }

    private void StartUpgrade()
    {
    }

    private void UpdateIdle()
    {
        stateComplete = true;
    }

    private void UpdateAttack()
    {
        if (FindClosestEnemy() != null)
        {
            stateComplete = true;
        }
    }

    private void UpdateUpgrade()
    {
        stateComplete = true;
    }
}