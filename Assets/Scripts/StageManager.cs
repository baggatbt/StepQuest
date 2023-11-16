using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<Stage> stages;
    

   

    public void UnlockNextStage(int completedStageID)
    {
        int nextStageID = completedStageID + 1;
        if (nextStageID < stages.Count)
        {
            stages[nextStageID].isUnlocked = true;
            stages[nextStageID].UpdateButtonColor(); 
        }
    }

    // Additional methods as needed for managing stages
}
