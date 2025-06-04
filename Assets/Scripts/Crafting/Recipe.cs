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

    [Header("Crafting Duration (in seconds or steps)")]
    [Tooltip("How many steps or seconds this recipe takes")]
    public float craftDuration = 1f;
}
