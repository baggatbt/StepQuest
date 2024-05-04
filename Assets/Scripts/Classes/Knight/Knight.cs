using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Companion
{
    protected override void Awake()
    {
        base.Awake();
        // Load the skill tree prefab from the Resources folder
    
        // Initialize with default values if no data is loaded
        if (this.heroLevel == 0)
        {
           // this.level = 1; phasing out, replace with heroLevel
            this.attackPower = 5;
            this.maxHealth = 12;
            this.health = this.maxHealth;
            this.defensePower = 1;
            this.speed = 4;
            this.maxEnergy = 5;
            this.energy = maxEnergy;
            this.defensePenetration = 0;
            this.heroID = "Knight";
            this.heroLevel = 1;
            this.heroExp = 0;
            this.heroSkillPoints = 0;
            this.heroStatPoints = 0;
            this.characterIDNumber = "1";
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
                this.maxHealth += 2; 
                break;
            case 3:
                this.attackPower += 1; 
                break;
            case 4:
                this.maxHealth += 3; 
                break;
            case 5:
                this.attackPower += 1; 
                break;
            case 6:
                this.maxEnergy += 1; 
                break;
            case 7:
                this.maxHealth += 3; 
                break;
            case 8:
                this.attackPower += 1; 
                break;
            case 9:
                this.defensePower += 1; 
                break;
            case 10:
                this.maxHealth += 4; 
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
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        
        
        
       
        
    };

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        SkillType.ReflectDamagePassive,
        SkillType.SpeedBreak,
        
    };

    public override List<SkillType> AllSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.ReflectDamagePassive,
        SkillType.SpeedBreak,
        
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
            
            case SkillType.Taunt:
                return new Taunt();
            
            case SkillType.ReflectDamagePassive:
                return new ReflectDamagePassive();

            case SkillType.SpeedBreak:
                return new SpeedBreak();

          
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}