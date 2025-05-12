using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps per-resource tool levels and hands out tap-damage numbers.
/// Extend with more resources as needed.
/// </summary>
public class PlayerHarvestStats : MonoBehaviour
{
    // ───────── singleton
    public static PlayerHarvestStats Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    // ───────── per-resource tool tiers
    [Header("Tool Levels (saved to PlayerPrefs)")]
    [SerializeField] private int axeLevel = 0;        // affects Wood
    [SerializeField] private int pickaxeLevel = 0;    // affects Ore, etc.

    // expose current level & damage
    public int  GetToolLevel   (ResourceType type) => type switch
    {
        ResourceType.Wood  => axeLevel,
        ResourceType.Ore   => pickaxeLevel,
        _                  => 0
    };

    public int  GetTapDamage   (ResourceType type)
    {
        // Base 1 + 1 per level; tweak as you like.
        return 1 + GetToolLevel(type);
    }

    public void IncreaseToolLevel(ResourceType type, int amount = 1)
    {
        switch (type)
        {
            case ResourceType.Wood:  axeLevel     += amount; break;
            case ResourceType.Ore:   pickaxeLevel += amount; break;
        }
        Save();
    }

    // ───────── save / load
    private const string AxeKey     = "AxeLvl";
    private const string PickKey    = "PickaxeLvl";

    private void Start()  => Load();

    private void OnApplicationQuit() => Save();

    private void Save()
    {
        PlayerPrefs.SetInt(AxeKey , axeLevel);
        PlayerPrefs.SetInt(PickKey, pickaxeLevel);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        axeLevel     = PlayerPrefs.GetInt(AxeKey , 0);
        pickaxeLevel = PlayerPrefs.GetInt(PickKey, 0);
    }
}
