using System;
using UnityEngine;

[Serializable]
public class CrafterGeneratorOutputEntry
{
    public CrafterEntityType entityType;
    [Min(0)]
    public int weight = 1;
}