using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    
 
    //This is because HP must intialize before the data in Start()
    protected override void Awake()
    {
        base.Awake();
        this.level = 1;
        this.maxHealth = 10;
        this.health = this.maxHealth;
        this.maxEnergy = 3;
        this.speed = 3;
        this.defensePower = 1;
        this.attackPower = 2;
        this.attacksBeforeSpecial = 2;
        
    }
    protected override void Start()
    {
        // Assign the skills to this specific type of character
        this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
        {
            new SlimeAttackSkill(),
            new SlimeSpecialAttackSkill()
        };
        
        this.expReward = 5;
        this.goldReward = 2;
        this.attacksBeforeSpecial = 2;
        
       
       

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

    
    }
}
