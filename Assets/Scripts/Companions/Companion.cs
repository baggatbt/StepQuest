using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : Character
{
   protected override void Start()
    {
        // Assign the skills to this specific type of character
        
        this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
        {
             new SlimeAttackSkill() //Dont forget to put the comma back after adding new ones
            //new SomeOtherAttackSkill()
        };
        this.level = 1;
        this.damage = 2;
        this.maxHealth = 10;
        this.health = this.maxHealth;
        this.defensePower = 1;
        this.speed = 2;
        this.maxEnergy = 5;
        this.energy = maxEnergy;

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

    
    }
   

    // Additional companion-specific properties and behavior
}
