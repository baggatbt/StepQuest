using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySpawnController : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI healthText2;
    public TextMeshProUGUI healthText3;

    private TextMeshProUGUI[] healthTexts; // Array to store all health texts
    private int enemiesSpawned = 0;        // Counter to keep track of the number of enemies spawned

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
        
        foreach (var pool in enemyPools)
        {
            poolDictionary.Add(pool.poolName, pool.enemies);
        }

        // Initialize the health texts array
        healthTexts = new TextMeshProUGUI[] { healthText, healthText2, healthText3 };
    }

    public Character SpawnEnemiesFromPool(string poolName, int numberToSpawn, Transform spawnPoint, Slider associatedHealthBarSlider)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning("Pool " + poolName + " doesn't exist!");
            return null;
        }

        Character lastSpawnedCharacter = null;
        for (int i = 0; i < numberToSpawn; i++)
        {
            GameObject enemyToSpawn = poolDictionary[poolName][Random.Range(0, poolDictionary[poolName].Count)];
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
            
            lastSpawnedCharacter = spawnedEnemy.GetComponent<Character>();
            if (lastSpawnedCharacter == null) continue;

            lastSpawnedCharacter.healthBar = associatedHealthBarSlider;

            // Assign the healthText based on the enemiesSpawned count
            lastSpawnedCharacter.healthText = healthTexts[enemiesSpawned];
            enemiesSpawned++;  // Increment the spawn count

            lastSpawnedCharacter.healthText.text = "HP: " + lastSpawnedCharacter.health;

           // associatedHealthBarSlider.gameObject.SetActive(true);
            associatedHealthBarSlider.maxValue = lastSpawnedCharacter.maxHealth;
            associatedHealthBarSlider.value = lastSpawnedCharacter.health;
        }

        return lastSpawnedCharacter;
    }
}
