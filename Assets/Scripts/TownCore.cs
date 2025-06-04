using UnityEngine;

public class TownCore : MonoBehaviour
{
    public static TownCore Instance { get; private set; }

    [Header("TownCore Leveling")]
    [Tooltip("Current upgrade level of the Town Core (starts at 1).")]
    public int level = 1;

    [Tooltip("Maximum level the Town Core can reach.")]
    public int maxLevel = 10;

    [Header("Step Cap Configuration")]
    [Tooltip("How many steps the Town Core holds at level 1.")]
    public int baseStepCap = 1000;

    [Tooltip("For a linear cap: steps added each level. E.g. +500 steps per level.")]
    public int stepCapPerLevel = 500;

    // If you want an exponential curve instead, comment out the above two,
    // and use these two fields instead:
    // public float stepCapGrowthFactor = 1.2f;
    // (Cap at level N ≈ baseStepCap * (stepCapGrowthFactor^(N−1)))

    [Header("Upgrade Cost Configuration")]
    [Tooltip("Base copper cost for upgrading from level 1 → 2.")]
    public int upgradeBaseCopperCost = 200;

    [Tooltip("Cost growth factor per TownCore level (e.g. 1.5 means cost × 1.5 each level).")]
    public float costGrowthFactor = 1.5f;

    [Header("(Optional) Other Resource Costs")]
    // If you want to add wood/ore/anything else, you could define:
    // public int upgradeWoodCost = 50;
    // public int upgradeOreCost = 20;
    // … and so on.

    private const string PREF_KEY_TOWN_LEVEL = "TownCore_Level";

    private void Awake()
    {
        // Enforce singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTownCoreData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadTownCoreData()
    {
        level = PlayerPrefs.GetInt(PREF_KEY_TOWN_LEVEL, 1);
        // Clamp in case someone manually tampered:
        level = Mathf.Clamp(level, 1, maxLevel);
    }

    private void SaveTownCoreData()
    {
        PlayerPrefs.SetInt(PREF_KEY_TOWN_LEVEL, level);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns the step cap based on current TownCore level.
    /// This example uses a **linear** increase per level:
    ///   Cap(level N) = baseStepCap + (N − 1) × stepCapPerLevel
    /// 
    /// If you prefer an exponential curve, replace the body with:
    ///   return Mathf.RoundToInt(
    ///       baseStepCap * Mathf.Pow(stepCapGrowthFactor, Mathf.Max(level − 1, 0))
    ///   );
    /// </summary>
    public int GetCurrentStepCap()
    {
        return baseStepCap + (level - 1) * stepCapPerLevel;

        // Exponential version (uncomment if you want):
        // float exponent = Mathf.Max(level - 1, 0);
        // return Mathf.RoundToInt(baseStepCap * Mathf.Pow(stepCapGrowthFactor, exponent));
    }

    /// <summary>
    /// Computes how much copper it costs to go from current level → current level + 1.
    /// Cost(level N → N+1) = upgradeBaseCopperCost * (costGrowthFactor^(N−1))
    /// Rounds to nearest int.
    /// </summary>
    public int GetUpgradeCopperCost()
    {
        if (level >= maxLevel)
            return 0; // Already at max—no further cost.

        float rawCost = upgradeBaseCopperCost * Mathf.Pow(costGrowthFactor, Mathf.Max(level - 1, 0));
        return Mathf.RoundToInt(rawCost);
    }

    /// <summary>
    /// Attempts to upgrade TownCore by one level.
    /// Returns true if upgrade succeeded (enough resources & not at max level),
    /// false otherwise.
    /// 
    /// This example only checks “copper” (using PlayerData.Instance.totalCopper).
    /// If you have more resources (wood, ore, etc.), add those checks/deductions here.
    /// </summary>
    public bool UpgradeTownCore()
    {
        if (level >= maxLevel)
            return false; // Already at max.

        int costCopper = GetUpgradeCopperCost();

        // Ensure PlayerData.Instance is ready
        if (PlayerData.Instance == null)
        {
            Debug.LogError("[TownCore] PlayerData.Instance is null!");
            return false;
        }

        // Check if player has enough copper
        if (PlayerData.Instance.totalCopper < costCopper)
            return false; // Not enough copper.

        // Deduct copper cost
        PlayerData.Instance.totalCopper -= costCopper;
        PlayerData.Instance.SavePlayerData();

        // (Optional) Deduct other resources here, e.g.:
        // if (PlayerData.Instance.wood < upgradeWoodCost) return false;
        // PlayerData.Instance.wood -= upgradeWoodCost;

        // Bump level
        level = Mathf.Min(level + 1, maxLevel);
        SaveTownCoreData();

        // (Optional) Fire an event or update UI, e.g.:
        // OnTownCoreUpgraded?.Invoke(level);

        Debug.Log($"[TownCore] Upgraded to level {level}. New step cap = {GetCurrentStepCap()}.");
        return true;
    }

    /// <summary>
    /// Returns true if TownCore is at its maximum level.
    /// </summary>
    public bool IsMaxLevel()
    {
        return level >= maxLevel;
    }
}
