using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Mushroom : Enemy
{   
    //This is because HP must intialize before the data in Start()
    protected override void Awake()
    {
        base.Awake();
        this.level = 1;
        this.maxHealth = 12;
        this.health = this.maxHealth;
        this.maxEnergy = 3;
        this.speed = 5;
        
    }
     protected override void Start()
    {
        // Assign the skills to this specific type of character
        this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
        {
            new MushroomAttackSkill(),
            new MushroomSpecialAttackSkill()
        };
        
        this.expReward = 10;  
        this.goldReward = 4;
        this.attacksBeforeSpecial = 3;
        this.damage = 2;
        this.defensePower = 0; 
        this.energy = 0;

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

        
    }
}

