using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Entity Database")]
public class CrafterEntityDatabase : ScriptableObject
{
    [SerializeField] private List<CrafterEntityDefinition> definitions = new();

    private Dictionary<CrafterEntityType, CrafterEntityDefinition> lookup;

    public void BuildLookup()
    {
        lookup = new Dictionary<CrafterEntityType, CrafterEntityDefinition>();

        foreach (var def in definitions)
        {
            if (def == null) continue;

            if (!lookup.ContainsKey(def.entityType))
                lookup.Add(def.entityType, def);
            else
                Debug.LogWarning($"Duplicate CrafterEntityDefinition for {def.entityType}");
        }
    }

    public CrafterEntityDefinition Get(CrafterEntityType type)
    {
        if (lookup == null)
            BuildLookup();

        lookup.TryGetValue(type, out var def);
        return def;
    }

    public List<CrafterEntityDefinition> GetAll()
    {
        return definitions;
    }
}