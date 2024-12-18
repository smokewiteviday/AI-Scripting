using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int maxHealth = 10; // Maximum health for the player
    private int currentHealth; // Current health of the player

    private float timer = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Player Health: {currentHealth}");
    }

    private void Update()
    {
        // Periodic enemy boost logic
        timer += Time.deltaTime;

        if (timer >= 5f)
        {
            EnemyAI.IncreaseBoosts(0.2f, 0.1f); // Example boost for health and speed
            timer = 0f;
        }
    }

    // Called when an enemy reaches the endpoint
    public void EnemyReachedEndpoint()
    {
        int damage = 1; // Damage dealt to the player when an enemy reaches the endpoint
        currentHealth -= damage;

        Debug.Log($"Player took {damage} damage! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    // Handles game over logic
    private void GameOver()
    {
        Debug.Log("Game Over! Player health has dropped to 0.");
        // Implement further game over logic (e.g., restart, show game over screen)
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
