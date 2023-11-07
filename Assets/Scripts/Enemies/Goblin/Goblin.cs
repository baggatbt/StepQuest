using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Goblin : Enemy
{   
    //This is because HP must intialize before the data in Start()
    protected override void Awake()
    {
        base.Awake();
        this.level = 1;
        this.maxHealth = 7;
        this.health = this.maxHealth;
        this.maxEnergy = 1;
        
    }
     protected override void Start()
    {
        // Assign the skills to this specific type of character
        this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
        {
            new GoblinAttackSkill(),
            new GoblinSpecialAttackSkill()
        };
        
        this.expReward = 8;  
        this.goldReward = 3;
        this.attacksBeforeSpecial = 2;
        this.damage = 6;
        this.defensePower = 0; 
        this.speed = 4;
        this.energy = 0;

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

        
    }
}

