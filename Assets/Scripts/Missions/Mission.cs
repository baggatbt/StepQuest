using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Mission : MonoBehaviour
{
    public string missionName;
    public int stepCost;
    public int stepsToGo;
    public int currentSteps;
    public int stepsAtStart;

    public void Start()
    {
        stepsAtStart = PlayerData.Instance.steps; // get current steps at the start
        UpdateMissionProgress(); // call to initialize values
    }

    protected virtual void Update()
    {
        UpdateMissionProgress();
    }

    public void UpdateMissionProgress()
    {
        currentSteps = PlayerData.Instance.steps - stepsAtStart;
        stepsToGo = stepCost - currentSteps;
    }

    public virtual void ClaimReward()
    {
        if(stepsToGo <= 0)
        {
            // reward logic here, for instance:
            PlayerData.Instance.gold += 100; // Giving 100 gold as an example
            Debug.Log("Rewards claimed!");

            // Reset the mission or move to the next one or disable the claim button
        }
        else
        {
            Debug.Log("Not enough steps!");
        }
    }
}
