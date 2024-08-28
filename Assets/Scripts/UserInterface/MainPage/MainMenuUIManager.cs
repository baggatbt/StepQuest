using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Linq;
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
    public TextMeshProUGUI levelText,atkText, hpText, spText, expText, spdText, defText; 

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
         if (stepsText != null) stepsText.text = PlayerData.Instance.inGameSteps.ToString();  
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
  
    //public GameObject heroSpriteIconButton;
    public GameObject heroSelectPanel;

    public void PopulateCompanionList()
{
    GameObject companionButtonContainer = GameObject.Find("Companion Button Container");
    if (companionButtonContainer == null)
    {
        Debug.LogError("Companion Button Container not found.");
        return;
    }

    // Clear existing buttons to avoid duplicates
    foreach (Transform child in companionButtonContainer.transform)
    {
        Destroy(child.gameObject);
    }

    HashSet<string> addedCompanions = new HashSet<string>(); // To track added companions

    foreach (CharacterData characterData in GameManager.Instance.allCharacterData)
    {
        if (characterData == null)
        {
            Debug.LogError("Found a null characterData in the allCharacterData list.");
            continue;
        }

        Debug.Log($"Processing character: {characterData.heroID}, isUnlocked: {characterData.isUnlocked}");

        if (characterData.isUnlocked && addedCompanions.Add(characterData.heroID)) // Checks if heroID is not already added
        {
            // Instantiate the button within the container
            GameObject buttonObject = Instantiate(companionButtonPrefab, companionButtonContainer.transform);
            Debug.Log("Button object instantiated for character: " + characterData.heroID);

            // Find components
            Image heroIconImage = buttonObject.transform.Find("Background/HeroIconBorder/HeroIcon")?.GetComponent<Image>();
            Slider healthBarSlider = buttonObject.transform.Find("HealthBar")?.GetComponent<Slider>();
            Slider expBarSlider = buttonObject.transform.Find("ExpBar")?.GetComponent<Slider>();
            Slider staBarSlider = buttonObject.transform.Find("StaminaBar")?.GetComponent<Slider>();
            Text healthBarText = buttonObject.transform.Find("HealthBar/HealthText")?.GetComponent<Text>();
            Text xpBarText = buttonObject.transform.Find("ExpBar/ExpText")?.GetComponent<Text>();
            Text staBarText = buttonObject.transform.Find("StaminaBar/StaText")?.GetComponent<Text>();

            // Check if components are found
            if (heroIconImage == null || healthBarSlider == null || expBarSlider == null || staBarSlider == null ||
                healthBarText == null || xpBarText == null || staBarText == null)
            {
                Debug.LogError("One or more components were not found on the button prefab.");
                Destroy(buttonObject); // Cleanup if the button is incomplete
                continue;
            }

            // Set the hero's icon
            if (heroIconImage != null && characterData.heroIcon != null)
            {
                heroIconImage.sprite = characterData.heroIcon;
                Debug.Log("Hero icon set from characterData");
            }

            healthBarSlider.maxValue = characterData.maxHealth;
            healthBarSlider.value = characterData.health;

            

            staBarSlider.maxValue = characterData.maxStamina;
            staBarSlider.value = characterData.stamina;

            // Set the text for health and experience
            healthBarText.text = $"{characterData.health} / {characterData.maxHealth}";
           
            staBarText.text = $"{characterData.stamina} / {characterData.maxStamina}";

            // Setup button to select the companion when clicked
            Button btn = buttonObject.GetComponent<Button>();
            btn.onClick.AddListener(() => OpenCompanionDetails(characterData));

            Debug.Log($"Character {characterData.heroID} added to list.");
        }
    }

    Debug.Log("Companion list populated.");
}

private void OpenCompanionDetails(CharacterData characterData)
{
    GameManager.Instance.currentCompanionData = characterData;
    UpdateCompanionStatsDisplay(characterData);
    companionStatsPanel.SetActive(true);
    UpdateHeroImage(characterData);
  //  UpdateSkillButtonImages(characterData);
}

private void UpdateCompanionStatsDisplay(CharacterData characterData)
{
    // Update the stats display with data from characterData
    // Example:
    levelText.text = $"Lv: {characterData.heroLevel}";
    atkText.text = $"{characterData.attackPower}";
   hpText.text = $"{characterData.health}/{characterData.maxHealth}";
   spText.text = $"{characterData.energy}/{characterData.maxEnergy}";
   spdText.text = $"{characterData.speed}";
   defText.text = $"{characterData.defensePower}";
   expText.text = $"Exp:{characterData.heroExp}/{characterData.ExpToNextLevel(characterData.heroLevel)}";
    // Add other stat updates here
}

private void UpdateHeroImage(CharacterData characterData)
{
    // Find the GameObject with the name "HeroImage"
    GameObject heroImageObject = GameObject.Find("HeroImage");

    // Ensure the heroImageObject is not null and has an Image component
    if (heroImageObject != null)
    {
        // Get the Image component attached to the heroImageObject
        Image heroImage = heroImageObject.GetComponent<Image>();

        // Ensure the heroImage is not null
        if (heroImage != null)
        {
            // Set the sprite of the Image component to the character's fullHeroImage
         //   heroImage.sprite = characterData.fullHeroImage;
        }
        else
        {
            Debug.LogError("HeroImage GameObject does not have an Image component attached.");
        }
    }
    else
    {
        Debug.LogError("HeroImage GameObject not found in the scene.");
    }
}


private void OnCharacterDataSelected(CharacterData characterData)
{
    GameManager gameManager = GameManager.Instance;
    if (gameManager != null)
    {
        if (gameManager.currentParty.Count < 2) // Check if there is space in the party
        {
            if (!gameManager.currentParty.Any(companion => companion.characterData == characterData))
            {
                // Find the prefab associated with the characterData and instantiate the companion
                GameObject characterPrefab = gameManager.GetCharacterPrefab(characterData.heroID);
                if (characterPrefab != null)
                {
                    GameObject instantiatedObject = Instantiate(characterPrefab);
                    Companion instantiatedCompanion = instantiatedObject.GetComponent<Companion>();
                    
                    if (instantiatedCompanion != null)
                    {
                        instantiatedCompanion.characterData = characterData;
                        instantiatedCompanion.InitializeCharacterStats();

                        gameManager.currentParty.Add(instantiatedCompanion);
                        Debug.Log("Added to party: " + characterData.heroID);
                        ShowPartyMembers(); // Update UI to reflect changes
                    }
                    else
                    {
                        Debug.LogError("Instantiated object does not have a Companion component.");
                        Destroy(instantiatedObject); // Clean up if instantiation failed
                    }
                }
                else
                {
                    Debug.LogError("Character prefab not found for heroID: " + characterData.heroID);
                }
            }
            else
            {
                Debug.Log("Companion already in party: " + characterData.heroID);
            }
        }
        else
        {
            Debug.LogError("Party is full. Cannot add more companions.");
        }
    }
    else
    {
        Debug.LogError("GameManager instance is null.");
    }
}




    public void SetActiveSelectPanel()
    {
        heroSelectPanel.SetActive(true);
        OpenSelectHeroList();
    }


    //For the dungeons
    [SerializeField] private GameObject heroSpriteIconButtonPrefab; // Drag your button prefab here in the inspector

    public void OpenSelectHeroList()
{
    Debug.Log("This button was clicked");
    GameObject companionButtonContainer = GameObject.Find("Companion Selection Container");
    if (!companionButtonContainer)
    {
        Debug.LogError("Companion Selection Container not found in the scene.");
        return;
    }

    // Clear existing buttons to avoid duplicates
    foreach (Transform child in companionButtonContainer.transform)
    {
        Destroy(child.gameObject);
    }

    HashSet<string> addedCompanions = new HashSet<string>(); // To track added companions to avoid duplicates

    // Iterate over the CharacterData list from GameManager
    foreach (CharacterData characterData in GameManager.Instance.allCharacterData)
    {
        if (characterData.isUnlocked && addedCompanions.Add(characterData.heroID)) // Check if unlocked and not already added
        {
            // Instantiate the button within the container
            GameObject buttonObject = Instantiate(heroSpriteIconButtonPrefab, companionButtonContainer.transform);

            // Setup the hero icon
            SetupHeroIcon(buttonObject, characterData);
        }
    }
}



    private void SetupHeroIcon(GameObject buttonObject, CharacterData characterData)
{
    Image heroIconImage = buttonObject.GetComponentInChildren<Image>();
    if (heroIconImage == null)
    {
        Debug.LogError("Image component not found in button prefab.");
        return;
    }

    if (characterData.heroIcon == null)
    {
        Debug.LogError("characterData.heroIcon is null for character: " + characterData.heroID);
        return;
    }

    heroIconImage.sprite = characterData.heroIcon;

    Button button = buttonObject.GetComponent<Button>();
    if (button != null)
    {
        button.onClick.AddListener(() => AddToParty(characterData));
    }
    else
    {
        Debug.LogError("Button component not found on the hero icon button prefab.");
    }
}


private void AddToParty(CharacterData characterData)
{
    GameManager gameManager = GameManager.Instance;
    if (gameManager != null)
    {
        if (gameManager.currentParty.Count < 2) // Check if there is space in the party
        {
            if (!gameManager.currentParty.Any(companion => companion.characterData == characterData))
            {
                // Find the prefab associated with the characterData and instantiate the companion
                GameObject characterPrefab = gameManager.GetCharacterPrefab(characterData.heroID);
                if (characterPrefab != null)
                {
                    Companion instantiatedCompanion = Instantiate(characterPrefab).GetComponent<Companion>();
                    instantiatedCompanion.SetCharacterData(characterData);

                    gameManager.currentParty.Add(instantiatedCompanion);
                    Debug.Log("Added to party: " + characterData.heroID);
                    ShowPartyMembers(); // Update UI to reflect changes
                    CheckPartyNotEmpty();
                }
                else
                {
                    Debug.LogError("Character prefab not found for heroID: " + characterData.heroID);
                }
            }
            else
            {
                Debug.Log("Companion already in party: " + characterData.heroID);
            }
        }
        else
        {
            Debug.LogError("Party is full. Cannot add more companions.");
        }
    }
    else
    {
        Debug.LogError("GameManager instance is null.");
    }
}

//Add more as needed
public GameObject dungeonSelectionPanel;
public Button forestDungeonButton;

 // Assuming you have a panel and a TextMeshProUGUI element assigned in the inspector
    public GameObject nodeInfoPanel;
    public TextMeshProUGUI nodeNameText;

    // Method to show the node information panel and update the text
    public void ShowNodeInfo(string nodeName)
    {
        nodeNameText.text = nodeName;
        nodeInfoPanel.SetActive(true);
    }

//ASSIGN THE DUNGEON PANEL TO CHECK PARTY 
public void OpenDungeonPanel()
{
    dungeonSelectionPanel.SetActive(true);
    CheckPartyNotEmpty();
}
public void ActivateDungeonButtons()
    {
        forestDungeonButton.interactable = true;
    }

    public void DeactivateDungeonButtons()
    {
        forestDungeonButton.interactable = false;
    }

private void CheckPartyNotEmpty()
{
    if (GameManager.Instance.currentParty.Count == 0)
    {
        DeactivateDungeonButtons();
    }
    else
    {
        ActivateDungeonButtons();
    }
}



public GameObject heroSlotOne;
public GameObject heroSlotTwo;
[SerializeField] private Sprite defaultHeroSprite; //The border they sit in

private void ShowPartyMembers()
{
    Image heroSlotOneImage = heroSlotOne.GetComponent<Image>();
    Image heroSlotTwoImage = heroSlotTwo.GetComponent<Image>();

    if (heroSlotOneImage == null || heroSlotTwoImage == null)
    {
        Debug.LogError("One or both hero slots do not have an Image component.");
        return;
    }

    // Reset slot images to default sprite
    heroSlotOneImage.sprite = defaultHeroSprite;
    heroSlotTwoImage.sprite = defaultHeroSprite;

    // Assign icons to slots based on current party members
    if (GameManager.Instance.currentParty.Count > 0)
    {
        heroSlotOneImage.sprite = GameManager.Instance.currentParty[0].heroIcon;
    }
    if (GameManager.Instance.currentParty.Count > 1)
    {
        heroSlotTwoImage.sprite = GameManager.Instance.currentParty[1].heroIcon;
    }
}




 public GameObject skillPanel;

public void OpenSkillPanel()
{   
   skillPanel.SetActive(true);
}

public void CloseSkillPanel()
{
    skillPanel.SetActive(false);
}


public GameObject restRecoverStaminaPanel;
public TextMeshProUGUI stepsToPayText;
public void openRestPanel()
{
    restRecoverStaminaPanel.SetActive(true);

    int stepsToPay = GameManager.Instance.CalculateStepCostForResting();

    stepsToPayText.text = ("Cost: " + stepsToPay.ToString() + " steps");  
}

public void RecoverStaminaUsingSteps()
{
    GameManager.Instance.RecoverAllForSteps();
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

    
     public void GoToBattle()
    {
        

        // Load the battle scene with the new configuration
        SceneManager.LoadScene("TestPortraitBattle");
        Debug.Log("Going to battle");
    }


   

   public void OnCompanionSelected(Companion companion)
{
    GameManager.Instance.currentCompanion = companion;
    companion.InitializeSkillsBasedOnLevel();


    // Update companion's stats display and make sure the stats panel is visible
    UpdateCompanionStatsDisplay(companion);
    companionStatsPanel.SetActive(true);

    // Find the GameObject with the name "HeroImage"
    GameObject heroImageObject = GameObject.Find("HeroImage");

    // Ensure the heroImageObject is not null and has an Image component
    if (heroImageObject != null)
    {
        // Get the Image component attached to the heroImageObject
        Image heroImage = heroImageObject.GetComponent<Image>();

        // Ensure the heroImage is not null
        if (heroImage != null)
        {
            // Set the sprite of the Image component to the companion's heroIcon
            heroImage.sprite = companion.heroIcon;
        }
        else
        {
            Debug.LogError("HeroImage GameObject does not have an Image component attached.");
        }
    }
    else
    {
        Debug.LogError("HeroImage GameObject not found in the scene.");
    }



    // Ensure the heroImageObject is not null and has an Image component
    if (heroImageObject != null)
    {
        // Get the Image component attached to the heroImageObject
        Image heroImage = heroImageObject.GetComponent<Image>();

        // Ensure the heroImage is not null
        if (heroImage != null)
        {
            // Set the sprite of the Image component to the companion's heroIcon
          //  heroImage.sprite = companion.fullHeroImage;
        }
        else
        {
            Debug.LogError("HeroImage GameObject does not have an Image component attached.");
        }
    }
    else
    {
        Debug.LogError("HeroImage GameObject not found in the scene.");
    }
    
    
    
    UpdateSkillButtonImages(companion);
}
    public GameObject skillInfoPanel;
    public void OnSkillButtonClick(Skill skill)
{
    Debug.Log("Button clicked for skill: " + skill.skillName);

    // Ensure the skillInfoPanel is active so the text update can be seen
    if (!skillInfoPanel.activeInHierarchy)
        skillInfoPanel.SetActive(true);

    // Find the TextMeshProUGUI component that should display the skill name
    TextMeshProUGUI skillNameText = skillInfoPanel.transform.Find("SkillNameText").GetComponent<TextMeshProUGUI>();
    TextMeshProUGUI skillDescriptionText = skillInfoPanel.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
    if (skillNameText != null)
    {
        // Set the skill name text
        skillNameText.text = skill.skillName;
    }
    if (skillDescriptionText != null)
    {
        // Set the skill name text
        skillDescriptionText.text = skill.description;
    }
    else
    {
        Debug.LogError("SkillNameText component not found or is not a TextMeshProUGUI");
    }
}


public void UpdateSkillButtonImages(Companion companion)
{
    // Update and set up the first skill button
    SetupSkillButton("SkillOneButton", companion.MainSkills[0], companion);

    
    SetupSkillButton("SkillTwoButton", companion.MainSkills[1], companion);

    
    SetupSkillButton("SkillThreeButton", companion.MainSkills[2], companion);


    SetupSkillButton("SkillFourButton", companion.MainSkills[3], companion);
}

private void SetupSkillButton(string buttonName, SkillType skillType, Companion companion)
{
    GameObject skillButtonObj = GameObject.Find(buttonName);
    if (skillButtonObj != null)
    {
        Image skillImage = skillButtonObj.GetComponent<Image>();
        Button skillButton = skillButtonObj.GetComponent<Button>();

        if (skillImage != null && skillButton != null)
        {
            Skill skill = companion.GetSkillInstance(skillType);
            if (skill != null)
            {
                skillImage.sprite = skill.iconImage;
                skillButton.onClick.RemoveAllListeners(); // Remove existing listeners to prevent multiple assignments
                skillButton.onClick.AddListener(() => OnSkillButtonClick(skill));
            }
            else
            {
                Debug.LogError("Skill instance is null for " + skillType);
            }
        }
        else
        {
            if (skillImage == null) Debug.LogError(buttonName + " does not have an Image component.");
            if (skillButton == null) Debug.LogError(buttonName + " does not have a Button component.");
        }
    }
    else
    {
        Debug.LogError(buttonName + " GameObject not found.");
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
    atkText.text = companion.attackPower.ToString();
    hpText.text = companion.maxHealth.ToString();
    spText.text = companion.maxEnergy.ToString();
    expText.text = "EXP: " + companion.heroExp.ToString() + " / " + companion.ExpToNextLevel(companion.heroLevel);
    Debug.Log(expText.text);
    spdText.text = companion.speed.ToString();
}

public void UnlockSkillOneForHero()
{
    GameManager.Instance.currentCompanionData.UnlockSkill(GameManager.Instance.currentCompanionData.skillOne);
    Debug.Log("added skill one");
}

public void UnlockSkillTwoForHero()
{
    GameManager.Instance.currentCompanionData.UnlockSkill(GameManager.Instance.currentCompanionData.skillTwo);
    Debug.Log("added skill two");
}
/*
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
*/
   
    
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
    //companionStatsPanel.SetActive(false);
}

    public void ToggleCanvasGroup(GameObject canvasGroupObject)
{
    // Check if the canvas group object exists
    if (canvasGroupObject != null)
    {
        // Get the CanvasGroup component
        CanvasGroup canvasGroup = canvasGroupObject.GetComponent<CanvasGroup>();

        // If the CanvasGroup component exists
        if (canvasGroup != null)
        {
            // Toggle visibility by toggling the alpha value
            canvasGroup.alpha = canvasGroup.alpha > 0 ? 0 : 1;

            // Toggle interactability and raycast blocking based on visibility
            canvasGroup.interactable = canvasGroup.alpha > 0;
            canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
        }
        else
        {
            Debug.LogError("CanvasGroup component not found on the GameObject: " + canvasGroupObject.name);
        }
    }
    else
    {
        Debug.LogError("CanvasGroup GameObject is null.");
    }
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
       // companionGUI.SetActive(true);
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

    public TextMeshProUGUI knightSkillPointText;

    public void openKnightSkillTree()
    {
        
        knightSkillListView.SetActive(true);
        knightSkillPointText.text = GameManager.Instance.currentCompanion.heroSkillPoints.ToString();
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
