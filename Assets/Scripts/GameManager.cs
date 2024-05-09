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
    [SerializeField]
    private List<Item> allItemsMasterList = new List<Item>(); //All items in game

    public List<Item> itemList = new List<Item>(); //Items player has
    public int maxInventorySlots = 16;
    
    

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

    public int accountLevel;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            currentStageIndex = PlayerData.Instance.currentStageIndex; //Gets the current stage from PlayerData
            maxInventorySlots = 16;
            Debug.Log("Initial item list count: " + itemList.Count);

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
        LoadInventory();   
    }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject inventoryPanel = GameObject.Find("Inventory Panel");  // Adjust the name as per your hierarchy
         inventory.UpdateInventoryUI();
         LoadAllCompanionData();
          
    }

    

    public void LoadAllItems()
{
    // Assuming all item prefabs are stored under a Resources/Items directory
    Item[] items = Resources.LoadAll<Item>("Items");
    itemList = new List<Item>(items);

    if (itemList == null || itemList.Count == 0)
    {
        Debug.LogError("Failed to load items or no items available.");
        return;
    }

    foreach (Item item in itemList)
    {
        Debug.Log("Loaded item: " + item.itemName);
    }
}


  
 public void AddItem(Item newItem)
    {
        Debug.Log("[GameManager] Adding item to inventory: " + (newItem != null ? newItem.itemName : "null"));

    if (newItem == null) {
        Debug.LogError("Attempted to add a null item to the inventory.");
        return;
    }

    if (itemList == null) {
        Debug.LogError("Item list is null.");
        itemList = new List<Item>();  // Initialize if null
    }
        if (itemList.Count >= maxInventorySlots)
        {
            Debug.Log("Inventory is full!");
            return;
        }
        Debug.Log("[GameManager] Adding item to inventory: " + (newItem != null ? newItem.itemName : "null"));

        var item = itemList.Find(i => i.itemID == newItem.itemID);
        if (item != null)
        {
            item.quantity++;
        }
        else
        {
            newItem.quantity = 1;
            itemList.Add(newItem);
        }
        Debug.Log("[GameManager] Adding item to inventory: " + (newItem != null ? newItem.itemName : "null"));

        SaveInventory();
        inventory.UpdateInventoryUI();
    }

    public Item FindItemInMasterList(int id)
{
    foreach (Item item in allItemsMasterList)
    {
        if (item.itemID == id)
            return item;
    }
    Debug.LogWarning("Item with ID " + id + " not found in master list.");
    return null;
}

    public void RemoveItem(Item item)
    {
        Item foundItem = itemList.Find(i => i.itemID == item.itemID);
        if (foundItem != null && foundItem.quantity > 1)
        {
            foundItem.quantity--;
        }
        else
        {
            itemList.Remove(foundItem);
        }
        SaveInventory();
        inventory.UpdateInventoryUI();
    }

    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(new ItemContainer { Items = itemList }, true);
        System.IO.File.WriteAllText($"{Application.persistentDataPath}/inventory.json", json);
        Debug.Log("Inventory saved.");
    }

    public void LoadInventory()
{
    string filePath = $"{Application.persistentDataPath}/inventory.json";
    if (System.IO.File.Exists(filePath))
    {
        string json = System.IO.File.ReadAllText(filePath);
        ItemContainer itemContainer = JsonUtility.FromJson<ItemContainer>(json);
        if (itemContainer != null && itemContainer.Items != null)
        {
            itemList.Clear(); // Clear the current inventory list
            foreach (var itemData in itemContainer.Items)
            {
                // Use FindItemInMasterList to match itemData with the actual item object
                Item item = FindItemInMasterList(itemData.itemID); // Ensure itemData has itemID
                if (item != null)
                {
                    item.quantity = itemData.quantity; // Set the quantity from the saved data
                    itemList.Add(item); // Add to the player's inventory
                }
            }
            inventory.UpdateInventoryUI(); // Update UI to reflect the loaded inventory
        }
        else
        {
            Debug.LogError("Failed to parse inventory data.");
        }
    }
    else
    {
        Debug.Log("No inventory save file found at: " + filePath);
    }
}






    [System.Serializable]
    class ItemContainer
    {
        public List<Item> Items;
    }


    public Item testItem;
    public void AddTestItem()
    {
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
    bool hasUnlockedAny = false; // Track if any new stages were unlocked

    foreach (Stage connectedStage in completedStage.connectedStages)
    {
        if (connectedStage.stageID != null){
        if (!UnlockedStageNames.Contains(connectedStage.stageID))
        {
            Debug.Log($"Unlocking connected stage: {connectedStage.stageID}");
            UnlockedStageNames.Add(connectedStage.stageID);
            hasUnlockedAny = true; // Indicate that a new stage has been unlocked
        }
        }
        else
        {
            Debug.Log($"Stage already unlocked: {connectedStage.stageID}");
        }
    }

    // If any new stages were unlocked, save the updated list
    if (hasUnlockedAny)
    {
        SaveUnlockedStages();
    }
}

private void SaveUnlockedStages()
{
    // Convert HashSet to a List to serialize
    List<string> unlockedStagesList = new List<string>(UnlockedStageNames);

    // Convert the list to a JSON string
    string json = JsonUtility.ToJson(new StageList { Stages = unlockedStagesList });

    // Save the JSON string to PlayerPrefs
    PlayerPrefs.SetString("UnlockedStages", json);
    PlayerPrefs.Save();
    Debug.Log("Unlocked stages saved.");
}

[System.Serializable]
private class StageList
{
    public List<string> Stages;
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
    DeleteSavedInventory();
    PlayerData.Instance.ResetSteps();
}

public void DeleteSavedInventory()
    {
        string filePath = $"{Application.persistentDataPath}/inventory.json";
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            itemList.Clear();
            inventory.UpdateInventoryUI();
            Debug.Log("Saved inventory data deleted.");
        }
        else
        {
            Debug.Log("No saved inventory data to delete.");
        }
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
                RestoreHealthAndEnergyForCompanion(companion);
                companion.SaveCharacterData(); // Save updated data
            }      
        }
    }

    public void RestoreHealthAndEnergyForCompanion(Companion companion)
    {
                companion.health = companion.maxHealth;
                companion.energy = companion.maxEnergy;
    }
    private void RecoverCompanion()
    {
        currentCompanion.health = currentCompanion.maxHealth;
        currentCompanion.energy = currentCompanion.maxEnergy;
        Debug.Log("Current Companion " + currentCompanion);
        currentCompanion.SaveCharacterData();
    }

    public void RecoverForSteps()
    {
       if (PlayerData.Instance.UseSteps(100))
       {
        RecoverCompanion();
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
