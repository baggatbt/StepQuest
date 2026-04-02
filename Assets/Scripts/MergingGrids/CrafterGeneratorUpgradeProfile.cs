using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Generator Upgrade Profile")]
public class CrafterGeneratorUpgradeProfile : ScriptableObject
{
    public CrafterEntityType generatorType;
    public List<CrafterGeneratorLevelData> levels = new();

    public CrafterGeneratorLevelData GetLevelData(int level)
    {
        CrafterGeneratorLevelData best = null;

        for (int i = 0; i < levels.Count; i++)
        {
            CrafterGeneratorLevelData data = levels[i];
            if (data == null)
                continue;

            if (data.level == level)
                return data;

            if (data.level <= level)
            {
                if (best == null || data.level > best.level)
                    best = data;
            }
        }

        return best;
    }

    public int GetMaxLevel()
    {
        int max = 1;

        for (int i = 0; i < levels.Count; i++)
        {
            CrafterGeneratorLevelData data = levels[i];
            if (data == null)
                continue;

            if (data.level > max)
                max = data.level;
        }

        return max;
    }
}