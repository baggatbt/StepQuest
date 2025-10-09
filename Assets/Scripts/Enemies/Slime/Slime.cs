using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Slime : Enemy
{
    private const int BASE_HEALTH = 12;
    private const int HEALTH_GROWTH = 3;

    private const int BASE_ATTACK = 6;
    private const float ATTACK_GROWTH_FACTOR = 1.5f;

    private const int BASE_GOLD = 5;
    private const int GOLD_PER_LEVEL = 2;

    private const int BASE_EXP = 10;
    private const float EXP_GROWTH_FACTOR = 1.1f;

    protected override void Awake()
    {
        base.Awake();
        level = 1; UpdateStats();

        speed = 5;
        maxEnergy = 1; energy = 0;
        defensePower = 0; defensePenetration = 0;
        attacksBeforeSpecial = 3;

        var pounce = new MeleeContactSkill
        {
            skillName = "Pounce",
            description = "Hop onto the target.",
            motion = MotionStyle.HopArc,
            arcHeight = 0.65f,
            inTime = 0.18f,
            outTime = 0.16f,
            overlapX = 0.16f,
            sortingBump = 8,
            dmgMultiplier = 1.0f,
            attackTrigger = "Attack1Trigger"
        };

        skills = new List<Skill> { pounce };
        normalSkill = skills[0];
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
