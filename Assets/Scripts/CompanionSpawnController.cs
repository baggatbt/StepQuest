using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // Needed for LINQ queries such as FirstOrDefault
using System.Collections.Generic; // Needed for List
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

    public List<Character> activeCompanions = new List<Character>();


    private void Start()
    {
        
    }

    public void SetupCompanion(Character companionCharacter)
{
    // Removing existing companion of the same type if present
    var existingCompanion = activeCompanions.FirstOrDefault(c => c.characterIDNumber == companionCharacter.characterIDNumber);
    if (existingCompanion != null)
    {
        RemoveCompanion(existingCompanion);
    }

    // Ensure you don't add more companions than spawn points or UI elements
    if (activeCompanions.Count >= companionSpawnPoints.Length)
    {
        Debug.Log("All spawn points are occupied.");
        return;
    }

    int spawnIndex = activeCompanions.Count; // Use the count for positioning
    Transform spawnPoint = companionSpawnPoints[spawnIndex];

    // Setting companion's position
    companionCharacter.transform.position = spawnPoint.position;
    companionCharacter.transform.rotation = spawnPoint.rotation;

    // Dynamically assign UI elements based on the current number of active companions
    if (activeCompanions.Count % 2 == 0)
    {
        SetupBars(companionCharacter, associatedHealthBarSlider, associatedEnergyBarSlider, associatedHealthText, associatedEnergyText);
    }
    else
    {
        SetupBars(companionCharacter, associatedHealthBarSlider2, associatedEnergyBarSlider2, associatedHealthText2, associatedEnergyText2);
    }

    // Adding to the list
    activeCompanions.Add(companionCharacter);
    companionCharacter.isSelected = true;
}



    private void SetupBars(Character character, Slider healthBar, Slider energyBar, TextMeshProUGUI healthText, TextMeshProUGUI energyText)
    {
        character.healthText = healthText;
        healthBar.maxValue = character.maxHealth;
        healthBar.value = character.health;
        character.healthBar = healthBar;
        healthText.text = character.health.ToString();

        character.energyText = energyText;
        energyBar.maxValue = character.maxEnergy;
        energyBar.value = character.energy;
        character.energyBar = energyBar;
        energyText.text = character.energy.ToString();
    }

    

    public void RemoveCompanion(Character companionCharacter)
{
    if (activeCompanions.Remove(companionCharacter)) // This now automatically removes the companion
    {
        companionCharacter.isSelected = false;
        DisableHeroUI(companionCharacter);

        // Remove the companion from the BattleManager's playerParty list
        if (battleManager.playerParty.Contains(companionCharacter))
        {
            battleManager.playerParty.Remove(companionCharacter);
        }

        Destroy(companionCharacter.gameObject);
    }
}


   

    // Update the RemoveCompanionFromGame method to work with the list
    private void RemoveCompanionFromGame(Companion companion)
    {
        var characterToRemove = activeCompanions.FirstOrDefault(c => c.characterIDNumber == companion.characterIDNumber);
        if (characterToRemove != null)
        {
            RemoveCompanion(characterToRemove);
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
        selectedCompanionCount --;
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
            EnableHeroUI(instantiatedCompanion);
            selectedCompanion.isSelected = true;
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

    private void EnableHeroUI(Character selectedCompanion)
    {
       selectedCompanion.healthBar.gameObject.SetActive(true);
       selectedCompanion.energyBar.gameObject.SetActive(true);
    }

    private void DisableHeroUI(Character selectedCompanion)
    {
        selectedCompanion.healthBar.gameObject.SetActive(false);
        selectedCompanion.energyBar.gameObject.SetActive(false);
    }

} 