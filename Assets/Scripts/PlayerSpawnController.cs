using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerSpawnController : MonoBehaviour
{
    public GameObject knightPrefab;
    public Transform playerSpawnPoint; // You can assign this directly in the inspector
    public Slider associatedHealthBarSlider; // Reference to the slider
    public Slider associatedEnergyBarSlider;
    public Slider associatedtempEnergyBarSlider;
    public TextMeshProUGUI associatedHealthText;
    public TextMeshProUGUI associatedEnergyText;
    public BattleManager battleManager;


   

    public Character SpawnPlayerAtPoint(Transform spawnPoint)
    {
        // Instantiate the player at the position of spawnPoint and with its rotation
        GameObject spawnedPlayerObject = Instantiate(knightPrefab, spawnPoint.position, spawnPoint.rotation);
        Character spawnedPlayer = spawnedPlayerObject.GetComponent<Character>();

        // Load the character's data 
        if (spawnedPlayer is Knight)
        {
            (spawnedPlayer as Knight).LoadCharacterData();
        }

        // Calculate the position 3 units below the spawned player
        Renderer playerRenderer = spawnedPlayerObject.GetComponent<Renderer>();
        float characterBottom = 0f;
        if (playerRenderer != null)
        {
            characterBottom = playerRenderer.bounds.min.y; // Get the lowest point of the player
        }
        Vector3 sliderPositionOffset = new Vector3(spawnedPlayerObject.transform.position.x, characterBottom - 0.5f, spawnedPlayerObject.transform.position.z);

        // Move health and energy bar sliders to the calculated position
        associatedHealthBarSlider.transform.position = sliderPositionOffset;
        associatedEnergyBarSlider.transform.position = sliderPositionOffset;

        // Link this character to the health and energy bars
        spawnedPlayer.healthBar = associatedHealthBarSlider;
        associatedHealthBarSlider.gameObject.SetActive(true); // Activate the health bar
        associatedHealthBarSlider.maxValue = spawnedPlayer.maxHealth;
        associatedHealthBarSlider.value = spawnedPlayer.health;
        spawnedPlayer.healthText = associatedHealthText;
        associatedHealthText.text = spawnedPlayer.health + " / " + spawnedPlayer.maxHealth;

        spawnedPlayer.energyBar = associatedEnergyBarSlider;
        associatedEnergyBarSlider.gameObject.SetActive(true); // Activate the energy bar
        associatedEnergyBarSlider.maxValue = spawnedPlayer.maxEnergy;
        associatedEnergyBarSlider.value = spawnedPlayer.energy;
        spawnedPlayer.energyText = associatedEnergyText;
return spawnedPlayer; // Ensure a player character is always returned
}}
