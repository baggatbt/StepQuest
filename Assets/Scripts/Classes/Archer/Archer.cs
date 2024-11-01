using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Companion
{
    protected override void Awake()
    {
        base.Awake();
        skillOne = SkillType.MeleeCombo;
        skillTwo = SkillType.ArrowRain;
        // Assign specific paths for Knight icons
        if (characterData != null)
        {
          
        }
         InitializeSkillsBasedOnLevel();
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();
        availableSkills.Add(SkillType.ShootArrow);
        //testing methods

      //  if (this.heroLevel >= 2) availableSkills.Add(SkillType.TripleHit);
      //  if (this.heroLevel >= 5) availableSkills.Add(SkillType.Taunt);
        Debug.Log($"Total Available skills: {availableSkills.Count}");
    }

    public void UpdateStats()
    {

        
        this.maxHealth = 8 + (this.level - 1) * 2;
        this.health = this.maxHealth;

        
        
       
        this.attackPower = 5 + ((this.level - 1) / 2);
        
        
    }

    

    public override void LevelUp()
    {
        if (this.heroExp >= ExpToNextLevel(this.heroLevel))
        {
            this.heroExp -= ExpToNextLevel(this.heroLevel);
            this.heroLevel++;
            UpdateStats();

            this.health = this.maxHealth;
            this.energy = this.maxEnergy;

            this.heroStatPoints += 1;
            this.heroSkillPoints += 1;

            Debug.Log("Hero leveled up to level " + this.heroLevel + ", stat upgraded.");
        }
    }

    

    
          
      

    
      
    

    
    
    public override List<SkillType> AvailableSkills => availableSkills;
    

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        SkillType.ArrowRain,
        SkillType.MeleeCombo,
    };
    public override List<SkillType> AllSkills => new List<SkillType>
    {
        SkillType.ShootArrow,
       SkillType.ArrowRain,
       SkillType.MeleeCombo,
       
        
        
        
    };

    private List<SkillType> mainSkills = new List<SkillType>()
    {
       SkillType.ShootArrow,
       SkillType.ArrowRain,
       SkillType.MeleeCombo,
       
    };

     public override List<SkillType> MainSkills => mainSkills;

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