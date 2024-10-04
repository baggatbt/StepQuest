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
    public BattleConfig stageBattleConfig;
}