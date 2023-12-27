using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleConfig CurrentBattleConfig { get; set; }
    public HashSet<string> UnlockedStageNames = new HashSet<string>();
    public Character companion1;
    public Character companion2;
     public Character companion3;
    private List<Companion> companions = new List<Companion>();
    
    //Used in EnemyAttack() to weight enemy targets
    public float probabilityCompanion1 = 1f; // Default probability for companion1 to be attacked
    public float probabilityCompanion2 = 1f; // Default probability for companion2 to be attacked

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
         Application.targetFrameRate = 60;  // Set target frame rate to 60 FPS.
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
        PlayerPrefs.DeleteAll();
        Debug.Log("Player prefs deleted");
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
            companion.SaveCharacterData();
        }
    }

    
    private void OnApplicationQuit()
    {
        SaveAllCompanionData();
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
