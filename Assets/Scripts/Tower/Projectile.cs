using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    private Transform target;
    

    public void SetTarget(GameObject targetObject)
    {
        target = targetObject.transform;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        // Move towards the target
        Vector2 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
          
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            gameObject.SetActive(false);
            EnemyAI enemy = target.GetComponent<EnemyAI>();
            TowerAI towerAI = GetComponent<TowerAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);

            }
            if (enemy.currentHealth <= 0)
            {
                enemy.Die();
                
               
            }
           
        }
    }
    
}
