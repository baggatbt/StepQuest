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
   

    public void UnlockNextStage(int completedStageID)
    {
        int stagesUnlocked = 0;
        int stagesComplete = completedStageID;
        if (stagesComplete > stagesUnlocked)
        {
            stages[stagesComplete].isUnlocked = true;
            stages[stagesComplete].UpdateButtonColor(); 
            
        }
    }
   

    // Additional methods as needed for managing stages
}
