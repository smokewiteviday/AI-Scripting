using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    public int enemiesReachedEndpoint = 0;
    public int maxEnemiesAllowed = 10;

    // Timer variables
    private float gameTime = 0f; 
    public Text timerText;

    public GameObject UI;

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
        UI.SetActive(true);
        Time.timeScale = 0f; 
    }
}