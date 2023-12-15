using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Companion
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.RegisterCompanion(this);
        LoadCharacterData(); // Load saved data

        // Initialize with default values if no data is loaded
        if (this.health <= 0)
        {
            this.level = 1;
            this.attackPower = 9;
            this.maxHealth = 14;
            this.health = this.maxHealth;
            this.defensePower = 6;
            this.speed = 4;
            this.maxEnergy = 10;
            this.energy = this.maxEnergy;
            this.defensePenetration = 0;
            this.heroID = "Wizard";
            this.heroLevel = 1;
            this.heroExp = 0;
        }  
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