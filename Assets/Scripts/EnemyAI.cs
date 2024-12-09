using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public int health;
    private GameObject targetBuilding;
    

    private void Start()
    {
        UpdateTarget();
        health = 50;
    }

    private void Update()
    {
        if (targetBuilding == null)
        {
            UpdateTarget(); // Find a new target if the current one is destroyed
            return;
        }

        // Move towards the target building
        Vector2 direction = (targetBuilding.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        // Check if reached the target
        if (Vector2.Distance(transform.position, targetBuilding.transform.position) < 0.5f)
        {
            //AttackBuilding();
        }
    }

    private void UpdateTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject building in buildings)
        {
            float distance = Vector2.Distance(transform.position, building.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                targetBuilding = building;
            }
        }
    }

    //private void AttackBuilding()
    //{
    //    Building building = targetBuilding.GetComponent<Building>();
    //    if (building != null)
    //    {
    //        building.TakeDamage(10); // Example damage value
    //    }
    //    
    //}

    public void TakeDamage(int damage)
    {
        TowerAI towerAI= GetComponent<TowerAI>();
        health -= damage;
        Debug.Log(health);
        if (health <= 0)
        {
            Destroy(gameObject);
            towerAI.enemiesLeftToUpgrade -= 1;
            Debug.Log(towerAI.enemiesLeftToUpgrade);
        }
        
    }

    
}
