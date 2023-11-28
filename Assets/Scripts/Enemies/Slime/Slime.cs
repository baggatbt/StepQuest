using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    protected override void Start()
    {
        // Assign the skills to this specific type of character
        this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
        {
            new SlimeAttackSkill(),
            new SlimeSpecialAttackSkill()
        };
        this.level = 1;
        PreferredAttackPosition = BattlePosition.Front;
        this.expReward = 5;
        this.goldReward = 2;
        this.attacksBeforeSpecial = 2;
        this.damage = 2;
        this.maxHealth = 10;
        this.health = this.maxHealth;
        this.defensePower = 1;
        this.speed = 2;
        this.maxEnergy = 2;

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

    
    }
}
