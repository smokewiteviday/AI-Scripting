using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        // Trigger a boost every 5 seconds
        if (timer >= 5f)
        {
            // Increase global multipliers
            EnemyAI.IncreaseBoosts(0.5f, 0.3f); // Increase health by 20% and speed by 10%

            timer = 0f; // Reset timer
        }
    }
}
