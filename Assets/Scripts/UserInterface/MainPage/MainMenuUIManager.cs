using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
public class MainMenuUIManager : MonoBehaviour
{
    public GameObject knightSkillPanel;
    public GameObject wizardSkillPanel;
    public GameObject archerSkillPanel;
    public GameObject adventurerSkillPanel;
    public GameObject forestTownPanel;
    public GameObject missionGUI;
    public GameObject companionGUI;
    public GameObject characterGUI;
    public GameObject overworldMapGUI;
    public GameObject knightSkillListView;
    public TextMeshProUGUI stepsText;
    public TextMeshProUGUI goldText;
    public GameObject companionButtonPrefab;
    public GameObject companionListPanel;
    public GameObject companionStatsPanel;
    public CircleShrinkAndCheck circleShrinkAndCheck;
    public GameObject consumablesPanel;  // Assign in inspector
    public Transform consumableListContent;  // Assign the content transform of your scroll view in the consumables panel

    // References to stat UI Text elements
    public TextMeshProUGUI levelText,atkText, hpText, spText, expText, spdText; 

    //KNIGHT 
    //public Knight knight;
    

    //ARCHER
    //public Archer archer;
    
    
  

    private void Start()
    {
       // knight = GameManager.Instance.knight;
       // archer = GameManager.Instance.archer;
       UpdateTimerDisplay(); // Initial display update
        UpdateUI();
    }
    private void Update()
    {
         if (stepsText != null) stepsText.text =  PlayerData.Instance.inGameSteps.ToString();  
         if (goldText != null) goldText.text =  PlayerData.Instance.gold.ToString();  
         UpdateTimerDisplay();
         UpdateKnightRecoveryTimerDisplay();
    }
    private void UpdateUI()
    {  
       

    }

    public void ToggleConsumablesPanel()
    {
        bool isActive = consumablesPanel.activeSelf;
        consumablesPanel.SetActive(!isActive);
        if (!isActive)
            PopulateConsumablesList();  // Populate list when the panel is opened
    }

    void PopulateConsumablesList()
    {
        // Clear existing entries
        foreach (Transform child in consumableListContent)
        {
            Destroy(child.gameObject);
        }

        // Populate the list with consumable items
        foreach (Item item in GameManager.Instance.itemList)
        {
            if (item.itemType == ItemType.Consumable)  // Check if it's a consumable
            {
                GameObject itemGO = Instantiate(inventoryItemPrefab, consumableListContent);
                itemGO.GetComponentInChildren<Image>().sprite = item.itemIcon;  // Assuming prefab structure
                itemGO.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.itemName} x{item.quantity}";

                Button useButton = itemGO.GetComponentInChildren<Button>(); // Assuming a Button exists in the prefab
                useButton.gameObject.SetActive(true);
                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(() => UseItem(item));
            }
        }
    }

    void UseItem(Item item)
    {
        // Assuming the consumable reduces quantity and might have other effects
        Debug.Log("Using item: " + item.itemName);
        ConsumableItem consumableItem = item as ConsumableItem;
        if (consumableItem != null)
        {
            consumableItem.Consume(GameManager.Instance.currentCompanion); // Just an example usage
            // Update UI or handle the item quantity decrease
            PopulateConsumablesList(); // Refresh list after using an item
            UpdateCompanionStatsDisplay(GameManager.Instance.currentCompanion);
        }
    }
  public void PopulateCompanionList()
{
    GameObject companionButtonContainer = GameObject.Find("Companion Button Container"); // Find or reference directly

    // Clear existing buttons to avoid duplicates
    foreach (Transform child in companionButtonContainer.transform)
    {
        Destroy(child.gameObject);
    }

    HashSet<string> addedCompanions = new HashSet<string>(); // To track added companions

    foreach (Companion companion in GameManager.Instance.companions)
    {
        if (companion.isUnlocked && addedCompanions.Add(companion.heroID)) // Checks if heroID is not already added
        {
            // Instantiate the button within the container
            GameObject buttonObject = Instantiate(companionButtonPrefab, companionButtonContainer.transform);

            // Find components
            Image heroIconImage = buttonObject.transform.Find("Background/HeroIconBorder/HeroIcon")?.GetComponent<Image>();
            Slider healthBarSlider = buttonObject.transform.Find("HealthBar").GetComponent<Slider>();
            Slider expBarSlider = buttonObject.transform.Find("ExpBar").GetComponent<Slider>();
            Text healthBarText = buttonObject.transform.Find("HealthBar/HealthText").GetComponent<Text>(); 
            Text xpBarText = buttonObject.transform.Find("ExpBar/ExpText").GetComponent<Text>(); 

            // Set the hero's icon
            if (heroIconImage != null && companion.heroIcon != null)
            {
                heroIconImage.sprite = companion.heroIcon;
            }
             
            healthBarSlider.maxValue = companion.maxHealth;
            healthBarSlider.value = companion.health; 

            expBarSlider.maxValue = companion.ExpToNextLevel(companion.heroLevel);
            expBarSlider.value = companion.heroExp;

            // Set the text for health and experience
            if (healthBarText != null)
            {
                healthBarText.text = $"{companion.health} / {companion.maxHealth}";
            }

            if (xpBarText != null)
            {
                xpBarText.text = $"{companion.heroExp} / {companion.ExpToNextLevel(companion.heroLevel)}";
            }
            else
            {
                Debug.LogError("One or more components were not found on the button prefab.");
            }

            // Setup button to select the companion when clicked
            Button btn = buttonObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate { OnCompanionSelected(companion); });
        }
    }
}



   

    public CanvasGroup uiCanvasGroup;
    

public void HideTilemap(TilemapRenderer tilemapRenderer)
{
   // uiCanvasGroup.alpha = 0; // Hide UI
    tilemapRenderer.enabled = false; // Hide Tilemap
}

public void ShowTilemap(TilemapRenderer tilemapRenderer)
{
   // uiCanvasGroup.alpha = 1; // Show UI
    tilemapRenderer.enabled = true; // Show Tilemap
}
   

   

public GameObject inventoryItemPrefab; // Assign this prefab in the Inspector


public void PopulateInventoryList() {
    Debug.Log("[MainMenuUIManager] Starting to populate inventory list...");
    
    GameObject inventoryItemContainer = GameObject.Find("Inventory Item Container");
    if (inventoryItemContainer == null) {
        Debug.LogError("[MainMenuUIManager] Inventory Item Container not found.");
        return;
    } else {
        Debug.Log("[MainMenuUIManager] Found Inventory Item Container.");
    }

    // Clear existing items to avoid duplicates
    foreach (Transform child in inventoryItemContainer.transform) {
        Debug.Log("[MainMenuUIManager] Destroying child: " + child.gameObject.name);
        Destroy(child.gameObject);
    }
    Debug.Log("[MainMenuUIManager] Cleared existing inventory items.");

    // Loop through all items in the GameManager's inventory list
    foreach (Item item in GameManager.Instance.itemList) {
        Debug.Log("[MainMenuUIManager] Creating inventory item slot for: " + item.itemName);
        GameObject itemSlotObject = Instantiate(inventoryItemPrefab, inventoryItemContainer.transform); // Use the container as parent

        if (itemSlotObject == null) {
            Debug.LogError("[MainMenuUIManager] Failed to instantiate inventory item slot prefab.");
            continue;
        }

        // Attempt to find and set the item icon
        Image itemIconImage = itemSlotObject.transform.Find("ItemContainer").GetComponent<Image>();
        if (itemIconImage == null) {
            Debug.LogError("[MainMenuUIManager] Failed to find Image component on ItemContainer.");
            continue;
        }

        // Set up the item icon
        if (item.itemIcon != null) {
            itemIconImage.sprite = item.itemIcon;
            itemIconImage.enabled = true;
            Debug.Log("[MainMenuUIManager] Set item icon for: " + item.itemName);
        } else {
            itemIconImage.enabled = false; // No icon for this item, so disable the image
            Debug.Log("[MainMenuUIManager] No icon found for item: " + item.itemName + "; disabling image component.");
        }

        // Attempt to find and set the item count text
        TextMeshProUGUI itemCountText = itemSlotObject.transform.Find("ItemCountText").GetComponent<TextMeshProUGUI>();
        if (itemCountText == null) {
            Debug.LogError("[MainMenuUIManager] Failed to find TextMeshProUGUI component on ItemCountText.");
            continue;
        }

        // Set up the item count text
        itemCountText.text = item.quantity.ToString();
        Debug.Log("[MainMenuUIManager] Set item quantity for: " + item.itemName);

    }
    Debug.Log("[MainMenuUIManager] Finished populating inventory list.");
}



    public void StartCircleShrink()
    {
        if (circleShrinkAndCheck != null)
        {
            // This would be your trigger to start the shrinking, replacing the spacebar press in the CircleShrinkAndCheck script
            circleShrinkAndCheck.isShrinking = true;
        }
        else
        {
            Debug.LogError("CircleShrinkAndCheck script not assigned in GameManager.");
        }
    }


     public void GoToNextStage()
    {
        
        if (GameManager.Instance.currentStageIndex >= GameManager.Instance.allStages.Count)
        {
            Debug.Log("The game is over, you win");
        }
        
        // Set the current battle configuration to the next stage
        GameManager.Instance.CurrentBattleConfig = GameManager.Instance.allStages[GameManager.Instance.currentStageIndex];

        // Load the battle scene with the new configuration
        SceneManager.LoadScene("TestPortraitBattle");
    }

    
    public GameObject atkUpgradeBar;
    public GameObject hpUpgradeBar;
    public GameObject spdUpgradeBar;
    public GameObject spUpgradeBar;

   public void OnCompanionSelected(Companion companion)
{
    GameManager.Instance.currentCompanion = companion;

    // Setup and subscribe to events for each upgrade bar
    SetupUpgradeBar(atkUpgradeBar, companion.statUpgradeProgress["atk"], () => IncreaseCompanionStat(companion, "atk"));
    SetupUpgradeBar(hpUpgradeBar, companion.statUpgradeProgress["hp"], () => IncreaseCompanionStat(companion, "hp"));
    SetupUpgradeBar(spdUpgradeBar, companion.statUpgradeProgress["spd"], () => IncreaseCompanionStat(companion, "spd"));
    SetupUpgradeBar(spUpgradeBar, companion.statUpgradeProgress["sp"], () => IncreaseCompanionStat(companion, "sp"));

    // Update companion's stats display and make sure the stats panel is visible
    UpdateCompanionStatsDisplay(companion);
    companionStatsPanel.SetActive(true);

    // Find and configure the HealButton
    GameObject healButton = GameObject.Find("HealButton");
    if (healButton != null)
    {
        Button btn = healButton.GetComponent<Button>();
        if (btn != null)
        {
            // Remove all listeners to avoid stacking if the button is reused
            btn.onClick.RemoveAllListeners();
            // Add a listener to call RecoverForSteps when clicked
            btn.onClick.AddListener(GameManager.Instance.RecoverForSteps);
        }
        else
        {
            Debug.LogError("HealButton does not have a Button component attached.");
        }
    }
    else
    {
        Debug.LogError("HealButton GameObject not found in the scene.");
    }
}

private void SetupUpgradeBar(GameObject upgradeBarGO, int initialProgress, Action onMaxLevelReached)
{
    if (upgradeBarGO == null)
    {
        Debug.LogError("Upgrade bar GameObject is null.");
        return;
    }

    var upgradeBar = upgradeBarGO.GetComponent<UpgradeBar>();
    if (upgradeBar == null)
    {
        Debug.LogError("UpgradeBar component is not found on the GameObject.");
        return;
    }

    upgradeBar.SetInitialProgress(initialProgress);
    upgradeBar.OnMaxLevelReached -= onMaxLevelReached;
    upgradeBar.OnMaxLevelReached += onMaxLevelReached;
}



private void UpdateCompanionStatsDisplay(Companion companion)
{
    levelText.text = "Lv: " + companion.heroLevel.ToString();
    atkText.text = "ATK: " + companion.attackPower.ToString();
    hpText.text = "HP: " + companion.health + " / " + companion.maxHealth.ToString();
    spText.text = "SP: " + companion.maxEnergy.ToString();
    expText.text = "EXP: " + companion.heroExp.ToString() + " / " + companion.ExpToNextLevel(companion.heroLevel);
    spdText.text = "SPD: " + companion.speed.ToString();
}

    public void IncreaseCompanionStat(Companion companion, string statType)
{
    // Increase the stat based on the type
    switch (statType)
    {
        case "atk":
            companion.attackPower += companion.atkGrowth;
            UpdateCompanionStatsDisplay(companion);
            atkText.text = "ATK: " + companion.attackPower.ToString(); 
            break;
        case "vit":
            companion.maxHealth +=  companion.healthGrowth;
            UpdateCompanionStatsDisplay(companion);
            hpText.text = "HP: " + companion.maxHealth.ToString(); 
            break;
        case "spd":
            companion.speed +=  companion.energyGrowth;
            UpdateCompanionStatsDisplay(companion);
            spdText.text = "SPD: " + companion.speed.ToString();
            break;
        case "sp":    
            companion.maxEnergy +=  companion.energyGrowth;
            UpdateCompanionStatsDisplay(companion);
            spText.text = "SP: " + companion.maxEnergy.ToString();
            break;

    }
}
   
    
    public CanvasGroup currentPanel;
    public CanvasGroup ForestMapCanvasGroup;

   public void SwitchPanel(CanvasGroup newPanel)
    {
        // Hide the current panel, if it exists
        if (currentPanel != null)
        {
            currentPanel.alpha = 0;
            currentPanel.interactable = false;
            currentPanel.blocksRaycasts = false;
        }

        // Show the new panel
        newPanel.alpha = 1;
        newPanel.interactable = true;
        newPanel.blocksRaycasts = true;

        // Update the current panel reference to the new panel
        currentPanel = newPanel;
    }

   public void TogglePanel(GameObject panel)
{
    panel.SetActive(!panel.activeSelf);
    companionStatsPanel.SetActive(false);
}

//TIMER MANAGEMENT
        public TMP_Text timerText; // Assign in the inspector
        public float timerDuration = 120; // Duration in seconds, adjust as needed

        private string timerId = "upgradeTimer"; // Unique ID for the timer
        public TMP_Text knightRecoveryText;
        private string knightRecoveryTimerId = "knightRecoveryTimer"; // Unique ID for the knight's recovery timer


       
        

        // Method to start the timer
        public void StartTimer(string timerId, float durationInSeconds)
        {
            TimerManager.Instance.SetTimer(timerId, durationInSeconds);
        }

        // Method to update the timer display
        void UpdateTimerDisplay()
        {
            TimeSpan remainingTime = TimerManager.Instance.GetRemainingTime(timerId);
            if (remainingTime > TimeSpan.Zero)
            {
                timerText.text = string.Format("{0:D2}:{1:D2}", remainingTime.Minutes, remainingTime.Seconds);
            }
            else
            {
                timerText.text = "00:00";
            }
        }

        void OnEnable() {
            TimerManager.Instance.OnTimerCompleted += HandleTimerCompletion;
        }

        void OnDisable() {
            TimerManager.Instance.OnTimerCompleted -= HandleTimerCompletion;
        }

        private void HandleTimerCompletion(string timerId) {
            // Check if the completed timer is the one we're interested in
            if (timerId == "knightRecoveryTimer") { 
                GameManager.Instance.knight.stamina += 1; // Increase stamina
                //update UI or do any other necessary actions
            }
        }

        // Example method call to start the knight's recovery timer
        public void StartKnightRecoveryTimer()
        {
            float knightRecoveryDuration = 600; // For example, 120 seconds for recovery
            StartTimer(knightRecoveryTimerId, knightRecoveryDuration);
        }

        void UpdateKnightRecoveryTimerDisplay() 
        {
            TimeSpan remainingTime = TimerManager.Instance.GetRemainingTime(knightRecoveryTimerId);
            if (remainingTime > TimeSpan.Zero) {
                knightRecoveryText.text = string.Format("{0:D2}:{1:D2}", remainingTime.Minutes, remainingTime.Seconds);
            } else {
                knightRecoveryText.text = "00:00";
                // Additional actions to take when the knight's recovery timer finishes could also be triggered here if needed
            }
        }


        


    











    //OLD NEED TO REDO WITH NEW TECHNIQUE 
        public void openMissionInterface()
        {
            missionGUI.SetActive(true);
        }

        public void closeMissionInterface()
        {
            missionGUI.SetActive(false);
        }

    public void openCompanionInterface()
    {
        companionGUI.SetActive(true);
    }

    public void closeCompanionInterface()
    {
        companionGUI.SetActive(false);
    }

    public void openCharacterInterface()
    {
        characterGUI.SetActive(true);

        PopulateCompanionList();
    }
    

    public void openInventory()
    {
        PopulateInventoryList();
        
    }

    public void closeCharacterInterface()
    {
        characterGUI.SetActive(false);
    }

    public void openOverworldMapInterface()
    {
        overworldMapGUI.SetActive(true);
    }

    public void closeOverworldMapInterface()
    {
        overworldMapGUI.SetActive(false);
        Debug.Log("ACTIVATED");
    }

    public void openForestTownPanel()
    {
        forestTownPanel.SetActive(true);
    }

    public void closeForestTownPanel()
    {
        forestTownPanel.SetActive(false);
    }

    public void openKnightSkillTree()
    {
        
        knightSkillListView.SetActive(true);
    }

    
    public void openKnightSkillList()
    {
        knightSkillListView.SetActive(true);
    }
    public void closeKnightSkillList()
    {
        knightSkillListView.SetActive(false);
    }
   
}
