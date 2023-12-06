using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<Stage> stages;
    
    private void Start()
    {
        UnlockNextStage(GameManager.Instance.currentStageID);
    }
    
    private void Update()
    {
        UnlockNextStage(GameManager.Instance.currentStageID);
    }

    public void UnlockNextStage(int completedStageID)
    {
        Stage completedStage = stages[completedStageID];
        foreach (Stage connectedStage in completedStage.connectedStages)
        {
            if (!connectedStage.isUnlocked)
            {
                connectedStage.isUnlocked = true;
                connectedStage.UpdateButtonColor();
            }
        }
    }
   

    // Additional methods as needed for managing stages
}
