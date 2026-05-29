using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Enemy
{
    [Header("Mushroom Stat Growth")]
    private const int BASE_HEALTH = 22;
    private const int HEALTH_GROWTH = 5;

    private const int BASE_ATTACK = 6;
    private const float ATTACK_GROWTH_FACTOR = 1.0f;

    private const int BASE_DEFENSE = 0;
    private const float DEFENSE_GROWTH_FACTOR = 0.25f;

    // Slower than Knight and Goblin.
    // Knight = 4, Goblin = 5, Mushroom = 3.
    private const int BASE_SPEED = 3;
    private const float SPEED_GROWTH_FACTOR = 0.20f;

    [Header("Mushroom Rewards")]
    private const int BASE_GOLD = 6;
    private const int GOLD_PER_LEVEL = 2;

    private const int BASE_EXP = 12;
    private const float EXP_GROWTH_FACTOR = 1.12f;

    protected override void Awake()
    {
        // Do not force level to 1 if spawner/config already assigned a level.
        if (level <= 0)
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

        int lvl = Mathf.Max(level, 1);

        maxHealth = BASE_HEALTH + ((lvl - 1) * HEALTH_GROWTH);
        health = maxHealth;

        attackPower = BASE_ATTACK + Mathf.FloorToInt((lvl - 1) * ATTACK_GROWTH_FACTOR);

        defensePower = BASE_DEFENSE + Mathf.FloorToInt((lvl - 1) * DEFENSE_GROWTH_FACTOR);

        speed = BASE_SPEED + Mathf.FloorToInt((lvl - 1) * SPEED_GROWTH_FACTOR);

        goldReward = BASE_GOLD + (GOLD_PER_LEVEL * lvl);
        expReward = Mathf.RoundToInt(BASE_EXP * Mathf.Pow(EXP_GROWTH_FACTOR, lvl - 1));

        energy = Mathf.Clamp(energy, 0, maxEnergy);

        Debug.Log($"[Mushroom] Lv {lvl} Stats | HP {maxHealth} | ATK {attackPower} | DEF {defensePower} | SPD {speed} | EXP {expReward} | Gold {goldReward}");
    }
}