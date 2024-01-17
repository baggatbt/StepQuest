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

    public Character SpawnEnemiesFromPool(string poolName, int numberToSpawn, Transform spawnPoint, Slider associatedHealthBarSlider, Slider associatedEnergyBarSlider, int healthTextIndex, int level)
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
        float yOffset = 0.4f; // Offset for enemy spawn position above the spawn point
        GameObject spawnedEnemy = Instantiate(enemyToSpawn, new Vector3(spawnPoint.position.x, spawnPoint.position.y + yOffset, spawnPoint.position.z), spawnPoint.rotation);
            
        lastSpawnedCharacter = spawnedEnemy.GetComponent<Character>();
        if (lastSpawnedCharacter == null) continue;

        lastSpawnedCharacter.level = level;
        Debug.Log("Last spawned enemy  " + lastSpawnedCharacter.level);
        // Set health and energy bars
        lastSpawnedCharacter.healthBar = associatedHealthBarSlider;
        lastSpawnedCharacter.energyBar = associatedEnergyBarSlider;

        // Calculate the position 3 units below the spawned character
        Renderer enemyRenderer = spawnedEnemy.GetComponent<Renderer>();
        float characterBottom = 0f;
        if (enemyRenderer != null)
        {
            characterBottom = enemyRenderer.bounds.min.y; // Get the lowest point of the character
        }
        Vector3 sliderPositionOffset = new Vector3(spawnedEnemy.transform.position.x, characterBottom - 0.5f, spawnedEnemy.transform.position.z);

        // Move health and energy bar sliders to the calculated position
        associatedHealthBarSlider.transform.position = sliderPositionOffset;
        associatedEnergyBarSlider.transform.position = sliderPositionOffset;

        // Update health text and sliders
        lastSpawnedCharacter.healthText = healthTexts[healthTextIndex];
        enemiesSpawned++;  // Increment the spawn count

        lastSpawnedCharacter.healthText.text = lastSpawnedCharacter.health + " / " + lastSpawnedCharacter.maxHealth;
        Debug.Log(lastSpawnedCharacter + " " + lastSpawnedCharacter.healthText.text);
        associatedHealthBarSlider.gameObject.SetActive(true);
        associatedHealthBarSlider.maxValue = lastSpawnedCharacter.maxHealth;
        associatedHealthBarSlider.value = lastSpawnedCharacter.health;

        associatedEnergyBarSlider.gameObject.SetActive(true);
        associatedEnergyBarSlider.maxValue = lastSpawnedCharacter.maxEnergy;
        associatedEnergyBarSlider.value = 0;

        lastSpawnedCharacter.UpdateStats();
    }
    enemiesSpawned = 0;

    return lastSpawnedCharacter;
}


}
