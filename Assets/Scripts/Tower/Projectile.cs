using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private float damage; // Damage is now dynamic
    private Transform target;
    private TowerAI tower; // Reference to the tower that shot this projectile

    public void SetTargetAndDamage(GameObject targetObject, float damageValue)
    {
        target = targetObject.transform;
        damage = damageValue;
        // Find the tower that shot this projectile
        tower = targetObject.GetComponent<EnemyAI>().GetComponentInParent<TowerAI>();
        if (tower == null)
        {
            tower = FindObjectOfType<TowerAI>(); // Fallback to find any TowerAI (less reliable)
        }
    }

    private void Update()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
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

            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);

                if (enemy.currentHealth <= 0)
                {
                    enemy.Die();
                    // Notify the tower that an enemy was killed
                    if (tower != null)
                    {
                        tower.OnEnemyKilled();
                    }
                }
            }
        }
    }
}