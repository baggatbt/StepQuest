using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject knightGhostPrefab; 
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();
    void Awake()
    {
        // Register the ObjectPool with all GhostEffect instances
        GhostEffect[] ghostEffects = FindObjectsOfType<GhostEffect>();
        foreach (var effect in ghostEffects)
        {
            effect.SetObjectPool(this);
        }
    }
    void Start()
    {
        // Pre-instantiate pool objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(knightGhostPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Optional: Create new objects if the pool is empty
            GameObject obj = Instantiate(knightGhostPrefab, transform);
            return obj;
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
