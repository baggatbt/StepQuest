using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MissionOne : Mission
{
    [Header("Mission One Rewards")]
    public int goldReward = 100;  // For example, a reward of 100 gold

    // Start is called before the first frame update
    void Start()
    {
        missionName = "Mission One";
        stepCost = 500;

        // Ensure base mission logic is run
        base.Start();
    }

    // If you want to give MissionOne-specific rewards, override the ClaimReward method.
    public override void ClaimReward()
    {
        if(stepsToGo <= 0)
        {
            PlayerData.Instance.gold += goldReward; 
            Debug.Log("Rewards claimed for Mission One!");

            // You can reset the mission, move to the next one, or disable the claim button
            // For simplicity, let's disable this mission after claiming once:
            this.enabled = false;
            // Maybe inform the player:
            Debug.Log("Mission One completed!");
        }
        else
        {
            Debug.Log("Not enough steps for Mission One!");
        }
    }
}
