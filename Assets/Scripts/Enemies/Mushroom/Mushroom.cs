using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Enemy
{
    // Beefier than goblin: more HP + more DEF, slower speed
    private const int BASE_HEALTH = 26;
    private const int HEALTH_GROWTH = 5;

    private const int BASE_ATTACK = 9;
    private const float ATTACK_GROWTH_FACTOR = 1.3f;

    private const int BASE_DEFENSE = 3;
    private const int DEFENSE_GROWTH = 1;

    // Slower than goblin/knight, but does grow slowly
    private const int BASE_SPEED = 6;

    private const int BASE_GOLD = 6;
    private const int GOLD_PER_LEVEL = 2;

    private const int BASE_EXP = 14;
    private const float EXP_GROWTH_FACTOR = 1.1f;

    protected override void Awake()
    {
        // ✅ Set level & reward curve BEFORE base.Awake() (same pattern as Goblin)
        level = 1;
        baseExp = BASE_EXP;
        growthFactor = EXP_GROWTH_FACTOR;

        base.Awake();

        maxEnergy = 1;
        energy = 0;

        attacksBeforeSpecial = 2;

        skills = new List<Skill>
        {
            new MushroomAttackSkill(),
            new MushroomSpecialAttackSkill()
        };

        normalSkill = skills[0];
        specialSkill = skills[1];
        currentSkill = normalSkill;

        UpdateStats();
    }

    public override void UpdateStats()
    {
        base.UpdateStats();

        maxHealth = BASE_HEALTH + (level - 1) * HEALTH_GROWTH;
        health = maxHealth;

        attackPower = BASE_ATTACK + Mathf.FloorToInt((level - 1) * ATTACK_GROWTH_FACTOR);

        defensePower = BASE_DEFENSE + (level - 1) * DEFENSE_GROWTH;

        // Slow-ish growth that keeps it behind goblin/knight curves
        // Example: +1 speed every 4 levels
        speed = BASE_SPEED + Mathf.FloorToInt((level - 1) / 4f);

        goldReward = BASE_GOLD + GOLD_PER_LEVEL * level;
        expReward = Mathf.RoundToInt(BASE_EXP * Mathf.Pow(EXP_GROWTH_FACTOR, level - 1));

        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }
}