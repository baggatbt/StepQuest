using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawnController : MonoBehaviour
{
    public GameObject playerPrefab;

    public Character SpawnPlayer(Transform spawnPoint, Slider associatedHealthBarSlider)
    {
        // Instantiate the player at the position of spawnPoint and with its rotation
        GameObject spawnedPlayerObject = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Character spawnedPlayer = spawnedPlayerObject.GetComponent<Character>();

        // Link this character to the health bar
        spawnedPlayer.healthBar = associatedHealthBarSlider;
        associatedHealthBarSlider.gameObject.SetActive(true);  // Activate the health bar
        associatedHealthBarSlider.maxValue = spawnedPlayer.maxHealth;
        associatedHealthBarSlider.value = spawnedPlayer.health;

        return spawnedPlayer;
    }
}
