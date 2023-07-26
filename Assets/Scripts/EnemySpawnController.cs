using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Character SpawnEnemiesFromPool(string poolName, int numberToSpawn, Transform spawnPoint, Slider associatedHealthBarSlider)
{
        // Check if the pool exists
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning("Pool " + poolName + " doesn't exist!");
            return null;
        }

        // Spawn the enemies and return the last spawned one (assuming only one will be spawned most of the time)
        Character lastSpawnedCharacter = null;
        for (int i = 0; i < numberToSpawn; i++)
        {
            GameObject enemyToSpawn = poolDictionary[poolName][Random.Range(0, poolDictionary[poolName].Count)];
            
            // Instantiate the enemy at the position of spawnPoint and with its rotation
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
            lastSpawnedCharacter = spawnedEnemy.GetComponent<Character>();

         // Link this character to the health bar
        lastSpawnedCharacter.healthBar = associatedHealthBarSlider;
        associatedHealthBarSlider.gameObject.SetActive(true);  // Activate the health bar
        associatedHealthBarSlider.maxValue = lastSpawnedCharacter.maxHealth;  
        associatedHealthBarSlider.value = lastSpawnedCharacter.health; 

        }

        return lastSpawnedCharacter;
    }
}
