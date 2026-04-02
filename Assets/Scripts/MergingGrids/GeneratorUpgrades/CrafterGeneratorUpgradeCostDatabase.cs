using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Generator Upgrade Cost Database")]
public class CrafterGeneratorUpgradeCostDatabase : ScriptableObject
{
    [SerializeField] private List<CrafterGeneratorUpgradeCostProfile> profiles = new();

    private Dictionary<CrafterEntityType, CrafterGeneratorUpgradeCostProfile> lookup;

    public void BuildLookup()
    {
        lookup = new Dictionary<CrafterEntityType, CrafterGeneratorUpgradeCostProfile>();

        for (int i = 0; i < profiles.Count; i++)
        {
            CrafterGeneratorUpgradeCostProfile profile = profiles[i];
            if (profile == null)
                continue;

            if (!lookup.ContainsKey(profile.generatorType))
                lookup.Add(profile.generatorType, profile);
            else
                Debug.LogWarning($"Duplicate upgrade cost profile for {profile.generatorType}");
        }
    }

    public CrafterGeneratorUpgradeCostProfile GetProfile(CrafterEntityType generatorType)
    {
        if (lookup == null)
            BuildLookup();

        lookup.TryGetValue(generatorType, out var profile);
        return profile;
    }

    public CrafterGeneratorUpgradeCostEntry GetNextCost(CrafterEntityType generatorType, int currentLevel)
    {
        CrafterGeneratorUpgradeCostProfile profile = GetProfile(generatorType);
        if (profile == null)
            return null;

        return profile.GetNextCostFromCurrentLevel(currentLevel);
    }
}