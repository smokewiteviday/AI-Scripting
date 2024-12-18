using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float baseMoveSpeed = 2f; // Base speed of the enemy
    public int baseHealth = 50; // Base health of the enemy
    private float currentMoveSpeed;
    public int currentHealth;

    private List<Cell> path; // Path the enemy will follow
    private int currentPathIndex; // Index of the current cell in the path
    private bool isMoving = false; // Whether the enemy is currently moving

    // Reference to the GridSystem (assign in the Inspector)
    public GridSystem gridSystem;

    // Boost multipliers (shared across all enemies)
    private static float healthMultiplier = 1f;
    private static float speedMultiplier = 1f;

    private void Start()
    {
        // Apply initial health and speed based on the multipliers
        currentMoveSpeed = baseMoveSpeed * speedMultiplier;
        currentHealth = Mathf.RoundToInt(baseHealth * healthMultiplier);
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
    }

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

    private void MoveAlongPath()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            OnReachEnd();
            return;
        }

        Vector2 targetPosition = gridSystem.GetWorldPosition(
            path[currentPathIndex].GridX,
            path[currentPathIndex].GridY
        );

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentMoveSpeed * Time.deltaTime);

        if ((Vector2)transform.position == targetPosition)
        {
            currentPathIndex++;
        }
    }

    private void OnReachEnd()
    {
        isMoving = false;
        Debug.Log("Enemy has reached the endpoint!");
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    // Static method to update global multipliers
    public static void IncreaseBoosts(float healthIncrease, float speedIncrease)
    {
        healthMultiplier += healthIncrease;
        speedMultiplier += speedIncrease;
        Debug.Log($"Boosts applied: HealthMultiplier={healthMultiplier}, SpeedMultiplier={speedMultiplier}");
    }
}
