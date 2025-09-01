using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string stageID;
    public string battleSceneName;
    public bool isUnlocked;
    public bool isBossBattle;
    public int stepCost;
    public BattleConfig stageBattleConfig;
    public List<string> connectedStageIDs;      // List of stage IDs connected to this stage

    
}