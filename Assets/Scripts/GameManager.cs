using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.EventSystems;


//Storing and managing game states across scenes
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
     public List<StageData> allStagesData = new List<StageData>(); // List of all stages
    private Dictionary<string, StageData> stageDictionary = new Dictionary<string, StageData>();
    // Ephemeral "run" state stages reset on loss
    public bool InDungeonRun { get; private set; }
    private HashSet<string> runUnlockedStageIDs = new HashSet<string>();
    public event Action OnRunUnlocksChanged;



    public StageData currentStage; // The current stage being played
    public GameObject knightPrefab; 
    public GameObject archerPrefab; 
    public GameObject wizardPrefab;
    public GameObject tamedGoblinPrefab;

    public GameObject characterSelectionPanel;
    public BattleConfig CurrentBattleConfig; 
    public Stage CurrentStage; // Track the current stage
    public HashSet<string> UnlockedStageNames = new HashSet<string>();
    public Character companion1; 
    public Character companion2; 
    public Character companion3;
    public Companion currentCompanion; //Used to expose the selected companion 
    public Inventory inventory;
    public EquipmentManager equipmentManager; // Reference to the EquipmentManager

    //WORKER SECTION
    public int totalWorkers = 2;
    public int availableWorkers;

    // Centralized item list managed by GameManager
    [SerializeField]
    private List<Item> allItemsMasterList = new List<Item>(); //All items in game

    public List<Item> itemList = new List<Item>(); //Items player has
    public int maxInventorySlots = 16;
    public List<Companion> currentParty = new List<Companion>(); //This is getting an instance, NOT the companions data

    public int currentStageIndex;
    private bool isUnlocked;
    //public List<BattleConfig> allStages; // list is populated with all stages in order

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
    public Dictionary<string, CharacterData> characterDataDictionary = new Dictionary<string, CharacterData>();
    public List<CharacterData> allCharacterData = new List<CharacterData>(); 
    public CharacterData currentCompanionData;
    public CharacterData knightData;
    public CharacterData archerData;
    

    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        //currentStageIndex = PlayerData.Instance.currentStageIndex; //Gets the current stage from PlayerData
        maxInventorySlots = 16;
        Debug.Log("Initial item list count: " + itemList.Count);
        InitializeStageDictionary(); // Initialize dictionary at game start

        // Initialize character data dictionary here
        foreach (var data in allCharacterData)
        {
            data.LoadData(); // Load data for each character
            characterDataDictionary[data.heroID] = data;
        }

        //Each companion has equal chance to be selected for attack, need to change so it uses the class's variable threat
        probabilityCompanion1 = 1f;
        probabilityCompanion2 = 1f;
        probabilityCompanion3 = 1f;

        
        
       
        
            // Mimic the AddToParty function for the knight companion using knightData
            if (currentParty.Count < 2)
            {
                if (!currentParty.Any(companion => companion.characterData == knightData))
                {
                    // Get the prefab associated with knightData.heroID
                    GameObject characterPrefab = GetCharacterPrefab(knightData.heroID);
                    if (characterPrefab != null)
                    {
                        // Instantiate the companion and set its character data
                        Companion instantiatedCompanion = Instantiate(characterPrefab).GetComponent<Companion>();
                        instantiatedCompanion.SetCharacterData(knightData);
                        
                        // Add the instantiated companion to the party
                        currentParty.Add(instantiatedCompanion);
                        Debug.Log("Added to party: " + knightData.heroID);
                    }
                    else
                    {
                        Debug.LogError("Character prefab not found for heroID: " + knightData.heroID);
                    }
                }
                else
                {
                    Debug.Log("Companion already in party: " + knightData.heroID);
                }
            }
            else
            {
                Debug.LogError("Party is full. Cannot add more companions.");
            }
        
        Application.quitting += SaveAllCharacterData; // Save all character data on application quit
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
        case "TamedGoblin":
            return tamedGoblinPrefab;
    
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

    // Initialize the stage dictionary for quick lookups
    private void InitializeStageDictionary()
    {
        foreach (StageData stageData in allStagesData)
        {
            if (!stageDictionary.ContainsKey(stageData.stageID))
            {
                stageDictionary.Add(stageData.stageID, stageData);
            }
        }
    }
    /*
    public void GetNextStage(int nextIndex)
    {
        if (currentStage != null && nextIndex < currentStage.connectedStageIDs.Count)
        {
            string nextStageID = currentStage.connectedStageIDs[nextIndex];
            if (stageDictionary.TryGetValue(nextStageID, out StageData nextStage))
            {
                SaveCurrentParty();
                currentStage = nextStage; // Update the current stage to the new one
                Debug.Log("Transitioning to next stage: " + nextStage.stageID);
                CurrentBattleConfig = currentStage.stageBattleConfig;
                SceneManager.LoadScene(currentStage.battleSceneName, LoadSceneMode.Single);

            }
            else
            {
                Debug.LogError("No stage found with ID: " + nextStageID);
            }
        }
        else
        {
            Debug.LogWarning("Invalid stage transition index: " + nextIndex);
        }
    }

    */

    public void SaveCurrentParty()
    {
        Debug.Log("Attempting to save party");
        List<string> companionDataList = new List<string>();
       foreach (var companion in currentParty)
{
    if (string.IsNullOrEmpty(companion.heroID))
        Debug.LogError("Companion has missing heroID!");

    string json = SerializationHelper.SerializeCompanion(companion);
    companionDataList.Add(json);
}

        string jsonList = JsonUtility.ToJson(new SerializableList<string>(companionDataList));
        PlayerPrefs.SetString("CurrentParty", jsonList);
        PlayerPrefs.Save();
    }

    public void LoadCurrentParty()
{
    // Do we even have saved party data?
    if (!PlayerPrefs.HasKey("CurrentParty"))
    {
        Debug.Log("No saved party; keeping whatever is already in currentParty.");
        return;
    }

    string jsonList = PlayerPrefs.GetString("CurrentParty", "");
    if (string.IsNullOrEmpty(jsonList))
    {
        Debug.Log("Saved party string empty; skipping load.");
        return;
    }

    Debug.Log("Loading saved party from PlayerPrefs…");
    currentParty.Clear();

    var companionDataList = JsonUtility.FromJson<SerializableList<string>>(jsonList);
    foreach (var json in companionDataList.Items)
    {
        var data = SerializationHelper.DeserializeCompanionData(json);
        // Prefer the authoritative ScriptableObject so we keep saved HP/EN/etc.
        var companion = InstantiateSelectedCompanion(data.heroID) as Companion;
        if (companion == null)
        {
            Debug.LogError($"Failed to instantiate companion for {data.heroID}");
            continue;
        }

        // If you persist level in your serialized snapshot, align it (HP/EN stay as in SO)
        if (data.heroLevel > 0)
            companion.heroLevel = data.heroLevel;

        // Persist any alignment back to SO
        companion.SaveCharacterData();

        currentParty.Add(companion);
        Debug.Log($"Loaded and added to party: {data.heroID} (HP {companion.health}/{companion.maxHealth})");
    }
}



    public void ClearCurrentParty()
    {
        currentParty.Clear();
    }

    private Companion InstantiateCompanion(CompanionData data)
{
    GameObject prefab = GetCharacterPrefab(data.heroID);
    if (prefab == null)
    {
        Debug.LogError($"[InstantiateCompanion] No prefab for {data.heroID}");
        return null;
    }

    var go = Instantiate(prefab);
    var companion = go.GetComponent<Companion>();

    // Use the ScriptableObject to pull the saved HP/EN/etc.
    if (characterDataDictionary.TryGetValue(data.heroID, out var so))
    {
        so.LoadData();                 // make sure we have freshest saved values
        companion.SetCharacterData(so); // applies saved health/energy into runtime instance

        // If your serialized snapshot stores level, align it (optional)
        if (data.heroLevel > 0)
            companion.heroLevel = data.heroLevel;

        companion.SaveCharacterData();
    }
    else
    {
        // Fallback: copy whatever the serialized data contains (only if you store these in CompanionData)
        Debug.LogWarning($"[InstantiateCompanion] No CharacterData SO found for {data.heroID}; using serialized snapshot.");
        companion.heroID    = data.heroID;
        companion.heroLevel = data.heroLevel;

        // Only do these if your CompanionData actually has them; otherwise omit.
        // companion.maxHealth = data.maxHealth;
        // companion.health    = Mathf.Clamp(data.health, 1, data.maxHealth);
        // companion.maxEnergy = data.maxEnergy;
        // companion.energy    = Mathf.Clamp(data.energy, 0, data.maxEnergy);
        // companion.attackPower  = data.attackPower;
        // companion.defensePower = data.defensePower;
        // companion.speed        = data.speed;
    }

    return companion;
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
        foreach (var i in itemList)
{
    Debug.Log($"Item: {i.name}, ID: {i.itemID}, Quantity: {i.quantity}");
}

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

        if (PlayerPrefs.HasKey("CurrentParty"))
            LoadCurrentParty();      // Load saved party with saved HP/EN from CharacterData
        else
            LoadDefaultPartyCharacter(); // First time: spawn default Knight using knightData
    }



    public void LoadDefaultPartyCharacter()
    {
        if (!currentParty.Any(companion => companion.characterData == knightData))
                {
                    // Get the prefab associated with knightData.heroID
                    GameObject characterPrefab = GetCharacterPrefab(knightData.heroID);
                    if (characterPrefab != null)
                    {
                        // Instantiate the companion and set its character data
                        Companion instantiatedCompanion = Instantiate(characterPrefab).GetComponent<Companion>();
                        instantiatedCompanion.SetCharacterData(knightData);
                        
                        // Add the instantiated companion to the party
                        currentParty.Add(instantiatedCompanion);
                        Debug.Log("Added to party: " + knightData.heroID);
                    }
                    else
                    {
                        Debug.LogError("Character prefab not found for heroID: " + knightData.heroID);
                    }
                }
                else
                {
                    Debug.Log("Companion already in party: " + knightData.heroID);
                }
    }
    

    public void LoadStageData()
    {
        // Here you would dynamically re-link or recreate stages when loading a new scene
        foreach (StageData stageData in allStagesData)
        {
            // Instantiate or find the stage and assign data as needed
            // Example: Find the stage by name or ID, then configure it with data
        }
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

    public static event Action OnInventoryChanged;

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
        OnInventoryChanged?.Invoke();

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

    public void RemoveItem(Item item, int quantity)
{
    var foundItem = itemList.Find(i => i.itemID == item.itemID);
    if (foundItem != null)
    {
        foundItem.quantity -= quantity;
        if (foundItem.quantity <= 0)
        {
            itemList.Remove(foundItem); // Remove the item if quantity drops to zero
        }

        SaveInventory();
        OnInventoryChanged?.Invoke();
        inventory.UpdateInventoryUI();
        
    }
}


    public bool HasItem(Item item, int quantity)
{
    var foundItem = itemList.Find(i => i.itemID == item.itemID);
    return foundItem != null && foundItem.quantity >= quantity;
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
                OnInventoryChanged?.Invoke();

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
    
    /// <summary>Return how many of this item the player has.</summary>
public int GetItemCount(Item item)
{
    var invItem = itemList.Find(i => i.itemID == item.itemID);
    return invItem != null ? invItem.quantity : 0;
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
            case "TamedGoblin":
                companionObject = Instantiate(tamedGoblinPrefab);
                break;
                // Add cases for other companions
        }
        if (companionObject != null)
        {
            Companion companion = companionObject.GetComponent<Companion>();
            if (characterDataDictionary.TryGetValue(heroID, out CharacterData characterData))
            {
                companion.SetCharacterData(characterData);
            }
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

    /* removing stam

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
    */

    public void RestoreHealthAndEnergyForCompanion(Companion companion)
    {
       // companion.health = companion.maxHealth;
        //companion.energy = companion.maxEnergy;
    }

    public void RecoverCompanion()
    {
       // currentCompanion.health = currentCompanion.maxHealth;
        //currentCompanion.energy = currentCompanion.maxEnergy;
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

    // WILL BE PHASING THIS OUT ONCE REPLACED WITH SCRIPTABLE DATA OBJECTS
    public void SaveAllCompanionData()
    {
        foreach (var companion in companions)
        {
            companion?.SaveCharacterData();
            Debug.Log("In game manager saving: " + companion);
        }
    }
    //THIS WILL BE THE ONLY SAVE METHOD
    private void SaveAllCharacterData()
    {
        foreach (var data in allCharacterData)
        {
            data.SaveData();
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

    public int GetBattleStepCost(StageData stage)
    {
        if (stage != null && stage.stepCost > 0) return stage.stepCost;
        return 200; // fallback default if unset
    }

    public bool TryPaySteps(int amount)
    {
        if (amount <= 0) return true;
        if (PlayerData.Instance.inGameSteps < amount) return false;
        PlayerData.Instance.inGameSteps -= amount;
        // If you persist steps somewhere else, call save here.
        return true;
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


    //Map Nodes Management
    public Node currentNode; // The player's current node
    public List<Node> allNodes; // List of all nodes in the scene

    public int CalculateTravelCost(Node fromNode, Node toNode)
    {
        return Node.CalculateCost(fromNode, toNode);
    }

    public void OnNodeSelected(Node selectedNode)
    {
        if (selectedNode != null && selectedNode != currentNode)
        {
            int cost = CalculateTravelCost(currentNode, selectedNode);
            bool paymentConfirmed = PromptPayment(cost);

            if (paymentConfirmed)
            {
                MoveToNode(selectedNode);
            }
        }
    }

    private bool PromptPayment(int cost)
    {
        // Implement your payment prompt logic here
        // Example: Show a UI dialog asking for confirmation to pay the cost
        // Return true if the payment is confirmed, otherwise return false

        Debug.Log($"Prompting payment of {cost} units to move to the selected node.");
        // Simulate payment confirmation for the example
        return true; // Replace this with actual confirmation logic
    }

    
    

    public void MoveToNode(Node destinationNode)
    {
        // Move player to the destination node
        currentNode = destinationNode;
        transform.position = destinationNode.position; // Update the player's position
        Debug.Log($"Moved to {destinationNode.nodeName}");
    }

    //STAGE REWORK METHODS
    
    public StageData GetStageData(string stageID)
{
    if (string.IsNullOrEmpty(stageID)) return null;
    if (stageDictionary.TryGetValue(stageID, out var sd)) return sd;
    Debug.LogError($"[GetStageData] No StageData for id: {stageID}");
    return null;
}

public void ClearAllStageUnlockedFlags()
{
    foreach (var sd in allStagesData) sd.isUnlocked = false;
}
public void StartDungeonRun(string startingStageID)
{
    InDungeonRun = true;
    runUnlockedStageIDs.Clear();
    ClearAllStageUnlockedFlags();

    var start = GetStageData(startingStageID);
    if (start == null) { Debug.LogError($"[Run] Missing StageData for {startingStageID}"); return; }

    currentStage = start;
    runUnlockedStageIDs.Add(start.stageID);
    start.isUnlocked = true;                     // for UI gating during this run
    CurrentBattleConfig = start.stageBattleConfig;

    Debug.Log($"[Run] Started at {start.stageID}");
    OnRunUnlocksChanged?.Invoke();
}

public void EndDungeonRun()
{
    InDungeonRun = false;
    runUnlockedStageIDs.Clear();
    ClearAllStageUnlockedFlags();
    currentStage = null;
    CurrentBattleConfig = null;

    Debug.Log("[Run] Ended");
    OnRunUnlocksChanged?.Invoke();
}
public void UnlockConnectedStagesForRun(StageData completedStage)
{
    if (!InDungeonRun) { Debug.LogWarning("[Run] Not in a run; unlock ignored."); return; }
    if (completedStage == null) { Debug.LogError("[Run] completedStage is null"); return; }

    var list = completedStage.connectedStageIDs;
    if (list == null || list.Count == 0)
    {
        Debug.LogWarning($"[Run] StageData {completedStage.stageID} has no connectedStageIDs.");
        return;
    }

    foreach (var nextId in list)
    {
        var next = GetStageData(nextId);
        if (next == null) { Debug.LogError($"[Run] Missing StageData for neighbor {nextId}"); continue; }

        if (runUnlockedStageIDs.Add(next.stageID))
        {
            next.isUnlocked = true; // UI flag for THIS RUN only
            Debug.Log($"[Run] Unlocked {next.stageID} from {completedStage.stageID}");
        }
    }

    OnRunUnlocksChanged?.Invoke();
}
public bool IsStageUnlockedForRun(string stageID)
{
    if (!InDungeonRun) return true; // outside a run, don't gate
    return runUnlockedStageIDs.Contains(stageID);
}

// GameManager.cs
public void EnterStageByID(string stageID)
{
    var next = GetStageData(stageID);
    if (next == null) return;

    // auto-start or gate the run (your existing logic)
    if (!InDungeonRun)
    {
        InDungeonRun = true;
        runUnlockedStageIDs.Clear();
        ClearAllStageUnlockedFlags();

        runUnlockedStageIDs.Add(next.stageID);
        next.isUnlocked = true;
        Debug.Log($"[Run] Auto-started at {next.stageID}");
        OnRunUnlocksChanged?.Invoke();
    }
    else if (!runUnlockedStageIDs.Contains(next.stageID))
    {
        Debug.LogWarning($"[Run] Stage {stageID} is not unlocked yet.");
        return;
    }

    currentStage = next;
    CurrentBattleConfig = next.stageBattleConfig;

    // IMPORTANT: load battle additively and make it the active scene
    StartCoroutine(LoadBattleAdditive(next.battleSceneName));
}

// GameManager.cs
private IEnumerator LoadBattleAdditive(string battleScene)
{
    var op = SceneManager.LoadSceneAsync(battleScene, LoadSceneMode.Additive);
    yield return op;

    var b = SceneManager.GetSceneByName(battleScene);
    SceneManager.SetActiveScene(b);

    // Turn OFF Town raycasters (your existing helper)
    SetUIRaycastsForScene("CharacterInfoPage", false);

    PromoteBattleCamera(b);       // <— new
    EnsureSingleEventSystem();    // <— new
}

private void PromoteBattleCamera(Scene battleScene)
{
    // Remove MainCamera tag from all other cameras
    foreach (var cam in Camera.allCameras)
        cam.tag = "Untagged";

    // Tag the first camera we find in the battle scene as MainCamera and bring it on top
    foreach (var root in battleScene.GetRootGameObjects())
    {
        var cam = root.GetComponentInChildren<Camera>(true);
        if (cam)
        {
            cam.tag = "MainCamera";
            cam.depth = 10;    // above Town camera
            cam.enabled = true;
            Debug.Log($"[BattleLoad] Promoted camera '{cam.name}' as MainCamera (scene {battleScene.name}).");
            break;
        }
    }
}


private void EnsureSingleEventSystem()
{
    var all = GameObject.FindObjectsOfType<EventSystem>(true);
    bool keptOne = false;
    foreach (var es in all)
    {
        bool enable = !keptOne;
        es.gameObject.SetActive(enable);
        if (enable) keptOne = true;
    }
    if (keptOne) return;

    // Create legacy module since you're on StandaloneInputModule
    new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
}

private void EnableOnlyEventSystemIn(Scene target)
{
    var all = GameObject.FindObjectsOfType<EventSystem>(true);
    bool anyEnabled = false;
    foreach (var es in all)
    {
        bool enable = es.gameObject.scene == target;
        es.gameObject.SetActive(enable);
        anyEnabled |= enable;
    }

    if (!anyEnabled)
    {
        var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        SceneManager.MoveGameObjectToScene(go, target);
    }
}

public static void Show(CanvasGroup cg, bool show)
{
    cg.alpha = show ? 1 : 0;
    cg.interactable   = show;
    cg.blocksRaycasts = show;
}





public void GetNextStage(int nextIndex)
{
    if (currentStage == null)
    {
        Debug.LogError("[Nav] currentStage is null"); 
        return;
    }
    var list = currentStage.connectedStageIDs;
    if (list == null || nextIndex < 0 || nextIndex >= list.Count)
    {
        Debug.LogWarning("[Nav] Invalid stage transition index.");
        return;
    }

    string nextId = list[nextIndex];
    EnterStageByID(nextId); // <- this now handles auto-run start + gating
}

// GameManager.cs
public void SetUIRaycastsForScene(string sceneName, bool enable)
{
    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
        var s = SceneManager.GetSceneAt(i);
        if (!s.IsValid() || s.name != sceneName) continue;

        foreach (var root in s.GetRootGameObjects())
        {
            foreach (var gr in root.GetComponentsInChildren<UnityEngine.UI.GraphicRaycaster>(true))
                gr.enabled = enable;

            foreach (var cg in root.GetComponentsInChildren<CanvasGroup>(true))
                cg.blocksRaycasts = enable;
        }
    }
}

public void ReturnToTownFromBattle()
{
    StartCoroutine(ReturnTownRoutine());
}

private IEnumerator ReturnTownRoutine()
{
    var battle = SceneManager.GetActiveScene();
    var town   = SceneManager.GetSceneByName("CharacterInfoPage");
    if (town.IsValid()) SceneManager.SetActiveScene(town);

    SetUIRaycastsForScene(town.name, true);
    SetCanvasGroupInteractable(town, true);   // <-- add this

    PromoteSceneCamera(town, 0);
    EnableOnlyEventSystemIn(town);

    Cursor.lockState = CursorLockMode.None;   // safety
    Cursor.visible   = true;
    Time.timeScale   = 1f;

    yield return SceneManager.UnloadSceneAsync(battle);
    Debug.Log("[BattleExit] Back in town.");
}


private void PromoteSceneCamera(Scene target, float depth)
{
    // clear MainCamera tag everywhere
    foreach (var cam in Camera.allCameras) cam.tag = "Untagged";

    // tag Town camera as MainCamera and enable it
    foreach (var root in target.GetRootGameObjects())
    {
        var cam = root.GetComponentInChildren<Camera>(true);
        if (!cam) continue;
        cam.tag = "MainCamera";
        cam.depth = depth;
        cam.enabled = true;
        Debug.Log($"[Camera] Promoted '{cam.name}' as MainCamera for scene {target.name}");
        break;
    }
}




private void SetCanvasGroupInteractable(Scene scene, bool enable)
{
    foreach (var root in scene.GetRootGameObjects())
    {
        foreach (var cg in root.GetComponentsInChildren<CanvasGroup>(true))
        {
            cg.interactable   = enable;  // <-- makes Buttons/etc. respond again
            cg.blocksRaycasts = enable;  // you already set this elsewhere; keep in sync here
        }
    }
}



}
