using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingUpgradeCost", menuName = "StepQuest/Building Upgrade Cost")]
public class BuildingUpgradeCost : ScriptableObject
{
    public string upgradeName;
    public int targetLevel = 2;

    [Header("Inventory Costs")]
    public List<InventoryRequirement> inventoryCosts = new();

    [Header("Grid Costs")]
    public List<GridRequirement> gridCosts = new();
}