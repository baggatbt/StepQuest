using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    
    // References to stat UI Text elements
    public TextMeshProUGUI levelText,atkText, hpText, defText, expText, spdText; 

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
         if (stepsText != null) stepsText.text =  PlayerData.Instance.stepTokens.ToString();  
         if (goldText != null) goldText.text =  PlayerData.Instance.gold.ToString();  
         UpdateTimerDisplay();
         UpdateKnightRecoveryTimerDisplay();
    }
    private void UpdateUI()
    {  
       

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
            GameObject buttonObject = Instantiate(companionButtonPrefab, companionButtonContainer.transform); // Use the container as parent
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = companion.heroID;
            Button btn = buttonObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate { OnCompanionSelected(companion); });
        }
    }
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
        Destroy(child.gameObject);
    }
    Debug.Log("[MainMenuUIManager] Cleared existing inventory items.");

    int itemIndex = 0;
    // Loop through the max number of inventory slots
    for (int i = 0; i < GameManager.Instance.maxInventorySlots; i++) {
        GameObject itemObject = Instantiate(inventoryItemPrefab, inventoryItemContainer.transform); // Use the container as parent

        // Check if there's an item for this slot
        if (i < GameManager.Instance.itemList.Count) {
            Item item = GameManager.Instance.itemList[i];
            Debug.Log($"[MainMenuUIManager] Setting up UI for item: {item.itemName}");

            TextMeshProUGUI itemText = itemObject.GetComponentInChildren<TextMeshProUGUI>();
            if (itemText != null) {
                itemText.text = item.itemName;
                Debug.Log("[MainMenuUIManager] Set item name text.");
            } else {
                Debug.LogError("[MainMenuUIManager] TextMeshProUGUI component not found on item prefab.");
            }

            Image itemIcon = itemObject.GetComponentInChildren<Image>();
            if (itemIcon != null && item.itemIcon != null) {
                itemIcon.sprite = item.itemIcon;
                Debug.Log("[MainMenuUIManager] Set item icon.");
            } else {
                itemIcon.enabled = false; // Disable the icon if there's no item
                Debug.LogWarning("[MainMenuUIManager] No item icon set or Image component not found.");
            }
        } else {
            // For slots without an item, you might want to clear or disable elements
            TextMeshProUGUI itemText = itemObject.GetComponentInChildren<TextMeshProUGUI>();
            if (itemText != null) {
                itemText.text = ""; // Clear the text
            }
            Image itemIcon = itemObject.GetComponentInChildren<Image>();
            if (itemIcon != null) {
                itemIcon.enabled = false; // Optionally hide the icon for empty slots
            }
        }
    }
    Debug.Log("[MainMenuUIManager] Finished populating inventory list.");
}

// Placeholder for OnItemSelected - make sure to implement this according to your game's logic
void OnItemSelected(Item item) {
    Debug.Log($"[MainMenuUIManager] Selected item: {item.itemName}");
    // Implement what should happen when an item is selected
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
    public GameObject vitUpgradeBar;
    public GameObject spdUpgradeBar;

   public void OnCompanionSelected(Companion companion)
{
    GameManager.Instance.currentCompanion = companion;

    // Update upgrade bars based on the selected companion's stat progress
    atkUpgradeBar.GetComponent<UpgradeBar>().SetInitialProgress(companion.statUpgradeProgress["atk"]);
    vitUpgradeBar.GetComponent<UpgradeBar>().SetInitialProgress(companion.statUpgradeProgress["vit"]);
    spdUpgradeBar.GetComponent<UpgradeBar>().SetInitialProgress(companion.statUpgradeProgress["spd"]);

    // Update companion's stats display
    UpdateCompanionStatsDisplay(companion);

    // Make sure the companion stats panel is visible
    companionStatsPanel.SetActive(true);
}

private void UpdateCompanionStatsDisplay(Companion companion)
{
    levelText.text = "Lv: " + companion.heroLevel.ToString();
    atkText.text = "ATK: " + companion.attackPower.ToString();
    hpText.text = "HP: " + companion.health.ToString();
    defText.text = "VIT: " + companion.defensePower.ToString();
    expText.text = "EXP: " + companion.heroExp.ToString() + " / " + companion.ExpToNextLevel(companion.heroLevel);
    spdText.text = "SPD: " + companion.speed.ToString();
}

    public void IncreaseCompanionStat(Companion companion, string statType)
{
    // Increase the stat based on the type
    switch (statType)
    {
        case "atk":
            companion.attackPower += 1; // Or any logic for stat increase
            atkText.text = "ATK: " + companion.attackPower.ToString(); // Update UI
            break;
        case "vit":
            companion.defensePower += 1; // Adjust accordingly
            defText.text = "VIT: " + companion.defensePower.ToString(); // Update UI
            break;
        case "spd":
            companion.speed += 1; // Adjust accordingly
            spdText.text = "SPD: " + companion.speed.ToString(); // Update UI
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
