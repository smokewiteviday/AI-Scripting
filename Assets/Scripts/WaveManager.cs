using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    //public GameObject enemyPrefab;
    //public int waveNumber = 1;
    [SerializeField] private ObjectPool enemyPool;

    //public void StartWave()
    //{
    //    StartCoroutine(SpawnWave());
    //}

    //private IEnumerator SpawnWave()
    //{
    //    int enemyCount = 1; // Example scaling formula
    //    for (int i = 0; i < enemyCount; i++)
    //    {
    //        SpawnEnemy();
    //        yield return new WaitForSeconds(1f);
    //    }
    //    waveNumber++;
    //}
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q");
            SpawnEnemy();
        }
    }
    public void SpawnEnemy()
    {
        GameObject enemy = enemyPool.GetPooledObject();
        
        if (enemy != null)
        {
            enemy.SetActive(true);
            enemy.transform.position = transform.position;
            
        }
        
       
    }
}
