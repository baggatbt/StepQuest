using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : Character
{
   protected override void Start()
    {
        this.level = 1;
        this.attackPower = 5;
        this.maxHealth = 10;
        this.health = this.maxHealth;
        this.defensePower = 1;
        this.speed = 2;
        this.maxEnergy = 0;
        this.energy = maxEnergy;

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
            case SkillType.Slash:
                return new Slash();

            case SkillType.TripleHit:
                return new TripleHitSkill();

            case SkillType.SwordWave:
                return new SwordWave();

            case SkillType.ShieldSlam:
                return new ShieldSlam();

            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
    // Additional companion-specific properties and behavior
}
