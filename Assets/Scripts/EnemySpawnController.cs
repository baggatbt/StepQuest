using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPool
    {
        public string poolName;
        public List<GameObject> enemies;
    }

    public List<EnemyPool> enemyPools;
    private Dictionary<string, List<GameObject>> poolDictionary;
    
    private void Awake()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();
        
        foreach(var pool in enemyPools)
        {
            poolDictionary.Add(pool.poolName, pool.enemies);
        }
    }

    public void SpawnEnemiesFromPool(string poolName, int count, Transform parentTransform)
{
    if (!poolDictionary.ContainsKey(poolName))
    {
        Debug.LogWarning("Pool " + poolName + " doesn't exist!");
        return;
    }

    for (int i = 0; i < count; i++)
    {
        GameObject enemyToSpawn = poolDictionary[poolName][Random.Range(0, poolDictionary[poolName].Count)];
        
        // Instantiate the enemy at the position of parentTransform and with its rotation
        GameObject spawnedEnemy = Instantiate(enemyToSpawn, parentTransform.position, parentTransform.rotation);
        
        
    }
}

}
