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