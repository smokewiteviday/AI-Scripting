using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int maxHealth = 10; 
    private int currentHealth; 

    private float timer = 0f;

    public GameObject UI;
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
            EnemyAI.IncreaseBoosts(0.2f, 0.1f); 
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

   
    private void GameOver()
    {
        Debug.Log("Game Over! Player health has dropped to 0.");
        UI.SetActive(true);
        Time.timeScale = 0f;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
