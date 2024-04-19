using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Companion
{
    protected override void Awake()
    {
        base.Awake();
        
    
        // Initialize with default values if no data is loaded
        if (this.heroLevel == 0)
        {
          //  this.level = 1;
            this.attackPower = 8;
            this.maxHealth = 14;
            this.health = this.maxHealth;
            this.defensePower = 0;
            this.speed = 10;
            this.maxEnergy = 6;
            this.energy = this.maxEnergy;
            this.defensePenetration = 0;
            this.heroID = "Archer";
            this.heroLevel = 1;
            this.heroExp = 0;
            this.heroSkillPoints = 0;
            this.heroStatPoints = 0;
            this.characterIDNumber = "2";
            this.stamina = 10;
        }
        else
        {
        LoadCharacterData(); // Load saved data
        }
    }

    

    public override void LevelUp()
{
    if (this.heroExp >= ExpToNextLevel(this.heroLevel))
    {
        this.heroExp -= ExpToNextLevel(this.heroLevel); // Handles exp past the required amount
        this.heroLevel++;
        

        switch (this.heroLevel)
        {
            case 2:
                this.attackPower += 1; 
                break;
            case 3:
                this.maxHealth += 2; 
                break;
            case 4:
                this.attackPower += 3; 
                break;
            case 5:
                this.attackPower += 1; 
                break;
            case 6:
                this.maxEnergy += 2; 
                break;
            case 7:
                this.maxHealth += 3; 
                break;
            case 8:
                this.speed += 1;
                break;
            case 9:
                this.attackPower += 2; 
                break;
            case 10:
                this.maxHealth += 2; 
                break;
            default:
                break; //TODO: extend this later when I have concrete balance plans
        }

        // Update current health and energy to new max values
        this.health = this.maxHealth;
        this.energy = this.maxEnergy;

        this.heroStatPoints += 1; // Every 3 levels will allow player to upgrade one stat of their choice +1
        this.heroSkillPoints += 1; //TODO: Implement skill unlocking

        Debug.Log("Hero leveled up to level " + this.heroLevel + ", stat upgraded.");
    }
}

    
    

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.ShootArrow,
        SkillType.ArrowRain,
        SkillType.MeleeCombo,
        
        
       
        
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

            case SkillType.ArrowRain:
                return new ArrowRain();

            case SkillType.MeleeCombo:
                return new MeleeCombo();
            // ... other cases ..
            
        
            default:
                Debug.LogError("Unknown skill type for Archer: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}