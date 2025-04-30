using System;
using UnityEngine;
using System.Collections;

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
    private float baseDamage = 20f;
    private float damageIncreasePerKill = 2f; 
    private bool isUpgraded = false; 
    private float secondShotDelay = 0.2f; 
    private SpriteRenderer spriteRenderer; 

    private void Start()
    {
        // Find the object pool in the scene
        projectilePool = FindObjectOfType<ObjectPool>();
        if (projectilePool == null)
        {
            Debug.LogError("No ObjectPool found in the scene!");
        }
        enemiesKilled = 0; // Initialize enemies killed

        // Get the SpriteRenderer component to change color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the tower!");
        }
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
        // Calculate the current damage based on enemies killed
        float currentDamage = baseDamage + (enemiesKilled * damageIncreasePerKill);

        // Check if damage has reached 50 and upgrade if not already upgraded
        if (currentDamage >= 50 && !isUpgraded)
        {
            isUpgraded = true;
            ChangeToUpgradeState();
        }

        // If upgraded, shoot two projectiles with a delay; otherwise, shoot one
        if (isUpgraded)
        {
            StartCoroutine(ShootTwoProjectiles(target, currentDamage));
        }
        else
        {
            ShootSingleProjectile(target, currentDamage);
        }
    }

    private void ShootSingleProjectile(GameObject target, float damage)
    {
        if (projectilePool == null)
        {
            Debug.LogError($"Tower at {transform.position} failed to shoot: ObjectPool is null!");
            return;
        }

        GameObject projectile = projectilePool.GetPooledObject();
        if (projectile == null)
        {
            Debug.LogWarning($"Tower at {transform.position} failed to shoot: No available projectiles in the pool!");
            return;
        }

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectile.SetActive(true);
            projectile.transform.position = transform.position;
            projectileScript.SetTargetAndDamage(target, damage);
            Debug.Log($"Tower at {transform.position} shoots with damage: {damage}");
        }
        else
        {
            Debug.LogError($"Tower at {transform.position} failed to shoot: Projectile script not found on object!");
            projectile.SetActive(false);
        }
    }

    private IEnumerator ShootTwoProjectiles(GameObject target, float damage)
    {
        // First shot
        ShootSingleProjectile(target, damage);
        yield return new WaitForSeconds(secondShotDelay);

        // Check if the target is still valid before shooting the second projectile
        if (target != null && target.activeInHierarchy)
        {
            ShootSingleProjectile(target, damage);
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

    private void ChangeToUpgradeState()
    {
        state = TowerState.Upgrade;
        StartUpgrade();
    }

    private void StartUpgrade()
    {
        // Change the tower's color to blue when upgraded
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue;
        }
        Debug.Log($"Tower at {transform.position} has upgraded to shoot two projectiles and turned blue!");
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
        // After upgrading, continue attacking with the new behavior
        StartAttack();
    }
}