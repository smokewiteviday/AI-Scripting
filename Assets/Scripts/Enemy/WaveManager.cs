using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public GameObject[] enemyPrefab;

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    //[SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning=false;
   

    private void Start()
    {
        StartWave();  
    }
    private void StartWave()
    {
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
    }
    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies*Mathf.Pow(currentWave, difficultyScalingFactor));
    }
    private void Update()
    {
        if (!isSpawning) return;

        timeSinceLastSpawn += Time.deltaTime;

        if(timeSinceLastSpawn >= (1f / enemiesPerSecond)&& enemiesLeftToSpawn>0)
        {
           
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }
    }
    private void SpawnEnemy()
    {
        Debug.Log("Spawn Enemy");
        GameObject prefabToSpawn = enemyPrefab[0];
        Instantiate(prefabToSpawn, this.transform.position, Quaternion.identity);
    }
}
