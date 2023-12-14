using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Companion
{
    protected override void Awake()
    {
        base.Awake();
        this.level = 1;
        this.attackPower = 7;
        this.maxHealth = 20;
        this.health = this.maxHealth;
        this.defensePower = 1;
        this.speed = 4;
        this.maxEnergy = 10;
        this.energy = this.maxEnergy;
        this.defensePenetration = 1;
        this.heroID = "Knight";
        
    }
    

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
       
        
    };

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        // ... skills
    };

    public override Skill GetSkillInstance(SkillType skillType)
    {
        switch(skillType)
        {
            case SkillType.Slash:
                return new Slash();
            // ... other cases ..
            
            case SkillType.TripleHit:
                return new TripleHitSkill();
          
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}