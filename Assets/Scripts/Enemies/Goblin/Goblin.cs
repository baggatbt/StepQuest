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
        this.maxHealth = 9;
        this.health = this.maxHealth;
        this.maxEnergy = 1;
        this.speed = 9;
        this.defensePenetration = 2;
        this.attacksBeforeSpecial = 2;
        this.attackPower = 6 ;
        this.defensePower = 5;
        this.energy = 0;
        
    }
     protected override void Start()
    {
        // Assign the skills to this specific type of character
        this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
        {
            new GoblinAttackSkill(),
            new GoblinSpecialAttackSkill()
        };
        
        this.expReward = 8 + (this.level * 2);
        this.goldReward = 3 + (this.level * 2);
        

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;
    }


   public override void UpdateStats()
    {
        this.maxHealth = 9 + (this.level * 2);
        this.health = maxHealth;
        this.expReward = 8 + (this.level * 2);
        this.goldReward = 3 + (this.level * 2);
        this.attacksBeforeSpecial = 2;
        this.attackPower = 6 + (this.level + 2);
        this.defensePower = 5 + (this.level + 1);
        this.energy = 0;
    }
    

   
}

