using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    private const int BASE_HEALTH = 12;
    private const int HEALTH_GROWTH = 3;

    private const int BASE_ATTACK = 6;
    private const float ATTACK_GROWTH_FACTOR = 1.5f;

    private const int BASE_DEFENSE = 0;
    private const int DEFENSE_GROWTH = 0;

    // Slightly faster than Knight curve
    private const int BASE_SPEED = 10;

    private const int BASE_GOLD = 5;
    private const int GOLD_PER_LEVEL = 2;

    private const int BASE_EXP = 10;
    private const float EXP_GROWTH_FACTOR = 1.1f;

    protected override void Awake()
    {
        // ✅ Set level BEFORE base.Awake()
        level = 1;

        baseExp = BASE_EXP;
        growthFactor = EXP_GROWTH_FACTOR;

        base.Awake();

        maxEnergy = 1;
        energy = 0;

        attacksBeforeSpecial = 2;

        skills = new List<Skill>
        {
            new GoblinAttackSkill(),
            new GoblinSpecialAttackSkill()
        };

        normalSkill = skills[0];
        specialSkill = skills[1];
        currentSkill = normalSkill;

        UpdateStats();
    }

    public override void UpdateStats()
    {
        base.UpdateStats();

        int lvl = Mathf.Max(level, 1);

        // HP scaling (weaker than Knight)
        maxHealth = BASE_HEALTH + (lvl - 1) * HEALTH_GROWTH;
        health = maxHealth;

        // ATK scaling (slightly weaker curve than Knight)
        attackPower = BASE_ATTACK + Mathf.FloorToInt((lvl - 1) * ATTACK_GROWTH_FACTOR);

        // DEF scaling (currently flat)
        defensePower = BASE_DEFENSE + (lvl - 1) * DEFENSE_GROWTH;

        // SPEED scaling (same growth rhythm as Knight, but starts higher)
        speed = BASE_SPEED + Mathf.FloorToInt((lvl - 1) / 3f);

        // Rewards
        goldReward = BASE_GOLD + GOLD_PER_LEVEL * lvl;
        expReward = Mathf.RoundToInt(BASE_EXP * Mathf.Pow(EXP_GROWTH_FACTOR, lvl - 1));

        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }
}