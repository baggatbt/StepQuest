using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerSpawnController : MonoBehaviour
{
    public GameObject knightPrefab;
    public Transform playerSpawnPoint; // You can assign this directly in the inspector
    public Slider associatedHealthBarSlider; // Reference to the slider
    public Slider associatedEnergyBarSlider;
    public TextMeshProUGUI associatedHealthText;
    public TextMeshProUGUI associatedEnergyText;
    public BattleManager battleManager;


   private void Start()
   {
    if (battleManager == null)
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    Character spawnedPlayer = SpawnPlayerAtPoint(playerSpawnPoint);
    Debug.Log(spawnedPlayer.attackPower);
    if (spawnedPlayer != null && battleManager != null)
    {
        battleManager.player = spawnedPlayer;
    }
   }

    public Character SpawnPlayerAtPoint(Transform spawnPoint)
{
    // Instantiate the player at the position of spawnPoint and with its rotation
    GameObject spawnedPlayerObject = Instantiate(knightPrefab, spawnPoint.position, spawnPoint.rotation);
    Character spawnedPlayer = spawnedPlayerObject.GetComponent<Character>();

     
    // Link this character to the health bar
    spawnedPlayer.healthBar = associatedHealthBarSlider;
    associatedHealthBarSlider.gameObject.SetActive(true); // Activate the health bar
    associatedHealthBarSlider.maxValue = spawnedPlayer.maxHealth;
    associatedHealthBarSlider.value = spawnedPlayer.health;
    spawnedPlayer.healthText = associatedHealthText;

    spawnedPlayer.energyBar = associatedEnergyBarSlider;
    associatedEnergyBarSlider.gameObject.SetActive(true); // Activate the energy bar
    associatedEnergyBarSlider.maxValue = spawnedPlayer.maxEnergy;
    associatedEnergyBarSlider.value = spawnedPlayer.energy;
    spawnedPlayer.energyText = associatedEnergyText;

    Debug.Log(spawnedPlayer.attackPower);

  

   

    return spawnedPlayer; // Ensure a Character is always returned
}

}
