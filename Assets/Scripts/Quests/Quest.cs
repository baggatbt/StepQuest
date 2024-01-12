using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Quest: MonoBehaviour
{
    public string questName;
    public int stepCost; // The total steps required to complete this mission.
    public bool isActive; // This tracks if the mission is currently active.
    public int currentSteps;
    public int stepsAtMissionStart; // Gets players current in game steps 
    public int stepsToGo;

 

    
    
    
    public virtual void ClaimReward()
    {
        // Add rewards here.
        PlayerData.Instance.gold += 1;
        PlayerData.Instance.exp += 1;
        this.isActive = false; // End the mission.       
    }  
    
}

