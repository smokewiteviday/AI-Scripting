using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public int amountToPool = 20; // Initial number of objects in the pool
    private List<GameObject> pooledObjects = new List<GameObject>();
    [SerializeField] private GameObject projectilePrefab; // The prefab to pool
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    public GameObject GetPooledObject()
    {
        for(int i = 0;i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }

        }
        return null;
    }
    //private void Start()
    //{
    //    // Pre-instantiate the pool objects
    //    for (int i = 0; i < initialPoolSize; i++)
    //    {
    //        GameObject obj = Instantiate(prefab);
    //        obj.SetActive(false);
    //        pool.Enqueue(obj);
    //    }
    //}

    //public GameObject GetObject()
    //{
    //    if (pool.Count > 0)
    //    {
    //        GameObject obj = pool.Dequeue();
    //        obj.SetActive(true);
    //        return obj;
    //    }
    //    else
    //    {
    //        // If the pool is empty, instantiate a new object
    //        GameObject obj = Instantiate(prefab);
    //        return obj;
    //    }
    //}

    //public void ReturnObject(GameObject obj)
    //{
    //    obj.SetActive(false);
    //    pool.Enqueue(obj);
    //}
}
