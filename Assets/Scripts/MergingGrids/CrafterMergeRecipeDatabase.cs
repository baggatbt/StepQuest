using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Merge Recipe Database")]
public class CrafterMergeRecipeDatabase : ScriptableObject
{
    [SerializeField] private List<CrafterMergeRecipe> recipes = new();

    public CrafterEntityType GetMergeResult(CrafterEntityType a, CrafterEntityType b)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            CrafterMergeRecipe recipe = recipes[i];
            if (recipe == null)
                continue;

            if (recipe.Matches(a, b))
                return recipe.result;
        }

        return CrafterEntityType.None;
    }

    public List<CrafterMergeRecipe> GetAllRecipes()
    {
        return recipes;
    }
}