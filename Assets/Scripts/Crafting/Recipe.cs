using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Ingredients (must be MaterialItem assets)")]
    public List<MaterialRequirement> materialRequirements = new List<MaterialRequirement>();

    [Header("Output (can be MaterialItem, Equipment, etc.)")]
    public Item outputItem;
    public int outputQuantity = 1;
    public int expForCraft;

    [Header("Crafting Duration (in steps)")]
    [Tooltip("How many steps this recipe takes before it finishes")]
    public float craftDuration = 1f;

    [Header("Unlock Requirements")]
    [Tooltip("Player's crafting level must be at least this to see/use the recipe")]
    public int requiredCraftingLevel = 0;
}
