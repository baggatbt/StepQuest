using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroClassEntry {
    public ClassType classType;
    public Companion prefab;           // Your Knight/Archer prefabs
    public CharacterData data;         // KnightData / ArcherData (SO)
    public bool unlocked = true;       // Use your existing flags if you have them
}

public enum ClassType { Knight, Archer /*, Mage, etc. */ }


public class HeroClassSwitcher : MonoBehaviour
{
    [Header("Where to spawn the active hero")]
    public Transform mount;            // Empty parent at the hero’s position

    [Header("Available hero forms")]
    public List<HeroClassEntry> entries;

    [Header("Default on load")]
    public ClassType defaultClass = ClassType.Knight;

    private Dictionary<ClassType, HeroClassEntry> _map;
    private Companion _activeHero;

    void Awake()
    {
        _map = entries?.ToDictionary(e => e.classType, e => e) ?? new Dictionary<ClassType, HeroClassEntry>();
    }

    void Start()
    {
        // Spawn default if nothing is active
        if (_activeHero == null)
            SwitchTo(defaultClass);
            
    }

    public ClassType CurrentType { get; private set; }  

    public IEnumerable<HeroClassEntry> UnlockedEntries() // helper for UI
    {
        foreach (var e in entries)
            if (e != null && (e.unlocked || (e.data != null && e.data.isUnlocked)))
                yield return e;
    }

    public void SwitchTo(ClassType target)
    {
        if (!_map.TryGetValue(target, out var entry)) {
            Debug.LogError($"[HeroClassSwitcher] No entry for {target}.");
            return;
        }
        if (!entry.unlocked && !(entry.data?.isUnlocked ?? false)) {
            Debug.Log("[HeroClassSwitcher] Target class is locked.");
            return;
        }
         CurrentType = target;   

        // Save & destroy the current hero instance (optional save)
        if (_activeHero != null) {
            try { _activeHero.SaveCharacterData(); } catch {} // safe if you added this
            Destroy(_activeHero.gameObject);
            _activeHero = null;
        }

        // Instantiate the new prefab at the mount
        if (entry.prefab == null) { Debug.LogError("[HeroClassSwitcher] Prefab missing."); return; }

        var go = Instantiate(entry.prefab, mount ? mount : null);
        if (mount != null) {
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        _activeHero = go.GetComponent<Companion>();
        if (_activeHero == null) {
            Debug.LogError("[HeroClassSwitcher] Prefab has no Companion component.");
            Destroy(go);
            return;
        }

        // Ensure the instance uses the right CharacterData (your prefabs likely already reference these)
        if (entry.data != null && _activeHero.characterData != entry.data) {
            _activeHero.SetCharacterData(entry.data);  // uses your existing method
        }

        // Register as the active hero in GameManager so all existing systems keep working
        GameManager.Instance.currentCompanion = _activeHero;
        GameManager.Instance.currentCompanionData = _activeHero.characterData;

        // Keep party consistent if you use it
        if (GameManager.Instance.currentParty != null) {
            GameManager.Instance.currentParty.Clear();
            GameManager.Instance.currentParty.Add(_activeHero);
        }

        

        // Optional: if you want to be extra-safe and force the stats panel update:
        var ui = FindObjectOfType<MainMenuUIManager>();
        if (ui != null && GameManager.Instance.currentCompanionData != null) {
            // You already have this method:
            // UpdateCompanionStatsDisplay(CharacterData characterData)
            var cd = GameManager.Instance.currentCompanionData;
            ui.SendMessage("UpdateCompanionStatsDisplay", cd, SendMessageOptions.DontRequireReceiver);
        }

        Debug.Log($"[HeroClassSwitcher] Switched to {target} ({_activeHero.characterData?.heroID}).");
    }

    // Convenience (wire these to buttons if you like)
    public void SwitchToKnight() => SwitchTo(ClassType.Knight);
    public void SwitchToArcher() => SwitchTo(ClassType.Archer);
}
