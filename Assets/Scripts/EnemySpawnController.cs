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

    public Character SpawnEnemiesFromPool(string poolName, int numberToSpawn, Transform spawnPoint, Slider associatedHealthBarSlider, Slider associatedEnergyBarSlider, int healthTextIndex)
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
            float yOffset = 0.4f; // This is the offset value by which you want to spawn the object above the Y value of the spawnPoint
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, new Vector3(spawnPoint.position.x, spawnPoint.position.y + yOffset, spawnPoint.position.z), spawnPoint.rotation);
            Debug.Log(spawnedEnemy);
            
            lastSpawnedCharacter = spawnedEnemy.GetComponent<Character>();
            if (lastSpawnedCharacter == null) continue;

            lastSpawnedCharacter.healthBar = associatedHealthBarSlider;
            lastSpawnedCharacter.energyBar = associatedEnergyBarSlider;

           
             lastSpawnedCharacter.healthText = healthTexts[healthTextIndex];
            enemiesSpawned++;  // Increment the spawn count

            lastSpawnedCharacter.healthText.text =  lastSpawnedCharacter.health + " / " + lastSpawnedCharacter.maxHealth;
            Debug.Log(lastSpawnedCharacter + " " + lastSpawnedCharacter.healthText.text);
            associatedHealthBarSlider.gameObject.SetActive(true);
            associatedHealthBarSlider.maxValue = lastSpawnedCharacter.maxHealth;
            associatedHealthBarSlider.value = lastSpawnedCharacter.health;

            associatedEnergyBarSlider.gameObject.SetActive(true);
            associatedEnergyBarSlider.maxValue = lastSpawnedCharacter.maxEnergy;
            associatedEnergyBarSlider.value = 0;
        }
        enemiesSpawned = 0;

        return lastSpawnedCharacter;
    }
}
