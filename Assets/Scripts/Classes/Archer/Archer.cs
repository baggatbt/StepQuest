using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Companion
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.RegisterCompanion(this);
        Debug.Log("Archer Companion registered");
        LoadCharacterData(); // Load saved data

        // Initialize with default values if no data is loaded
        if (this.level == 0)
        {
            this.level = 1;
            this.attackPower = 8;
            this.maxHealth = 14;
            this.health = this.maxHealth;
            this.defensePower = 12;
            this.speed = 10;
            this.maxEnergy = 6;
            this.energy = this.maxEnergy;
            this.defensePenetration = 0;
            this.heroID = "Archer";
            this.heroLevel = 1;
            this.heroExp = 0;
            this.heroSkillPoints = 0;
            this.heroStatPoints = 0;
        }
    }

    

    public override void LevelUp()
    {
        if (this.heroExp >= ExpToNextLevel(this.heroLevel))
        {
            this.heroLevel++;
            this.attackPower += 4;
            this.maxHealth += 2;
            this.health = this.maxHealth;
            this.defensePower += 1;
            this.speed += 3;
            this.maxEnergy += 1;
            this.energy = this.maxEnergy;
            this.heroExp = 0;  
            this.heroStatPoints += 3;
            this.heroSkillPoints += 1;
        } 
    }

    
    

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.ShootArrow,
        
        
       
        
    };

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        // ... skills
    };

    public override Skill GetSkillInstance(SkillType skillType)
    {
        switch(skillType)
        {
            case SkillType.ShootArrow:
                return new ShootArrow();
            // ... other cases ..
            
        
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}