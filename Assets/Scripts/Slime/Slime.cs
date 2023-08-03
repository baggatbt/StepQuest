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
            // new SlimeSpecialAttackSkill()
        };

        this.expReward = 5;  //TODO: Add scaling
        this.goldReward = 2;
        this.attacksBeforeSpecial = 2;

        // Assign the skills
        this.normalSkill = this.skills[0]; // Normal skill
        // this.specialSkill = this.skills[1]; // Special skill

        // Set the default currentSkill
        this.currentSkill = this.normalSkill;

        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject)
        {
            // Set the attackTarget of the slime to the player's transform
            this.attackTarget = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found. Ensure your player GameObject is tagged 'Player'.");
        }
    }
}
