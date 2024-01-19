using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionSpawnController : MonoBehaviour
{
    public GameObject companionPrefab;
    public Transform[] companionSpawnPoints; // Assign in the inspector
    public Slider associatedHealthBarSlider; // Reference to the slider
    public Slider associatedEnergyBarSlider;
    public TextMeshProUGUI associatedHealthText;
    public TextMeshProUGUI associatedEnergyText;
    public Slider associatedHealthBarSlider2; // Reference to the slider
    public Slider associatedEnergyBarSlider2;
    public TextMeshProUGUI associatedHealthText2;
    public TextMeshProUGUI associatedEnergyText2;
    public Slider associatedtempEnergyBarSlider2;
    public BattleManager battleManager;

    private Character[] activeCompanions;

    private void Start()
    {
        // Initialize the array based on the number of spawn points
        activeCompanions = new Character[companionSpawnPoints.Length];
    }

    public void SetupCompanion(Character companionCharacter)
    {
        int spawnIndex = FindNextEmptySpot();
        if (spawnIndex == -1)
        {
            Debug.LogError("No empty spot available for the companion.");
            return;
        }

        Transform spawnPoint = companionSpawnPoints[spawnIndex];
        companionCharacter.transform.position = spawnPoint.position;
        companionCharacter.transform.rotation = spawnPoint.rotation;

        // Setup health and energy bars based on spawn index
        if (spawnIndex == 0)
        {
            SetupBars(companionCharacter, associatedHealthBarSlider2, associatedEnergyBarSlider2, associatedHealthText2, associatedEnergyText2);
        }
        else if (spawnIndex == 1)
        {
            SetupBars(companionCharacter, associatedHealthBarSlider, associatedEnergyBarSlider, associatedHealthText, associatedEnergyText);
        }

        // Update the active companions array
        activeCompanions[spawnIndex] = companionCharacter;
    }

    private void SetupBars(Character character, Slider healthBar, Slider energyBar, TextMeshProUGUI healthText, TextMeshProUGUI energyText)
    {
        healthBar.maxValue = character.maxHealth;
        healthBar.value = character.health;
        character.healthBar = healthBar;
        healthText.text = character.health.ToString();

        energyBar.maxValue = character.maxEnergy;
        energyBar.value = character.energy;
        character.energyBar = energyBar;
        energyText.text = character.energy.ToString();
    }

    private int FindNextEmptySpot()
    {
        for (int i = 0; i < activeCompanions.Length; i++)
        {
            if (activeCompanions[i] == null)
            {
                return i;
            }
        }
        return -1; // No empty spot found
    }

    public void RemoveCompanion(Character companionCharacter)
    {
        for (int i = 0; i < activeCompanions.Length; i++)
        {
            if (activeCompanions[i] == companionCharacter)
            {
                activeCompanions[i] = null; // Clear the spot
                break;
            }
        }
    }
}