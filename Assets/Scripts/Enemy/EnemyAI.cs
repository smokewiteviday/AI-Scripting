using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int currentHealth;
    public int maxHealth=50;

    public float speed = 2f; // Speed of the enemy's movement
    private List<Cell> path; // The path the enemy will follow
    private int currentPathIndex; // Index of the current cell in the path
    private bool isMoving = false; // Whether the enemy is currently moving

    // Reference to the GridSystem (assign in the Inspector)
    public GridSystem gridSystem;

    // Initialize the enemy with the path to follow
    public void Initialize(List<Cell> pathToFollow, GridSystem grid)
    {
        if (pathToFollow == null || pathToFollow.Count == 0)
        {
            Debug.LogError("No valid path provided to the enemy!");
            return;
        }

        path = pathToFollow;
        gridSystem = grid;
        currentPathIndex = 0;
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    // Moves the enemy along the calculated path
    private void MoveAlongPath()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            // Reached the endpoint
            OnReachEnd();
            return;
        }

        // Get the target position from the current path cell
        Vector2 targetPosition = gridSystem.GetWorldPosition(
            path[currentPathIndex].GridX,
            path[currentPathIndex].GridY
        );

        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the enemy has reached the target position
        if ((Vector2)transform.position == targetPosition)
        {
            currentPathIndex++;
        }
    }

    // Called when the enemy reaches the end of the path
    private void OnReachEnd()
    {
        isMoving = false;
        Debug.Log("Enemy has reached the endpoint!");

        // Perform any desired actions (e.g., damage the player's base, destroy the enemy, etc.)
        Destroy(gameObject);
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
