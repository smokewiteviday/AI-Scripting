using System.Collections;
using System.Collections.Generic;
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
            Destroy(gameObject); // Destroy projectile if target no longer exists
            return;
        }

        // Move towards the target
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        EnemyAI enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            //enemy.TakeDamage(damage);
        }

        Destroy(gameObject); // Destroy the projectile
    }
}
