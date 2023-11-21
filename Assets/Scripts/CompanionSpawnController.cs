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
        battleManager.companion = spawnedCompanion;
    }
   }

    public Character SpawnCompanionAtPoint(Transform spawnPoint)
{
    // Instantiate the Companion at the position of spawnPoint and with its rotation
    GameObject spawnedCompanionObject = Instantiate(companionPrefab, spawnPoint.position, spawnPoint.rotation);
    Character spawnedCompanion = spawnedCompanionObject.GetComponent<Character>();
    
    // Set the tag for the spawned companion object
    spawnedCompanionObject.tag = "Companion";


     
    // Link this character to the health bar
    spawnedCompanion.healthBar = associatedHealthBarSlider;
    associatedHealthBarSlider.gameObject.SetActive(true); // Activate the health bar
    associatedHealthBarSlider.maxValue = spawnedCompanion.maxHealth;
    associatedHealthBarSlider.value = spawnedCompanion.health;
    spawnedCompanion.healthText = associatedHealthText;

    spawnedCompanion.energyBar = associatedEnergyBarSlider;
    spawnedCompanion.energyText = associatedEnergyText;



  

   

    return spawnedCompanion; // Ensure a Character is always returned
}

}
