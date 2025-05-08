// ─────────────────────────────────────────────────────────────
// Idle Mission System
// Purpose: Allows sending companions to gather resources while idle
// Rewards based on steps taken while the mission is running.
// ─────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BackgroundBattleMission
{
    public string stageID;
    public List<string> assignedHeroIDs;
    public int stepsAtStart;
    public string timeStartedIso; // stored as ISO string

    [NonSerialized] public DateTime timeStarted;
    public int RequiredSteps = 1000;

    public void InitializeTime()
    {
        timeStarted = DateTime.Now;
        timeStartedIso = timeStarted.ToString("o");
    }

    public void RestoreTime()
    {
        if (!string.IsNullOrEmpty(timeStartedIso))
            timeStarted = DateTime.Parse(timeStartedIso);
    }

    public int GetStepsSinceStart() => PlayerData.Instance.inGameSteps - stepsAtStart;
    public bool IsComplete => GetStepsSinceStart() >= RequiredSteps;

    public int CalculateExpReward() => GetStepsSinceStart() / 10;
    public int CalculateGoldReward() => GetStepsSinceStart() / 20;
}


