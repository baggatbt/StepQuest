using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySpawnController : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI healthText2;
    public TextMeshProUGUI healthText3;
    public GameObject[] healthUI;
    private Camera mainCamera;

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

        // Initialize the main camera reference
        mainCamera = Camera.main;
    }

    public Character SpawnEnemiesFromPool(string poolName, Transform spawnPoint, Slider associatedHealthBarSlider, Slider associatedEnergyBarSlider, int healthTextIndex, int level, GameObject enemyHealthUI)
{
    if (!poolDictionary.ContainsKey(poolName))
    {
        Debug.LogWarning($"Pool '{poolName}' not found!");
        return null;
    }

    // Spawn one enemy only
    GameObject enemyToSpawn = poolDictionary[poolName][Random.Range(0, poolDictionary[poolName].Count)];
    float yOffset = 0.4f; // Offset for enemy spawn position above the spawn point
    GameObject spawnedEnemy = Instantiate(enemyToSpawn, new Vector3(spawnPoint.position.x, spawnPoint.position.y + yOffset, spawnPoint.position.z), spawnPoint.rotation);

    // Get the Character component
    Character lastSpawnedCharacter = spawnedEnemy.GetComponent<Character>();
    if (lastSpawnedCharacter == null)
    {
        Debug.LogWarning("Spawned enemy does not have a Character component!");
        return null;
    }

    lastSpawnedCharacter.level = level;

    // Ensure each enemy has its own health bar UI and health text
    lastSpawnedCharacter.enemyHealthUI = enemyHealthUI; // Assign unique health UI for this enemy
    lastSpawnedCharacter.healthText = healthTexts[healthTextIndex]; // Assign unique health text

    // Ensure the health UI is active
    if (lastSpawnedCharacter.enemyHealthUI != null && !lastSpawnedCharacter.enemyHealthUI.activeInHierarchy)
    {
        // Activate the parent and the health UI
        Transform healthUIParent = lastSpawnedCharacter.enemyHealthUI.transform.parent;
        if (healthUIParent != null && !healthUIParent.gameObject.activeSelf)
        {
            healthUIParent.gameObject.SetActive(true);  // Activate parent if inactive
        }

        lastSpawnedCharacter.enemyHealthUI.SetActive(true);  // Activate the health UI
        Debug.Log($"Health UI {lastSpawnedCharacter.enemyHealthUI.name} is now active for {spawnedEnemy.name}");
    }

    // Set health and energy bars
    if (!associatedHealthBarSlider.gameObject.activeSelf)
    {
        associatedHealthBarSlider.gameObject.SetActive(true);
    }

    if (!associatedEnergyBarSlider.gameObject.activeSelf)
    {
       // associatedEnergyBarSlider.gameObject.SetActive(true);
    }

    lastSpawnedCharacter.healthBar = associatedHealthBarSlider;
    lastSpawnedCharacter.energyBar = associatedEnergyBarSlider;

    // Set the values for health and energy
    lastSpawnedCharacter.healthBar.maxValue = lastSpawnedCharacter.maxHealth;
    lastSpawnedCharacter.healthBar.value = lastSpawnedCharacter.health;

    lastSpawnedCharacter.energyBar.maxValue = lastSpawnedCharacter.maxEnergy;
    lastSpawnedCharacter.energyBar.value = 0;

    // Calculate the position above the spawned character
    Renderer enemyRenderer = spawnedEnemy.GetComponent<Renderer>();
    float characterTop = enemyRenderer != null ? enemyRenderer.bounds.max.y : 0f;
    Vector3 worldPosition = new Vector3(spawnedEnemy.transform.position.x, characterTop + 0.5f, spawnedEnemy.transform.position.z);
    Vector2 screenPosition;

    // Convert world position to screen space with RectTransformUtility
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        (RectTransform)associatedHealthBarSlider.transform.parent,
        mainCamera.WorldToScreenPoint(worldPosition),
        mainCamera,
        out screenPosition
    );

    associatedHealthBarSlider.transform.localPosition = screenPosition;

    // Update stats and UI after spawning
    lastSpawnedCharacter.UpdateStats();
    lastSpawnedCharacter.healthText.text = lastSpawnedCharacter.health.ToString();

    return lastSpawnedCharacter;
}










}
