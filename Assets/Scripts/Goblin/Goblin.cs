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
        this.maxHealth = 22;
        this.health = this.maxHealth;
        
    }
     protected override void Start()
    {
        // Assign the skills to this specific type of character
        this.skills = new List<Skill> // Make sure to initialize the skills list before adding to it
        {
            new GoblinAttackSkill(),
            //new GoblinSpecialAttackSkill()
        };
        
        this.expReward = 8;  //TODO: Add scaling
        this.goldReward = 3;
        this.attacksBeforeSpecial = 2;
        this.damage = Mathf.RoundToInt(2 * Mathf.Pow(1.10f, level - 1)); //Increase by 10% each level, controlled by 1.10f
        this.defensePower = 2;

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        // this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject)
        {
            // Set the attackTarget of the goblin to the player's transform object
            this.attackTarget = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found. Ensure your player GameObject is tagged 'Player'.");
        }
    }
}

