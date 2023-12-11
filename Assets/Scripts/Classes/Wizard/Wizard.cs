using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Companion
{
    protected override void Awake()
    {
        base.Awake();
        this.level = 1;
        this.attackPower = 3;
        this.maxHealth = 8;
        this.health = this.maxHealth;
        this.defensePower = 0;
        this.speed = 6;
        this.maxEnergy = 10;
        this.energy = this.maxEnergy;
        
    }
    

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.WizardBasicAttack,
        SkillType.FirePillar,
        // ... other skills ...
    };

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        // ... skills
    };

    public override Skill GetSkillInstance(SkillType skillType)
    {
        switch(skillType)
        {
            case SkillType.WizardBasicAttack:
                return new WizardBasicAttack();
            // ... other cases ..
            
            case SkillType.FirePillar:
                return new FirePillar();
          
            default:
                Debug.LogError("Unknown skill type for Wizard: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}