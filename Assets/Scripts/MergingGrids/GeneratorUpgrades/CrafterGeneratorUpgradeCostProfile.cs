using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Generator Upgrade Cost Profile")]
public class CrafterGeneratorUpgradeCostProfile : ScriptableObject
{
    public CrafterEntityType generatorType;
    public List<CrafterGeneratorUpgradeCostEntry> levelCosts = new();

    public CrafterGeneratorUpgradeCostEntry GetCostForTargetLevel(int targetLevel)
    {
        for (int i = 0; i < levelCosts.Count; i++)
        {
            CrafterGeneratorUpgradeCostEntry entry = levelCosts[i];
            if (entry == null)
                continue;

            if (entry.targetLevel == targetLevel)
                return entry;
        }

        return null;
    }

    public CrafterGeneratorUpgradeCostEntry GetNextCostFromCurrentLevel(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        return GetCostForTargetLevel(nextLevel);
    }
}