using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Companion
{
    protected override void Awake()
    {
        base.Awake();
        atkGrowth = 1;
        healthGrowth = 3;
        energyGrowth = 1;
       
        
        

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
            this.heroExp = 0; // Handle overflow exp
            this.heroStatPoints += 5;
            this.heroSkillPoints += 1;
            
            // Stat increases by level
            if (this.heroLevel % 2 == 0) this.attackPower += 1; // Every 2 levels increase attack power
            if (this.heroLevel % 3 == 0) this.maxHealth += 3; // Every 3 levels increase health by 3
            else this.maxHealth += 2; // Other levels increase health by 2
            if (this.heroLevel == 5 || this.heroLevel == 10) this.maxEnergy += 1; // Levels 5 and 10 increase energy
            

            //TO IMPLEMENT LATER
            //As the levels go higher, the growth rates will rise to match.
            //Example: past 20, it will be 2 attack power ever 2 levels
            
            this.health = this.maxHealth; // Refresh health to new max
            this.energy = this.maxEnergy; // Refresh energy to new max
            
            Debug.Log("Hero leveled up, now level: " + this.heroLevel);
        }
    }

    
    

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.ReflectDamagePassive,
        SkillType.SpeedBreak,
        
        
       
        
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

            case SkillType.SpeedBreak:
                return new SpeedBreak();

          
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}