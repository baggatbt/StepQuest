using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Slime : Enemy
{
    // ─────────── Tunables / Growth (mirrors your Goblin pattern)
    private const int BASE_HEALTH = 9;
    private const int HEALTH_GROWTH = 3;

    private const int BASE_ATTACK = 6;      // damage baseline; adjust to your balance
    private const float ATTACK_GROWTH_FACTOR = 1.0f;   // +1 per level, tweak as needed

    private const int BASE_GOLD = 2;
    private const int GOLD_PER_LEVEL = 2;

    private const int BASE_EXP = 5;
    private const float EXP_GROWTH_FACTOR = 1.1f;

    protected override void Awake()
    {
        base.Awake();

        this.level = 1;           // set spawn level (BattleManager can override later)
        UpdateStats();

        // Movement/energy/etc. (like your Goblin)
        this.speed = 3;
        this.maxEnergy = 1;
        this.energy = 0;
        this.defensePower = 0;
        this.defensePenetration = 0;

        // How many basics before special
        this.attacksBeforeSpecial = 2;

        // Skills list (normal + special)
        this.skills = new List<Skill>
        {
            new SlimeAttackSkill(),
            new SlimeSpecialAttackSkill()
        };

        this.normalSkill = skills[0];
        this.specialSkill = skills[1];
        this.currentSkill = normalSkill;

        // EXP growth parameters on Enemy base (if you use them)
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

        // Health scales like Goblin’s approach
        this.maxHealth = BASE_HEALTH + (this.level - 1) * HEALTH_GROWTH;
        this.health = this.maxHealth;

        // Attack scales (linear-ish, tweak factor above)
        this.attackPower = BASE_ATTACK + Mathf.FloorToInt((this.level - 1) * ATTACK_GROWTH_FACTOR);

        // Rewards
        this.goldReward = BASE_GOLD + GOLD_PER_LEVEL * this.level;
        this.expReward = Mathf.RoundToInt(BASE_EXP * Mathf.Pow(EXP_GROWTH_FACTOR, this.level - 1));
    }
}
