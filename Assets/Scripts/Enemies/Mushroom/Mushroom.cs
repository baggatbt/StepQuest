using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mushroom : Enemy
{
    private const int BASE_HEALTH = 18;
    private const int HEALTH_GROWTH = 4;

    private const int BASE_ATTACK = 3;
    private const float ATTACK_GROWTH_FACTOR = 1.5f;

    private const int BASE_GOLD = 7;
    private const int GOLD_PER_LEVEL = 2;

    private const int BASE_EXP = 12;
    private const float EXP_GROWTH_FACTOR = 1.1f;

    protected override void Awake()
    {
        base.Awake();
        this.level = 1;
        UpdateStats();

        this.speed = 7;
        this.maxEnergy = 1;
        this.energy = 0;
        this.defensePower = 0;
        this.defensePenetration = 0;
        this.attacksBeforeSpecial = 2;

        this.skills = new List<Skill>
        {
            new MushroomAttackSkill(),
            new MushroomSpecialAttackSkill()
        };

        this.normalSkill = skills[0];
        this.specialSkill = skills[1];
        this.currentSkill = normalSkill;

        this.baseExp = BASE_EXP;
        this.growthFactor = EXP_GROWTH_FACTOR;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void UpdateStats()
    {
        base.UpdateStats();

        this.maxHealth = BASE_HEALTH + (this.level - 1) * HEALTH_GROWTH;
        this.health = this.maxHealth;

        this.attackPower = BASE_ATTACK + Mathf.FloorToInt((this.level - 1) * ATTACK_GROWTH_FACTOR);

        this.goldReward = BASE_GOLD + GOLD_PER_LEVEL * this.level;
        this.expReward = Mathf.RoundToInt(BASE_EXP * Mathf.Pow(EXP_GROWTH_FACTOR, this.level - 1));
    }
}
