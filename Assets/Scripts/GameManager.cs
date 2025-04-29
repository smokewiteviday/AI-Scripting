using UnityEngine;
using UnityEngine.UI; // For using UI Text (or TMPro for TextMeshPro)
using TMPro; // If using TextMeshPro

public class GameManager : MonoBehaviour
{
    public int enemiesReachedEndpoint = 0;
    public int maxEnemiesAllowed = 10;

    // Timer variables
    private float gameTime = 0f; // Time in seconds
    public Text timerText; // Reference to UI Text (for regular Text)
    // public TextMeshProUGUI timerText; // Uncomment if using TextMeshPro

    private void Start()
    {
        // Ensure timerText is assigned in the Inspector
        if (timerText == null)
        {
            Debug.LogError("Timer Text is not assigned in the GameManager!");
        }
    }

    private void Update()
    {
        // Update the game timer
        gameTime += Time.deltaTime;
        UpdateTimerDisplay();
    }

    public float GetGameTime()
    {
        return gameTime;
    }

    public void EnemyReachedEndpoint()
    {
        enemiesReachedEndpoint++;
        Debug.Log($"Enemy reached endpoint. Total: {enemiesReachedEndpoint}");

        if (enemiesReachedEndpoint >= maxEnemiesAllowed)
        {
            GameOver();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Convert gameTime to minutes and seconds
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Too many enemies reached the endpoint.");
        // Add additional game over logic here (e.g., stop spawning enemies, show game over screen)
        Time.timeScale = 0f; // Pause the game
    }
}