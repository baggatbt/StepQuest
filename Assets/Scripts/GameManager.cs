using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

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
    public EquipmentManager equipmentManager; // Reference to the EquipmentManager

    // Centralized item list managed by GameManager
    [SerializeField]
    private List<Item> allItemsMasterList = new List<Item>(); //All items in game

    public List<Item> itemList = new List<Item>(); //Items player has
    public int maxInventorySlots = 16;
    public List<Companion> currentParty = new List<Companion>();

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

    //Testing data object implementation
    public List<CharacterData> allCharacterData = new List<CharacterData>(); 
    public CharacterData currentCompanionData;
    public CharacterData knightData;
    

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
            LoadUnlockedStages(); // Ensure this is called to initialize unlocked stages

            //Each companion has equal chance to be selected for attack, need to change so it uses the class's variable threat
            probabilityCompanion1 = 1f;
            probabilityCompanion2 = 1f;
            probabilityCompanion3 = 1f;

            Debug.Log(PlayerData.Instance.firstTimeLogin);
            if (PlayerData.Instance.firstTimeLogin)
            {
                // Handle first-time login logic
            }
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;  // Set target frame rate to 60 FPS.
    }

    public Companion InstantiateCharacter(GameObject characterPrefab, CharacterData characterData, Vector3 position, Quaternion rotation)
    {
        GameObject characterObject = Instantiate(characterPrefab, position, rotation);
        Companion companion = characterObject.GetComponent<Companion>();
        companion.characterData = characterData;
        return companion;
    }

    public GameObject GetCharacterPrefab(string heroID)
{
    switch (heroID)
    {
        case "Knight":
            return knightPrefab;
        case "Archer":
            return archerPrefab;
        // Add cases for other characters
        default:
            return null;
    }
}

    [Serializable]
    private class SerializableCompanionData
    {
        public string type;
        public string json;
    }

    public void SaveCurrentParty()
    {
        List<string> companionDataList = new List<string>();
        foreach (var companion in currentParty)
        {
            string json = SerializationHelper.SerializeCompanion(companion);
            companionDataList.Add(json);
        }
        string jsonList = JsonUtility.ToJson(new SerializableList<string>(companionDataList));
        PlayerPrefs.SetString("CurrentParty", jsonList);
        PlayerPrefs.Save();
    }

    public void LoadCurrentParty()
    {
        currentParty.Clear();
        string jsonList = PlayerPrefs.GetString("CurrentParty", "{}");
        var companionDataList = JsonUtility.FromJson<SerializableList<string>>(jsonList);

        foreach (var json in companionDataList.Items)
        {
            CompanionData data = SerializationHelper.DeserializeCompanionData(json);
          //  Companion companion = InstantiateCompanion(data);
          //  currentParty.Add(companion);
        }
    }

    private Companion InstantiateCompanion(CompanionData data)
    {
        GameObject prefab = null;
        if (data.heroID == "Knight")
        {
            prefab = Instantiate(knightPrefab);  // Assign your Knight prefab here
        }
        // Handle other heroID cases...

        if (prefab != null)
        {
            var companion = prefab.GetComponent<Companion>();
            companion.heroID = data.heroID;
            companion.heroLevel = data.heroLevel;
            // Set other properties...

            if (data is KnightData knightData && companion is Knight knight)
            {
                
                // Set other Knight-specific properties...
            }

            return companion;
        }

        return null;
    }

    [Serializable]
    private class SerializableList<T>
    {
        public List<T> Items;

        public SerializableList(List<T> items)
        {
            Items = items;
        }
    }

    private void Start()
    {
        TimerManager.Instance.OnTimerCompleted += HandleTimerCompletion;
        TimerManager.Instance.SetPeriodicTimer("StaminaIncrement", 1);  // 1800 seconds = 30 minutes

        LoadInventory();  
    }

    private void HandleTimerCompletion(string timerId)
    {
        if (timerId == "StaminaIncrement")
        {
            IncrementCompanionStamina();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReassignInventoryComponent();
        LoadAllCompanionData();
    }

    private void ReassignInventoryComponent()
    {
        GameObject inventoryPanel = GameObject.Find("Inventory Panel");
        if (inventoryPanel != null)
        {
            inventory = inventoryPanel.GetComponent<Inventory>();
            if (inventory != null)
            {
                Debug.Log("Inventory component reassigned successfully.");
                inventory.UpdateInventoryUI();  // Optionally update UI here if it's safe to do so
            }
            else
            {
                Debug.LogError("Failed to find Inventory component on the Inventory Panel.");
            }
        }
        else
        {
            Debug.Log("Inventory Panel not found in the scene. This may be expected in some scenes.");
        }
    }

    void OnEnable()
    {
        Debug.Log("SceneManagement is enabled and registering to sceneLoaded event.");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Debug.Log("SceneManagement is disabled and unregistering from sceneLoaded event.");
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

        if (newItem == null)
        {
            Debug.LogError("Attempted to add a null item to the inventory.");
            return;
        }

        if (itemList == null)
        {
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
        GameObject inventoryPanel = GameObject.Find("Inventory Panel"); // Adjust the name as per your hierarchy
        if (inventoryPanel != null && inventory != null)
        {
            inventory.UpdateInventoryUI();
        }
        else
        {
            Debug.LogError("InventoryPanel or inventory is null.");
        }
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

    [Serializable]
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
            if (!UnlockedStageNames.Contains(connectedStage.stageID))
            {
                Debug.Log($"Unlocking connected stage: {connectedStage.stageID}");
                UnlockedStageNames.Add(connectedStage.stageID);
                connectedStage.isUnlocked = true;
                //  connectedStage.UpdateButtonColor();
                hasUnlockedAny = true; // Indicate that a new stage has been unlocked
            }
        }

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

    private void LoadUnlockedStages()
    {
        string json = PlayerPrefs.GetString("UnlockedStages", "{}");
        if (json != "{}")
        {
            StageList stageList = JsonUtility.FromJson<StageList>(json);
            UnlockedStageNames = new HashSet<string>(stageList.Stages);
        }
        else
        {
            // Setup default unlocked stages
            UnlockedStageNames.Add("0"); // Default first stage unlocked
        }
    }

    public void ResetStagesOnBossDefeat()
    {
        // Clear all currently unlocked stages
        UnlockedStageNames.Clear();

        // Re-unlock the initial stage (assuming stage ID "0" is your initial stage ID)
        UnlockedStageNames.Add("0");

        // Optionally, force update UI or state of all stages if they are listening to changes
        // UpdateAllStagesState();

        // Save the updated stage unlocks
        SaveUnlockedStages();
    }

    [Serializable]
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
                companion.SaveCharacterData();
                Debug.Log("Deleted data for " + companion.name);
            }
        }

        // After individual deletions, clear all PlayerPrefs
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted");
        DeleteSavedInventory();
        PlayerData.Instance.ResetSteps();
        PlayerData.Instance.SavePlayerData();
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
                Debug.Log("Companion stamina: " + companion.stamina);
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
        currentCompanion.stamina = currentCompanion.maxStamina;
        Debug.Log("Current Companion " + currentCompanion);
        currentCompanion.SaveCharacterData();
    }

    private void IncrementCompanionStamina()
    {
        foreach (Companion companion in companions)
            if (companion.stamina < companion.maxStamina)
            {
                companion.stamina += 1;
            }
    }

    public void RecoverAllForSteps()
    {
        int stepsToUse = CalculateStepCostForResting();

        if (PlayerData.Instance.UseSteps(stepsToUse))
        {
            RecoverCompanion();
        }
        else
        {
            Debug.Log("not enough steps");
        }
    }

    public int CalculateStepCostForResting() //Later include modifiers for upgraded player buildings
    {
        int staminaToRecover = (currentCompanion.maxStamina - currentCompanion.stamina);
        int calculatedStepCost = (staminaToRecover * 200); //1000 steps for full recovery

        return calculatedStepCost;
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
       // CreateAndLoadCompanion(knightPrefab, "Knight");
      //  CreateAndLoadCompanion(archerPrefab, "Archer");
       // CreateAndLoadCompanion(wizardPrefab, "Wizard");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        SaveAllCompanionData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveAllCompanionData();
        }
    }

    public Equipment GetItemForSlot(EquipmentType slotType)
    {
        // Loop through the itemList to find an equipment item that matches the slot type
        foreach (Item item in itemList)
        {
            if (item is Equipment equipment && equipment.equipmentType == slotType)
            {
                inventory.UpdateInventoryUI();
                return equipment;
            }
        }
        // If no matching item is found, return null
        return null;
    }
}
