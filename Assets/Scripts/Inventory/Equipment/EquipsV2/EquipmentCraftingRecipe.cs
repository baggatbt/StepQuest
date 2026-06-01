using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafting/Equipment Recipe")]
public class EquipmentCraftingRecipe : ScriptableObject
{
    [Header("Recipe Info")]
    public string recipeName;

    [TextArea]
    public string description;

    [Header("Result")]
    public Equipment resultEquipmentTemplate;

    [Header("Costs")]
    public List<EquipmentMaterialCost> materialCosts = new List<EquipmentMaterialCost>();

    public int goldCost = 0;
    public int stepCost = 0;
}