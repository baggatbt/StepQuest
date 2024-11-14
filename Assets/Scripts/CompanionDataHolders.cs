using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CompanionData
{
    public string heroID;
    public int heroLevel;
    public string type;
    // Add other properties here
}

[Serializable]
public class KnightData : CompanionData
{
    
    // Add other Knight-specific properties here
}
