using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mushroom : Enemy
{
    protected override void Awake()
    {
        base.Awake(); // Calls Enemy.Awake(), ensuring base class initialization, including exp and gold reward calculations
        this.level = 1;
        this.maxHealth = 11;
        this.health = this.maxHealth;
        this.maxEnergy = 3;
        this.speed = 3;
        this.attackPower = 6;
        this.defensePower = 0;
        this.energy = 0;
        this.baseExp = 10; // Base experience points given
        this.growthFactor = 1.1f; // Growth factor to control the steepness
        this.multiplier = 0; // Multiplier for the experience calculation
 
    }

    protected override void Start()
    {
        base.Start(); // Ensure any base class initialization happens

        // Initialize Mushroom-specific skills
        this.skills = new List<Skill>
        {
            new MushroomAttackSkill(),
            new MushroomSpecialAttackSkill()
        };
        
        // Skills assignment
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill
        this.currentSkill = this.normalSkill;
    }

    public override void UpdateStats()
    {
        base.UpdateStats(); // Ensures that any updates that happen in the base class, including expReward and goldReward updates, are applied

        // Apply any Mushroom-specific stat updates
      //  this.maxHealth = 18 + (this.level * 5); // Adjusted to match the original pattern
        this.maxHealth = 11 +(this.level - 1) * 2;
        this.health = this.maxHealth;
        this.expReward = 10 + (this.level * 2);
        this.goldReward = 4 + (this.level * 3); // You can keep or modify this if you want specific gold logic for Mushrooms
        this.attackPower = 6 + (this.level); // Adjusted to match the original pattern
       // this.defensePower = 15 + (this.level + 3); // Adjusted to match the original pattern
        this.energy = 0;
    }
}
