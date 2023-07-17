using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    // Constructor
    public Slime()
    {
        // Assign the skills to this specific type of character
        this.skills.Add(new SlimeAttackSkill());

       // this.skills.Add(new SlimeSpecialAttackSkill()); // New skill i need to implement
        
       this.expReward = 5;  //TODO: Add scaling
       this.goldReward = 2;
       this.attacksBeforeSpecial = 2;
    }

    private void Awake()
{
    // Assign the skills to this specific type of character
    this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
    {
        new SlimeAttackSkill(),
       // new SlimeSpecialAttackSkill()
    };

    this.attacksBeforeSpecial = 2;

     // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
       // this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;
}

    
}
