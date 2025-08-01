using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Research Node", menuName = "Research/Research Node")]
public class ResearchNode : ScriptableObject
{
    public string nodeID;
    public string displayName;
    public string description;
    public int stepCost;

    public GlobalStatType globalStatType;
    public int globalStatAmount;

    public List<ResearchNode> prerequisites;
}

public enum GlobalStatType
{
    None,
    Attack,
    Health,
    Defense,
    Speed
}

