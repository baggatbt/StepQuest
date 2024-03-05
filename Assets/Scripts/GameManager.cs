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
    public GameObject companionButtonPrefab;
    public GameObject companionListPanel;
    public GameObject companionStatsPanel;

    // References to stat UI Text elements
    public TextMeshProUGUI atkText, hpText, defText, staminaText, spdText; 

    private int currentStageIndex;
    private bool isUnlocked;
  //  public List<BattleConfig> allStages; // list is populated with all stages in order

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

    

    public void PopulateCompanionList()
{
    GameObject companionButtonContainer = GameObject.Find("Companion Button Container"); // Find or reference directly

    foreach (Companion companion in companions)
    {
        if (companion.isUnlocked)
        {
            GameObject buttonObject = Instantiate(companionButtonPrefab, companionButtonContainer.transform); // Use the container as parent
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = companion.heroID;
            Button btn = buttonObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate { OnCompanionSelected(companion); });
        }
    }
}


    void OnCompanionSelected(Companion companion)
    {
        // Assuming your Companion class has these fields or properties
        atkText.text = "ATK: " + companion.attackPower.ToString();
        hpText.text = "HP: " + companion.health.ToString();
        defText.text = "DEF: " + companion.defensePower.ToString();
        staminaText.text = "Stamina: " + companion.stamina.ToString();
        spdText.text = "SPD: " + companion.speed.ToString();
        
        // Make sure the stats panel is visible
        companionStatsPanel.SetActive(true);
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

/*
    public void GoToNextStage()
    {
        // Increment the stage index
        currentStageIndex++;
        
        
        if (currentStageIndex >= allStages.Count)
        {
            Debug.Log("The game is over, you win");
        }
        
        // Set the current battle configuration to the next stage
        CurrentBattleConfig = allStages[currentStageIndex];

        // Load the battle scene with the new configuration
        SceneManager.LoadScene("TestPortraitBattle");
    }
*/
    
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
            if (companion.heroID == heroID)
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
       SaveAllCompanionData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveAllCompanionData();
        }
    }

    
   
    

}
