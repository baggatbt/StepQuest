using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class QuestOne : Quest
{
    public Button missionOneStartButton;
    public TextMeshProUGUI stepCostText;
    public TextMeshProUGUI stepsToGoText;
    
    

    private void Awake()
    {
        questName = "Test Mission";
        stepCost = 500;
        isActive = false; // This tracks if the mission is currently active.
       
        
    }

    //Update on 5 second interval to reduce load
    private float updateInterval = 3.0f;
    private float nextUpdate = 0.0f;

     private void Update()
    {
    if (isActive && Time.time >= nextUpdate)
    {
        stepsToGo = stepCost - currentSteps;
        Debug.Log(stepsToGo);
        nextUpdate = Time.time + updateInterval;
        UpdateCurrentSteps();
        if (stepsToGo > 0)
        {
        stepsToGoText.text = "Steps to Go: " + stepsToGo;
        }
    }
    }

    
    public void UpdateCurrentSteps()
    {
        currentSteps = (PlayerData.Instance.inGameSteps - stepsAtMissionStart);
    
    }

    public void OnClickMissionOneStart()
    {
        if (!isActive)
        {
           
            ActivateMission();
            
        }
        CheckMissionCompletion();
    }

    public void ActivateMission()
    {
        if (isActive == false)
        {
            isActive = true;
            stepsAtMissionStart = PlayerData.Instance.inGameSteps;
            Debug.Log("Steps at mission start" +stepsAtMissionStart);
        }
    }

    public void CheckMissionCompletion()  
    {
           
        if (currentSteps >= stepCost) 
        {
            ClaimReward();
        }
        else 
        {
            Debug.Log("Not enough steps");
        }
            
    }
    
    public override void ClaimReward()
    {
        // Add rewards here.
        PlayerData.Instance.gold += 100;
        PlayerData.Instance.exp += 100;
        this.isActive = false; // End the mission.  
        Debug.Log("Rewards have been claimed");     
    }  
   
}
