using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Header("Rewards")]
    public int expReward;
    public int goldReward;

    [Header("EXP Curve (optional)")]
    public int baseExp;                 // Base experience points given by enemies
    public float growthFactor = 1.1f;   // Growth factor to control curve steepness
    public int multiplier;              // Multiplier for experience calculation (if used)

    [Header("Drops")]
    public Sprite enemyIcon;
    public MaterialItem[] possibleDrops;     // Assign in Inspector
    public int dropChancePercentage = 50;    // 0-100

    protected override void Awake()
    {
        base.Awake();
        UpdateRewards();
    }

    /// <summary>
    /// Call this whenever level changes (or after UpdateStats()).
    /// </summary>
    protected virtual void UpdateRewards()
    {
        expReward = CalculateExpReward(level);
        goldReward = CalculateGoldReward(level);
    }

    /// <summary>
    /// Optional alt exp function you had (kept for experimentation).
    /// </summary>
    protected virtual int ExpGivenByEnemy(int heroLevel)
    {
        int exp = Mathf.RoundToInt(baseExp + multiplier * Mathf.Pow(heroLevel, growthFactor));
        Debug.Log($"Level {heroLevel} with Multiplier {multiplier}: EXP Reward {exp}");
        return exp;
    }

    // Your original curve (kept)
    protected int CalculateExpReward(int lvl)
    {
        if (lvl < 20)
        {
            return 100 * lvl * lvl;
        }
        else
        {
            return (int)(100 * Mathf.Pow(1.5f, lvl - 19) * 400);
        }
    }

    protected int CalculateGoldReward(int lvl)
    {
        return 3 + (lvl * 2);
    }

    // Call when enemy is defeated.
    public void DropMaterial()
    {
        if (possibleDrops == null || possibleDrops.Length == 0) return;

        if (Random.Range(0, 100) < dropChancePercentage)
        {
            int dropIndex = Random.Range(0, possibleDrops.Length);
            MaterialItem droppedMaterial = possibleDrops[dropIndex];

            Debug.Log("Dropped material: " + droppedMaterial.itemName);

            // Route depending on run state
            if (GameManager.Instance != null)
                GameManager.Instance.AddLootItem(droppedMaterial, 1);
        }
    }

    public override void UpdateStats()
    {
        base.UpdateStats();
        // Enemies override in derived classes (Goblin, etc.)
        // but we keep this for the hook.
    }
}