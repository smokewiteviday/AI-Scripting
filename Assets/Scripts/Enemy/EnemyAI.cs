using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public int currentHealth;
    public int maxHealth=50;
    private GameObject targetBuilding;
    public Pathfinding pathfinding;

    private void Start()
    {
        
        currentHealth =maxHealth;
    }

    private void Update()
    {
       
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

    public void TakeDamage(int damage)
    {
       
        currentHealth -= damage;
        Debug.Log(currentHealth);
        
    }
    public void Die()
    {   

        Destroy(gameObject); 
    }
    
}
