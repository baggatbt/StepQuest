using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<Stage> stages;
    private int currentStageIndex = 0;

   

    public void UnlockStage(int index)
    {
        if (index >= 0 && index < stages.Count)
        {
            stages[index].isUnlocked = true;
        }
    }

    // Additional methods as needed for managing stages
}
