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
            this.attackPower = 5;
            this.maxHealth = 12;
            this.health = this.maxHealth;
            this.defensePower = 0;
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
            this.heroLevel++;
            this.attackPower += 0;
            this.maxHealth += 0;
            this.health = this.maxHealth;
            this.defensePower += 0;
            this.speed += 0;
            this.maxEnergy += 0;
            this.energy = 0;
            this.heroExp = 0;  
            this.heroStatPoints += 5;
            this.heroSkillPoints += 1;
        } 
    }

    
    

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.ReflectDamagePassive,
        
        
       
        
    };

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        
        
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

          
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}