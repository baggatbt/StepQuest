using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Knight : Companion
{
    // =========================
    // Baseline (Level 1 targets)
    // =========================
    private const int BASE_HEALTH  = 24;
    private const int BASE_ATTACK  = 10;
    private const int BASE_DEFENSE = 3;
    private const int BASE_SPEED   = 9;   // slightly slower than goblin baseline you wanted
    private const int BASE_ENERGY  = 5;

    // =========================
    // Progression (per level)
    // =========================
    private const int HEALTH_GROWTH = 5;  // +5 HP per level feels chunky but readable
    private const int ATTACK_GROWTH = 2;  // +2 ATK per level keeps damage scaling steady

    // Defense/speed/energy scale slower via steps:
    // - Speed: +1 every 3 levels
    // - Defense: +1 every 4 levels
    // - Energy: +1 every 2 levels

    protected override void Awake()
    {
        base.Awake();

        // Starter skills
        skillOne = SkillType.TripleHit;
        skillTwo = SkillType.Taunt;

        InitializeSkillsBasedOnLevel();

        // Build stats from current heroLevel (loaded or defaults)
        ApplyProgressionStats();

        // If you want to ALWAYS start test battles at full hp/energy, call this:
        // FillToMax();
        //
        // If you want to preserve saved HP/EN normally, but fix "0 hp after prefs clear",
        // keep this (recommended):
        EnsureVitalsInitialized();

        // Save so characterData matches runtime stats (your existing pattern)
        SaveCharacterData();
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();
        availableSkills.Add(SkillType.Slash);
        Debug.Log($"[Knight] Available skills: {availableSkills.Count}");
    }

    /// <summary>
    /// Computes derived stats from heroLevel using your intended curve.
    /// </summary>
    private void ApplyProgressionStats()
    {
        int lvl = Mathf.Max(heroLevel, 1);

        maxHealth   = BASE_HEALTH + (lvl - 1) * HEALTH_GROWTH;
        attackPower = BASE_ATTACK + (lvl - 1) * ATTACK_GROWTH;

        speed       = BASE_SPEED   + Mathf.FloorToInt((lvl - 1) / 3f);
        defensePower= BASE_DEFENSE + Mathf.FloorToInt((lvl - 1) / 4f);
        maxEnergy   = BASE_ENERGY  + Mathf.FloorToInt((lvl - 1) / 2f);

        // Clamp current values to new maxes (don’t set to max here; we do it in EnsureVitalsInitialized)
        health = Mathf.Clamp(health, 0, maxHealth);
        energy = Mathf.Clamp(energy, 0, maxEnergy);

        // Keep ScriptableObject mirrored (your UI/inspector reads from it)
        if (characterData != null)
        {
            characterData.maxHealth   = maxHealth;
            characterData.health      = health;

            characterData.attackPower = attackPower;
            characterData.defensePower= defensePower;
            characterData.speed       = speed;

            characterData.maxEnergy   = maxEnergy;
            characterData.energy      = energy;

            characterData.heroLevel       = heroLevel;
            characterData.heroExp         = heroExp;
            characterData.heroStatPoints  = heroStatPoints;
            characterData.heroSkillPoints = heroSkillPoints;
        }
    }

    /// <summary>
    /// Fixes the "fresh run has 0 currentHP / 0 energy" issue without requiring a rework.
    /// - If prefs were cleared or no save existed, health/energy are commonly 0.
    /// - We treat 0 as "uninitialized" and fill to max.
    /// </summary>
    private void EnsureVitalsInitialized()
    {
        // If you've cleared prefs, your CharacterData likely has health=0 (or heroLevel defaulted weird).
        // This ensures you start playable.
        if (health <= 0 || health > maxHealth)
            health = maxHealth;

        if (energy < 0 || energy > maxEnergy)
            energy = 0;

        // Mirror to data object so the UI matches
        if (characterData != null)
        {
            characterData.health = health;
            characterData.energy = energy;
        }
    }

    /// <summary>Optional helper: always start at full HP/EN (useful for testing).</summary>
    public void FillToMax()
    {
        health = maxHealth;
        energy = maxEnergy;

        if (characterData != null)
        {
            characterData.health = health;
            characterData.energy = energy;
        }

        SaveCharacterData();
    }

    public override void LevelUp()
    {
        if (heroExp >= ExpToNextLevel(heroLevel))
        {
            heroExp -= ExpToNextLevel(heroLevel);
            heroLevel++;

            // Recompute stats for new level
            ApplyProgressionStats();

            // Many RPGs refill on level up; if you don’t want that, delete these 2 lines:
            health = maxHealth;
            energy = Mathf.Clamp(energy, 0, maxEnergy);

            // Mirror + save
            if (characterData != null)
            {
                characterData.heroLevel = heroLevel;
                characterData.heroExp = heroExp;
                characterData.health = health;
                characterData.energy = energy;
            }

            SaveCharacterData();
            Debug.Log($"[Knight] Leveled up to {heroLevel}");
        }
    }

    public override List<SkillType> AvailableSkills => availableSkills;

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        SkillType.ReflectDamagePassive,
        SkillType.SpeedBreak,
    };

    public override List<SkillType> AllSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.SpeedBreak,
        SkillType.SwordWave
    };

    private List<SkillType> mainSkills = new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.SpeedBreak,
        SkillType.SwordWave
    };

    public override List<SkillType> MainSkills => mainSkills;

    public override Skill GetSkillInstance(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Slash: { var s = new Slash(); s.Type = SkillType.Slash; return s; }
            case SkillType.TripleHit: { var s = new TripleHitSkill(); s.Type = SkillType.TripleHit; return s; }
            case SkillType.Taunt: { var s = new Taunt(); s.Type = SkillType.Taunt; return s; }
            case SkillType.ReflectDamagePassive: { var s = new ReflectDamagePassive(); s.Type = SkillType.ReflectDamagePassive; return s; }
            case SkillType.SpeedBreak: { var s = new SpeedBreak(); s.Type = SkillType.SpeedBreak; return s; }
            case SkillType.SwordWave: { var s = new SwordWave(); s.Type = SkillType.SwordWave; return s; }
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
}