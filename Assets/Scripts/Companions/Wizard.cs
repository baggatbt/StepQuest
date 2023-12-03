using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Character
{
   protected override void Start()
    {
        this.level = 1;
        this.attackPower = 3;
        this.maxHealth = 8;
        this.health = this.maxHealth;
        this.defensePower = 0;
        this.speed = 2;
        this.maxEnergy = 5;
        this.teamEnergy = PlayerData.Instance.teamEnergy;

    }
   
    public  List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.SlimeCompanionBasicAttack,
        
        
    };

    public  List<SkillType> LockedSkills => new List<SkillType>
    {
        
    };

    public  Skill GetSkillInstance(SkillType skillType)
    {
        switch(skillType)
        {
            case SkillType.SlimeCompanionBasicAttack:
                return new SlimeCompanionBasicAttack();
            

            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}
