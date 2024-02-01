using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    public Character[] activeCompanions;

    private void Start()
    {
        // Initialize the array based on the number of spawn points
        activeCompanions = new Character[companionSpawnPoints.Length];
    }

    public void SetupCompanion(Character companionCharacter)
    {
        // Check if a companion of the same type is already active
        for (int i = 0; i < activeCompanions.Length; i++)
        {
            if (activeCompanions[i] != null && activeCompanions[i].characterIDNumber == companionCharacter.characterIDNumber)
            {
                RemoveCompanion(activeCompanions[i]);
                Debug.Log("A companion of the same type is already active and has been removed.");
                break;
            }
        }

        int spawnIndex = FindNextEmptySpot();
        if (spawnIndex == -1)
        {
            Debug.Log("No empty spot available for the companion.");
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
    companionCharacter.isSelected = true;
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
            if (activeCompanions[i] != null && activeCompanions[i].characterIDNumber == companionCharacter.characterIDNumber)
            {
                activeCompanions[i].isSelected = false;
                Destroy(activeCompanions[i].gameObject);
                activeCompanions[i] = null;
                break;
            }
        }
    }


    public void CreateHeroSelectionUI()
{
    Debug.Log("Creating Hero Selection UI");

    // Clear existing buttons
    foreach (Transform child in battleManager.heroSelectionPanel.transform)
    {
        Debug.Log("Destroying existing button: " + child.gameObject.name);
        Destroy(child.gameObject);
    }

    // Log the count of companions
    Debug.Log("Number of companions: " + GameManager.Instance.companions.Count);

    // Create a button for each companion
    foreach (Companion companion in GameManager.Instance.companions)
{
    Debug.Log("Creating button for: " + companion.heroID);
    GameObject buttonObj = Instantiate(battleManager.heroButtonPrefab, battleManager.heroSelectionPanel.transform);
    buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = companion.heroID;

    Companion localCompanion = companion; // Local copy
    Debug.Log("This button's attached companion is: " + localCompanion);
    buttonObj.GetComponent<Button>().onClick.AddListener(() => OnHeroSelected(localCompanion));
}


    // Make the panel visible
    battleManager.heroSelectionPanel.SetActive(true);

    // Activate the battle start button and add click listener
       battleManager.battleStartButton.SetActive(true);
        battleManager.battleStartButton.GetComponent<Button>().onClick.AddListener(() => battleManager.StartBattle(GameManager.Instance.CurrentBattleConfig));
   
}

 private int selectedCompanionCount = 0; // To track the number of companions selected

    public void OnHeroSelected(Companion selectedCompanion)
{
    // Check if the companion is already selected
    if (selectedCompanion.isSelected)
    {
        // If already selected, remove the companion
        Debug.Log("Companion is already selected, removing");
        RemoveCompanionFromGame(selectedCompanion);
        selectedCompanion.isSelected = false;
    }
    else
    {
        // If not selected, add the companion
        Debug.Log("Companion wasn't selected, adding");
        Character instantiatedCompanion = GameManager.Instance.InstantiateSelectedCompanion(selectedCompanion.heroID);
        if (instantiatedCompanion != null)
        {
            SetupSelectedCompanion(instantiatedCompanion);
            AssignCompanion(instantiatedCompanion);
            selectedCompanion.isSelected = true;
        }
    }
}

private void RemoveCompanionFromGame(Companion companion)
{
    // Find the instantiated character in the game and remove it
    foreach (var character in activeCompanions)
    {
        if (character != null && character.characterIDNumber == companion.characterIDNumber)
        {
            RemoveCompanion(character);
            break;
        }
    }
}


    private void AssignCompanion(Character companion)
    {
        if (selectedCompanionCount == 0)
        {
            battleManager.companion1 = companion;
        }
        else if (selectedCompanionCount == 1)
        {
            battleManager.companion2 = companion;
        }

        selectedCompanionCount++;
    }


    public void SetupSelectedCompanion(Character selectedCompanion)
    {
        SetupCompanion(selectedCompanion);
        battleManager.playerParty.Add(selectedCompanion);
        Debug.Log(selectedCompanion.transform.position);
    }

} 