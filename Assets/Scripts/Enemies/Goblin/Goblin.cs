using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Goblin : Enemy
{
    // Base stats and growth constants
    private const int BASE_HEALTH = 6;              // Health at level 1
    private const int HEALTH_GROWTH = 2;            // Additional HP per level after 1

    private const int BASE_ATTACK = 2;              // Attack at level 1
    private const float ATTACK_GROWTH_FACTOR = 0.5f;  // Adds +1 attack every 2 levels

    private const int BASE_GOLD = 5;                // Base gold reward offset
    private const int GOLD_PER_LEVEL = 2;           // Additional gold per level

    private const int BASE_EXP = 10;                 // Base experience reward at level 1
    private const float EXP_GROWTH_FACTOR = 1.1f;     // EXP growth factor per level

    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake()

        // Initialize level and stats for level 1 using our formulas:
        this.level = 1;
        this.maxHealth = BASE_HEALTH + (this.level - 1) * HEALTH_GROWTH;  // For level 1: 6 HP
        this.health = this.maxHealth;
        this.maxEnergy = 1;
        this.speed = 7;
        this.defensePenetration = 0;
        this.attacksBeforeSpecial = 2;
        this.attackPower = BASE_ATTACK + Mathf.FloorToInt((this.level - 1) * ATTACK_GROWTH_FACTOR);  // For level 1: 2
        this.defensePower = 0;
        this.energy = 0;
        this.baseExp = BASE_EXP; // Base EXP for level 1
        this.growthFactor = EXP_GROWTH_FACTOR;  // EXP scaling factor
        this.multiplier = 0;  // Not used in these formulas, but left here if needed
    }

    protected override void Start()
    {
        base.Start(); // Call the base class Start()

        // Initialize Goblin-specific skills
        this.skills = new List<Skill>
        {
            new GoblinAttackSkill(),
            new GoblinSpecialAttackSkill()
        };

        // Assign skills
        this.normalSkill = this.skills[0];
        this.specialSkill = this.skills[1];
        this.currentSkill = this.normalSkill;
    }

    public override void UpdateStats()
    {
        base.UpdateStats(); // Call base update logic (if any)

        // Update stats based on the current level:
        this.maxHealth = BASE_HEALTH + (this.level - 1) * HEALTH_GROWTH;
        this.health = this.maxHealth;

        // Attack increases by +1 every two levels.
        this.attackPower = BASE_ATTACK + Mathf.FloorToInt((this.level - 1) * ATTACK_GROWTH_FACTOR);

        // Gold reward scales linearly: base gold plus a bonus per level.
        this.goldReward = BASE_GOLD + GOLD_PER_LEVEL * this.level;

        // EXP reward scales exponentially:
        this.expReward = Mathf.RoundToInt(BASE_EXP * Mathf.Pow(EXP_GROWTH_FACTOR, this.level - 1));

        // Other stats remain constant or are set as needed:
        this.attacksBeforeSpecial = 2;
        this.energy = 0;
    }
}
