using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    readonly Dictionary<string, PlayableHeroDefinition> registeredHeroes = new Dictionary<string, PlayableHeroDefinition>();
    readonly List<CharacterData> runtimeHeroData = new List<CharacterData>();

    void RegisterAdditionalHeroes()
    {
        foreach (var definition in Resources.LoadAll<PlayableHeroDefinition>("Heroes"))
        {
            if (definition == null || string.IsNullOrEmpty(definition.heroID) || definition.characterData == null || definition.prefab == null)
            { Debug.LogError("A registered hero is missing its ID, data, or prefab."); continue; }
            if (registeredHeroes.ContainsKey(definition.heroID)) continue;
            registeredHeroes.Add(definition.heroID, definition);
            // Keep authored defaults immutable; discover the new hero independently
            // of the contents of an older save's character or party lists.
            var data = Instantiate(definition.characterData);
            data.name = definition.characterData.name + " (runtime)";
            data.heroID = definition.heroID;
            if (data.heroID == Buffmage.HeroID && !PlayerPrefs.HasKey("CharacterData_" + data.heroID))
                Buffmage.InitializeNewData(data, BuffmageSettings.Load());
            allCharacterData.RemoveAll(h => h != null && h.heroID == data.heroID);
            allCharacterData.Add(data); runtimeHeroData.Add(data);
        }
    }

    public bool IsHeroAvailable(CharacterData data)
    {
        return data != null && (data.isUnlocked ||
            (registeredHeroes.TryGetValue(data.heroID, out var definition) && definition.DevelopmentAvailable));
    }

    public string GetHeroUnlockStatus(CharacterData data)
    {
        if (data == null) return "Unavailable";
        if (data.isUnlocked) return "Unlocked";
        return IsHeroAvailable(data) ? "Development testing" : "Locked - quest reward";
    }

    // Future quest rewards can call UnlockHero("Buffmage"). Only this explicit
    // permanent unlock writes isUnlocked; the development override never does.
    public bool UnlockHero(string heroID)
    {
        if (!characterDataDictionary.TryGetValue(heroID, out var data)) return false;
        data.isUnlocked = true; data.SaveData(); return true;
    }

    void FilterUnavailablePartyMembers()
    {
        currentPartyHeroIDs.RemoveAll(id => registeredHeroes.ContainsKey(id) &&
            (!characterDataDictionary.TryGetValue(id, out var data) || !IsHeroAvailable(data)));
    }
}
