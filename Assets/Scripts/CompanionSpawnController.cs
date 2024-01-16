using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class CompanionSpawnController : MonoBehaviour
{
    public GameObject companionPrefab;
    public Transform companionSpawnPoint; // You can assign this directly in the inspector
    public Slider associatedHealthBarSlider; // Reference to the slider
    public Slider associatedEnergyBarSlider;
    public TextMeshProUGUI associatedHealthText;
    public TextMeshProUGUI associatedEnergyText;
    public Slider associatedtempEnergyBarSlider;
    public BattleManager battleManager;


   private void Start()
   {
    if (battleManager == null)
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    Character spawnedCompanion = SpawnCompanionAtPoint(companionSpawnPoint);
    Debug.Log(spawnedCompanion.attackPower);
    if (spawnedCompanion != null && battleManager != null)
    {
        battleManager.companion2 = spawnedCompanion;
        battleManager.playerParty.Add(spawnedCompanion);
    }
   }

    public Character SpawnCompanionAtPoint(Transform spawnPoint)
    {
        // Instantiate the Companion at the position of spawnPoint and with its rotation
        GameObject spawnedCompanionObject = Instantiate(companionPrefab, spawnPoint.position, spawnPoint.rotation);
        Character spawnedCompanion = spawnedCompanionObject.GetComponent<Character>();

        // Set the tag for the spawned companion object
        spawnedCompanionObject.tag = "Companion";

        // Calculate the position 3 units below the spawned companion
        Renderer companionRenderer = spawnedCompanionObject.GetComponent<Renderer>();
        float characterBottom = 0f;
        if (companionRenderer != null)
        {
            characterBottom = companionRenderer.bounds.min.y; // Get the lowest point of the companion
        }
        Vector3 sliderPositionOffset = new Vector3(spawnedCompanionObject.transform.position.x, characterBottom - 0.5f, spawnedCompanionObject.transform.position.z);

        // Move health and energy bar sliders to the calculated position
        associatedHealthBarSlider.transform.position = sliderPositionOffset;
        associatedEnergyBarSlider.transform.position = sliderPositionOffset;

        // Link this character to the health bar
        spawnedCompanion.healthBar = associatedHealthBarSlider;
        associatedHealthBarSlider.gameObject.SetActive(true); // Activate the health bar
        associatedHealthBarSlider.maxValue = spawnedCompanion.maxHealth;
        associatedHealthBarSlider.value = spawnedCompanion.health;
        spawnedCompanion.healthText = associatedHealthText;

        spawnedCompanion.energyBar = associatedEnergyBarSlider;
        spawnedCompanion.energyText = associatedEnergyText;

        spawnedCompanion.isFront = false;

        return spawnedCompanion; // Ensure a Character is always returned
    }

}
