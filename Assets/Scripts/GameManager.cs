using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleConfig CurrentBattleConfig { get; set; }
    public HashSet<string> UnlockedStageNames = new HashSet<string>();
    public Character companion1; //Battle Position
    public Character companion2; 
    public Character companion3;
    public int currentStageIndex;
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
            currentStageIndex = PlayerData.Instance.currentStageIndex; //Gets the current stage from PlayerData

            //Each companion has equal chance to be selected for attack, need to change so it uses the class's variable threat
            probabilityCompanion1 = 1f;
            probabilityCompanion2 = 1f;
            probabilityCompanion3 = 1f;
            CreateAndRegisterKnight();
            CreateAndRegisterArcher();
            Debug.Log(companion1);
           
        }
        else
        {
            Destroy(gameObject);
        }
         Application.targetFrameRate = 60;  // Set target frame rate to 60 FPS.
    }

    private void CreateAndRegisterKnight()
    {
        GameObject knightObject = new GameObject("Knight");
        knight = knightObject.AddComponent<Knight>();
        knight.heroID = "Knight"; // This line ensures the GameObject name is unique and avoids saves being overwritten
        RegisterCompanion(knight);
        knight.LoadCharacterData();
    }

    private void CreateAndRegisterArcher()
    {
        GameObject archerObject = new GameObject("Archer");
        archer = archerObject.AddComponent<Archer>();
        archer.heroID = "Archer"; // This line ensures the GameObject name is unique and avoids saves being overwritten
        RegisterCompanion(archer);
        archer.LoadCharacterData();
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

    public void GoToNextStage()
    {
        // Increment the stage index
        currentStageIndex++;
        
        // Make sure we don't go out of bounds
        if (currentStageIndex >= allStages.Count)
        {
            Debug.Log("The game is over, you win");
        }
        
        // Set the current battle configuration to the next stage
        CurrentBattleConfig = allStages[currentStageIndex];

        // Load the battle scene with the new configuration
        SceneManager.LoadScene("TestPortraitBattle");
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

    
    private void OnApplicationQuit()
    {
      //OUT FOR EASY DELETION  SaveAllCompanionData();
    }

    private void OnApplicationPause()
    {
        SaveAllCompanionData();
    }

    public void GainExp(int expGained)
    {
        foreach (Companion companion in companions)
        {
            companion.heroExp += expGained;
            Debug.Log(companion + "exp gained" + expGained);
            companion.LevelUp();
            SaveAllCompanionData();
        }
    }

}
