using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private int currentStageIndex;
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
            ShowCharacterSelectionPanel(); 
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
        // Code to run every time a new scene is loaded
            companions.Clear();
            CreateAndRegisterKnight();
            CreateAndRegisterArcher();
            CreateAndRegisterWizard();
    }

    public void ShowCharacterSelectionPanel()
{
    Debug.Log("ShowingCharSelectPanel");
    characterSelectionPanel.SetActive(true);
    PlayerData.Instance.firstTimeLogin = false;
    PlayerData.Instance.SavePlayerData(); 
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

    private void CreateAndRegisterKnight()
{
    if (knight == null) // Check if the knight is already created
    {
        GameObject knightObject = new GameObject("Knight");
        knight = knightObject.AddComponent<Knight>();
        knight.heroID = "Knight";
        RegisterCompanion(knight);
        knight.LoadCharacterData();
    }
}

private void CreateAndRegisterArcher()
{
    if (archer == null) // Check if the archer is already created
    {
        GameObject archerObject = new GameObject("Archer");
        archer = archerObject.AddComponent<Archer>();
        archer.heroID = "Archer";
        RegisterCompanion(archer);
        archer.LoadCharacterData();
    }
}

private void CreateAndRegisterWizard()
{
    if (wizard == null) // Check if the wizard is already created
    {
        GameObject wizardObject = new GameObject("Wizard");
        wizard = wizardObject.AddComponent<Wizard>();
        wizard.heroID = "Wizard";
        RegisterCompanion(wizard);
        wizard.LoadCharacterData();
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
        Debug.Log("Registered companion: " + companion.heroID);
        Debug.Log("Amount of companions: " + companions.Count);
    }
    else
    {
        Debug.LogWarning("Trying to register a companion that is already registered: " + companion.heroID);
    }
}


    // Call this method to save the data of all companions
    public void SaveAllCompanionData()
    {
        foreach (var companion in companions)
        {
            if (companion != null)
            {
                Debug.Log("Saving " + companion);
             companion.SaveCharacterData();
            }
        }
    }

    
    void OnDestroy()
    {
    // unsubscribe to avoid memory leaks
    SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnApplicationQuit()
    {
       SaveAllCompanionData();
    }

    private void OnApplicationPause()
    {
        SaveAllCompanionData();
    }

    
   
    

}
