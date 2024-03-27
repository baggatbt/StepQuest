using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Goblin : Enemy
{   
    protected override void Awake()
    {
        base.Awake(); // Calls Enemy.Awake(), which now includes the call to UpdateExpAndGoldRewards
        this.level = 1;
        this.maxHealth = 21;
        this.health = this.maxHealth;
        this.maxEnergy = 1;
        this.speed = 7;
        this.defensePenetration = 0;
        this.attacksBeforeSpecial = 2;
        this.attackPower = 8;
        this.defensePower = 5;
        this.energy = 0;
    }

    protected override void Start()
    {
        base.Start(); // Ensure any base class initialization happens. Currently, Enemy.Start() might not exist or do anything, but it's good practice.

        // Initialize Goblin-specific skills
        this.skills = new List<Skill>
        {
            new GoblinAttackSkill(),
            new GoblinSpecialAttackSkill()
        };
        
        // Skills assignment
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill
        this.currentSkill = this.normalSkill;
    }

    public override void UpdateStats()
    {
        base.UpdateStats(); // Ensures that any updates that happen in the base class, including expReward and goldReward updates, are applied

        // Goblin-specific updates
       // this.maxHealth = 9 + (this.level * 2);
        this.health = this.maxHealth;
        this.expReward = 10 + (this.level * 2);
        this.goldReward = 3 + (this.level * 2); // You can keep or modify this if you want specific gold logic for Goblins
        this.attacksBeforeSpecial = 2;
      //  this.attackPower = 6 + (this.level * 2); 
        this.defensePower = 5 + (this.level + 1);
        this.energy = 0;
    }
}
