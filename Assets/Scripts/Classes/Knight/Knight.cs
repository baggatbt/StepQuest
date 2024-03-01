using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Companion
{
    protected override void Awake()
    {
        base.Awake();
        
        
        

        // Initialize with default values if no data is loaded
        if (this.heroLevel == 0)
        {
           // this.level = 1;
            this.attackPower = 7;
            this.maxHealth = 20;
            this.health = this.maxHealth;
            this.defensePower = 20;
            this.speed = 4;
            this.maxEnergy = 10;
            this.energy = this.maxEnergy;
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
            this.heroLevel++;
            this.attackPower += 2;
            this.maxHealth += 5;
            this.health = this.maxHealth;
            this.defensePower += 3;
            this.speed += 1;
            this.maxEnergy += 1;
            this.energy = this.maxEnergy;
            this.heroExp = 0;  
            this.heroStatPoints += 3;
            this.heroSkillPoints += 1;
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
            
            case SkillType.Taunt:
                return new Taunt();

          
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}