using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Generator Upgrade Database")]
public class CrafterGeneratorUpgradeDatabase : ScriptableObject
{
    [SerializeField] private List<CrafterGeneratorUpgradeProfile> profiles = new();

    private Dictionary<CrafterEntityType, CrafterGeneratorUpgradeProfile> lookup;

    public void BuildLookup()
    {
        lookup = new Dictionary<CrafterEntityType, CrafterGeneratorUpgradeProfile>();

        for (int i = 0; i < profiles.Count; i++)
        {
            CrafterGeneratorUpgradeProfile profile = profiles[i];
            if (profile == null)
                continue;

            if (!lookup.ContainsKey(profile.generatorType))
                lookup.Add(profile.generatorType, profile);
            else
                Debug.LogWarning($"Duplicate generator upgrade profile for {profile.generatorType}");
        }
    }

    public CrafterGeneratorUpgradeProfile GetProfile(CrafterEntityType generatorType)
    {
        if (lookup == null)
            BuildLookup();

        lookup.TryGetValue(generatorType, out var profile);
        return profile;
    }

    public int GetMaxLevel(CrafterEntityType generatorType)
    {
        CrafterGeneratorUpgradeProfile profile = GetProfile(generatorType);
        return profile != null ? profile.GetMaxLevel() : 1;
    }

    public CrafterEntityType GetOutputForLevel(
        CrafterEntityType generatorType,
        int level,
        CrafterEntityType fallbackOutput)
    {
        CrafterGeneratorUpgradeProfile profile = GetProfile(generatorType);
        if (profile == null)
            return fallbackOutput;

        CrafterGeneratorLevelData levelData = profile.GetLevelData(level);
        if (levelData == null || levelData.outputs == null || levelData.outputs.Count == 0)
            return fallbackOutput;

        int totalWeight = 0;

        for (int i = 0; i < levelData.outputs.Count; i++)
        {
            CrafterGeneratorOutputEntry entry = levelData.outputs[i];
            if (entry == null || entry.entityType == CrafterEntityType.None || entry.weight <= 0)
                continue;

            totalWeight += entry.weight;
        }

        if (totalWeight <= 0)
            return fallbackOutput;

        int roll = Random.Range(0, totalWeight);
        int running = 0;

        for (int i = 0; i < levelData.outputs.Count; i++)
        {
            CrafterGeneratorOutputEntry entry = levelData.outputs[i];
            if (entry == null || entry.entityType == CrafterEntityType.None || entry.weight <= 0)
                continue;

            running += entry.weight;
            if (roll < running)
                return entry.entityType;
        }

        return fallbackOutput;
    }
}