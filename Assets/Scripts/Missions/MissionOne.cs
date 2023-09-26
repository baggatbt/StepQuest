using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MissionOne : Mission
{
    public MissionOne()
    {
        missionName = "Test Mission";
        stepCost = 500;
        isActive = false; // This tracks if the mission is currently active.
        stepsAtMissionStart = 0;
        currentSteps = 0;
    }
   
}
