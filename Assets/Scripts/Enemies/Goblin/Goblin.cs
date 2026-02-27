using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    // ✅ Baseline test stats (Level 1)
    private const int BASE_HEALTH = 20;
    private const int HEALTH_GROWTH = 3;

    private const int BASE_ATTACK = 8;
    private const float ATTACK_GROWTH_FACTOR = 1.5f;

    private const int BASE_DEFENSE = 2;
    private const int DEFENSE_GROWTH = 0;

    // Goblin slightly faster than Knight
    private const int BASE_SPEED = 10;

    private const int BASE_GOLD = 5;
    private const int GOLD_PER_LEVEL = 2;

    private const int BASE_EXP = 10;
    private const float EXP_GROWTH_FACTOR = 1.1f;

    protected override void Awake()
    {
        // ✅ Set level BEFORE base.Awake() so reward calcs don't run at default/old value
        level = 1;

        // Setup exp curve vars before Awake reward calc too (optional but nice)
        baseExp = BASE_EXP;
        growthFactor = EXP_GROWTH_FACTOR;

        base.Awake();

        // Core combat setup
        speed = BASE_SPEED;

        maxEnergy = 1;
        energy = 0;

        attacksBeforeSpecial = 2;

        // Skills
        skills = new List<Skill>
        {
            new GoblinAttackSkill(),
            new GoblinSpecialAttackSkill()
        };

        normalSkill = skills[0];
        specialSkill = skills[1];
        currentSkill = normalSkill;

        // Apply stats now
        UpdateStats();
    }

    public override void UpdateStats()
    {
        base.UpdateStats();

        maxHealth = BASE_HEALTH + (level - 1) * HEALTH_GROWTH;
        health = maxHealth;

        attackPower = BASE_ATTACK + Mathf.FloorToInt((level - 1) * ATTACK_GROWTH_FACTOR);

        defensePower = BASE_DEFENSE + (level - 1) * DEFENSE_GROWTH;

        // Keep speed locked to baseline + (optional growth later if you want)
        speed = BASE_SPEED;

        goldReward = BASE_GOLD + GOLD_PER_LEVEL * level;
        expReward = Mathf.RoundToInt(BASE_EXP * Mathf.Pow(EXP_GROWTH_FACTOR, level - 1));

        // Keep energy within bounds (in case maxEnergy changes later)
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }
}