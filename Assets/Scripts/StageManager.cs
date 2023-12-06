using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public List<Stage> allStages;

    private void Start()
    {
        InitializeStages();
    }

   private void InitializeStages()
{
    if (GameManager.Instance.UnlockedStageNames.Count == 0 && allStages.Count > 0)
    {
        // Assuming the first stage is always at index 0
        Stage firstStage = allStages[0];
        firstStage.isUnlocked = true;
        GameManager.Instance.UnlockedStageNames.Add(firstStage.stageID);
        firstStage.UpdateButtonColor();
    }

    // Continue initializing other stages
    foreach (Stage stage in allStages)
    {
        stage.isUnlocked = GameManager.Instance.UnlockedStageNames.Contains(stage.stageID);
        stage.UpdateButtonColor();
    }
}

   

    // Additional methods as needed for managing stages
}
