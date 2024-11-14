using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageConfig
{
    public string stageID;
    public string battleSceneName;
    public bool isUnlocked;
    public Vector2 position; // You might include position here if you want to link it directly with the config
}

