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
        this.maxHealth = 18;
        this.health = this.maxHealth;
        this.maxEnergy = 3;
        this.speed = 3;
        this.attackPower = 8;
        this.defensePower = 15; 
        this.energy = 0;
        
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
        

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

    
    }
        public override void UpdateStats()
    {
        this.maxHealth = 9 + (this.level * 5);
        this.health = maxHealth;
        this.expReward = 8 + (this.level * 3);
        this.goldReward = 3 + (this.level * 3);
        this.attacksBeforeSpecial = 2;
        this.attackPower = 6 + (this.level + 1);
        this.defensePower = 5 + (this.level + 3);
        this.energy = 0;
    }
}

