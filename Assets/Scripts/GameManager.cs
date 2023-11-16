using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleConfig CurrentBattleConfig { get; set; } // Temporary storage for the battle config

    public int currentStageID = 0; //This will need to be replaced by loading data on start. for testing purposes setting to 0
    public List<Stage> stages;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes sure the GameManager persists between scenes
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockNextStage(int currentStageID)
{
    int nextStageID = currentStageID + 1;
    if (nextStageID < stages.Count)
    {
        stages[nextStageID].isUnlocked = true;
        stages[nextStageID].UpdateButtonColor(); // Update the button color

        currentStageID = nextStageID; // Update the current stage index
      //  SaveProgress(); // Save the new progress
    }
}
}
