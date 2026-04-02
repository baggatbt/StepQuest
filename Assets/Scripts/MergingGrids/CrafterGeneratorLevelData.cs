using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CrafterGeneratorLevelData
{
    public int level = 1;
    public List<CrafterGeneratorOutputEntry> outputs = new();
}