using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//Storing and managing game states across scenes
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject knightPrefab; 
    public GameObject archerPrefab; 
    public GameObject wizardPrefab;
    public GameObject characterSelectionPanel;
    public BattleConfig CurrentBattleConfig { get; set; }
    public HashSet<string> UnlockedStageNames = new HashSet<string>();
    public Character companion1; 
    public Character companion2; 
    public Character companion3;
    public Companion currentCompanion; //Used to expose the selected companion 
    public Inventory inventory;
    // Centralized item list managed by GameManager
    public List<Item> itemList = new List<Item>();
    

    public int currentStageIndex;
    private bool isUnlocked;
     public List<BattleConfig> allStages; // list is populated with all stages in order

    public List<Companion> companions = new List<Companion>();

    //Instantiate any unlocked characters
    public Knight knight;
    public Wizard wizard;
    public Archer archer;
    
    //Used in EnemyAttack() to weight enemy targets
    public float probabilityCompanion1; // Default probability for companion1 to be attacked
    public float probabilityCompanion2;// Default probability for companion2 to be attacked
    public float probabilityCompanion3;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            currentStageIndex = PlayerData.Instance.currentStageIndex; //Gets the current stage from PlayerData

            //Each companion has equal chance to be selected for attack, need to change so it uses the class's variable threat
            probabilityCompanion1 = 1f;
            probabilityCompanion2 = 1f;
            probabilityCompanion3 = 1f;
            
            
            Debug.Log(PlayerData.Instance.firstTimeLogin);
           if (PlayerData.Instance.firstTimeLogin)
        {
           
        }
        }
        else
        {
            Destroy(gameObject);
        }
         Application.targetFrameRate = 60;  // Set target frame rate to 60 FPS.
    }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
         LoadAllCompanionData();
          
    }

    public int maxInventorySlots = 16; // Maximum number of slots
public void AddItem(Item newItem)
{
    bool itemExists = false;
    foreach (Item item in itemList) {
        if (item.itemID == newItem.itemID) {
            item.quantity++;
            Debug.Log("item quantity: " + item.quantity);
            itemExists = true;
            inventory.SaveInventory();
            break; // Exit the loop after finding and incrementing the item
        }
    }
    
    if (!itemExists) {
        // If the item doesn't exist, add it to the list with a quantity of 1
        newItem.quantity = 1;
        itemList.Add(newItem);
        inventory.SaveInventory();
    }
}

        
        

    // Remove item from the list
    public void RemoveItem(Item item) {
        if (itemList.Contains(item)) {
            itemList.Remove(item);
        }
        // Optionally, trigger any necessary updates or notifications
    }

    public Item testItem;
    public void AddTestItem() {
        AddItem(testItem);
    }

    

    
    public Character InstantiateSelectedCompanion(string heroID)
    {
        GameObject companionObject = null;

        switch (heroID)
        {
            case "Knight":
                companionObject = Instantiate(knightPrefab);

                break;
            case "Archer":
                companionObject = Instantiate(archerPrefab);
                break;
            case "Wizard":
                companionObject = Instantiate(wizardPrefab);
                break;
            // Add cases for other companions
        }
         if (companionObject != null)
        {
            Companion companion = companionObject.GetComponent<Companion>();
            companion.LoadCharacterData();
            return companion;
        }

        return null;
    }

    private void CreateAndLoadCompanion(GameObject prefab, string heroID)
    {
        GameObject companionObject = Instantiate(prefab);
        Companion companion = companionObject.GetComponent<Companion>();
        if (companion != null)
        {
            companion.heroID = heroID;
            companion.LoadCharacterData(); // Load saved data or initialize with default values
            RegisterCompanion(companion);
        }
    }


    public void UnlockConnectedStages(Stage completedStage)
{
    Debug.Log($"Unlocking stages connected to: {completedStage.stageID}");

    foreach (Stage connectedStage in completedStage.connectedStages)
    {
        Debug.Log($"Checking connected stage: {connectedStage.stageID}");

        if (!UnlockedStageNames.Contains(connectedStage.stageID))
        {
            Debug.Log($"Unlocking connected stage: {connectedStage.stageID}");
            UnlockedStageNames.Add(connectedStage.stageID);
        }
        else
        {
            Debug.Log($"Stage already unlocked: {connectedStage.stageID}");
        }
    }
}


   

    
    //Testing method for deleting all saved data
    public void DeleteEverything()
{
    // Delete data for each companion
    foreach (var companion in companions)
    {
        if (companion != null)
        {
            companion.DeleteCharacterData();
            Debug.Log("Deleted data for " + companion.name);
        }
    }

    // After individual deletions, clear all PlayerPrefs
    PlayerPrefs.DeleteAll();
    Debug.Log("All PlayerPrefs deleted");
}

// Method to add a companion to the list
   public void RegisterCompanion(Companion companion)
    {
        if (!companions.Contains(companion))
        {
            companions.Add(companion);
        }
    }

    public void UpdateCompanionExp(string heroID, int expGained)
    {
        foreach (var companion in companions)
        {
            if (companion.heroID == heroID)
            {
                companion.heroExp += expGained;
                companion.LevelUp(); // Check and apply level up if applicable
                companion.SaveCharacterData(); // Save updated data
            }
        }
    }

    public void UpdateCompanionStamina(string heroID)
    {
        foreach (var companion in companions)
        {
            if (companion.heroID == heroID && companion.stamina > 0)
            {
                companion.stamina -= 1;
                companion.SaveCharacterData(); // Save updated data
            }
            
            
        }
    }


    // Call this method to save the data of all companions
    public void SaveAllCompanionData()
    {
        foreach (var companion in companions)
        {
            companion?.SaveCharacterData();
            Debug.Log("In game manager saving: " + companion);
        }
    }


    private void LoadAllCompanionData()
    {
        // Clear existing companions list to repopulate it
        companions.Clear();
        
        // Instantiate and load data for each companion type
        CreateAndLoadCompanion(knightPrefab, "Knight");
        CreateAndLoadCompanion(archerPrefab, "Archer");
        CreateAndLoadCompanion(wizardPrefab, "Wizard");
    }

    
     void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnApplicationQuit()
    {
      // SaveAllCompanionData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveAllCompanionData();
        }
    }

    
   
    

}
