using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f; // Movement speed toward the target
    public float detectionRange = 10f; // Range within which the enemy detects buildings

    private Transform target; // Current target (nearest building)

    void Update()
    {
        UpdateTarget();

        if (target != null)
        {
            MoveTowardsTarget();
        }
    }

    void UpdateTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestBuilding = null;

        foreach (GameObject building in buildings)
        {
            float distanceToBuilding = Vector2.Distance(transform.position, building.transform.position);
            if (distanceToBuilding < shortestDistance && distanceToBuilding <= detectionRange)
            {
                shortestDistance = distanceToBuilding;
                nearestBuilding = building;
            }
        }

        // Set the closest building within range as the target
        if (nearestBuilding != null && shortestDistance <= detectionRange)
        {
            target = nearestBuilding.transform;
        }
        else
        {
            target = null; // No building within range
        }
    }

    void MoveTowardsTarget()
    {
        // Smoothly move towards the target
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
}
