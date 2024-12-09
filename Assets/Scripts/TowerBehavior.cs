using System;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    enum TowerState { Idle, Attack, Upgrade}
    TowerState state;
    bool stateComplete;
    public float fireRate = 1f; // Time between shots
    public float range = 5f; // Target detection range
    public float fireCooldown = 0f;
    public int enemiesLeftToUpgrade = 10;
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
       
        if (stateComplete) {
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
            projectileScript.SetTarget(target); // Assign the target to the projectile
          
        }
        else { projectile.SetActive(false); }
    }
    
    void SelectState()
    {
        stateComplete = false;
        if (FindClosestEnemy() != null)
        {
            state=TowerState.Attack;
            StartAttack();
        }
        else { 
            state=TowerState.Idle;
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
        
    }
    

}
