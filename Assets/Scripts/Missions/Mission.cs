using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Mission : MonoBehaviour
{
    public string missionName;
    public int stepCost; // The total steps required to complete this mission.
    public bool isActive = false; // This tracks if the mission is currently active.
    public int currentSteps = 0;
    public int stepsAtMissionStart = 0;

    private Update()
    {
        while (isActive)
        {
            currentSteps = (PlayerData.Instance.inGameSteps - stepsAtMissionStart);
        }
    }
    
    public void ActivateMission()
    {
        if (!isActive)
        {
            isActive = true;
            stepsAtMissionStart = PlayerData.Instance.inGameSteps
            
        }
    }
    
    public void checkMissionCompletion()
    {
        while (isActive)
        {
            if (currentSteps >= stepCost) 
            {
                ClaimReward();
            }
        }
    }
    
    public void ClaimReward()
    {
        // Add rewards here.
        PlayerData.Instance.gold += 100;
        PlayerData.Instance.exp += 100;
        isActive = false; // End the mission.       
    }  
    
}

