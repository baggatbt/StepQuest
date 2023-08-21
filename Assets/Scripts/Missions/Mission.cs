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
    public int stepsSinceActivation = 0; // This tracks the steps since the mission was activated.
    
    public void ActivateMission()
    {
        isActive = true;
        stepsSinceActivation = 0; // Reset the steps.
    }
    
    public bool CheckMissionCompletion()
    {
        return stepsSinceActivation >= stepCost;
    }
    
    public void ClaimReward()
    {
        if (CheckMissionCompletion())
        {
            // Add rewards here.
            // e.g. PlayerData.Instance.gold += someRewardAmount;
            isActive = false; // End the mission.
        }
    }
}

