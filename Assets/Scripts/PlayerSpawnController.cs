using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawnController : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerSpawnPoint; // You can assign this directly in the inspector
    public Slider associatedHealthBarSlider; // Reference to the slider
    public Slider associatedEnergyBarSlider;

   

    public void SpawnPlayerAtPoint(Transform spawnPoint)
    {
        // Instantiate the player at the position of spawnPoint and with its rotation
        GameObject spawnedPlayerObject = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Character spawnedPlayer = spawnedPlayerObject.GetComponent<Character>();

        // Check if the character script exists on the spawned player
        if (spawnedPlayer == null)
        {
            Debug.LogError("Character component not found on spawned player.");
            return;
        }

        // Link this character to the health bar
        spawnedPlayer.healthBar = associatedHealthBarSlider;
        associatedHealthBarSlider.gameObject.SetActive(true); // Activate the health bar
        associatedHealthBarSlider.maxValue = spawnedPlayer.maxHealth;
        associatedHealthBarSlider.value = spawnedPlayer.health;

        spawnedPlayer.energyBar = associatedEnergyBarSlider;
        associatedEnergyBarSlider.gameObject.SetActive(true); // Activate the energy bar
        associatedEnergyBarSlider.maxValue = spawnedPlayer.maxEnergy;
        associatedEnergyBarSlider.value = spawnedPlayer.energy;
    }
}
