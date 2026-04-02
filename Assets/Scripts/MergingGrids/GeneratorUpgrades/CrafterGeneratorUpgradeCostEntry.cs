using System;
using UnityEngine;

[Serializable]
public class CrafterGeneratorUpgradeCostEntry
{
    [Min(2)]
    public int targetLevel = 2;

    [Header("Inventory Costs")]
    public InventoryRequirement[] inventoryCosts;

    [Header("Grid Costs")]
    public GridRequirement[] gridCosts;
}