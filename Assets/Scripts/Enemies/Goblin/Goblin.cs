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
        this.maxHealth = 8;
        this.health = this.maxHealth;
        this.maxEnergy = 1;
        this.speed = 7;
        this.defensePenetration = 0;
        this.attacksBeforeSpecial = 2;
        this.attackPower = 5;
        this.defensePower = 0;
        this.energy = 0;
        this.baseExp = 6; // Base experience points given by goblins
        this.growthFactor = 1.1f; // Growth factor to control the steepness
        this.multiplier = 0; // Multiplier for the experience calculation
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
        this.maxHealth = 8 +(this.level - 1) * 2;
        this.health = this.maxHealth;

        // Calculate expReward based on the specific multiplier (2 in this case)
        this.expReward = ExpGivenByEnemy(this.level);
        
        this.goldReward = 3 + (this.level * 2); // You can keep or modify this if you want specific gold logic for Goblins
        this.attacksBeforeSpecial = 2;
        this.attackPower = 4 + ((this.level - 1) / 2) + 1 ;
        //this.defensePower = 0 + (this.level + 1);
        this.energy = 0;
    }
   
}
