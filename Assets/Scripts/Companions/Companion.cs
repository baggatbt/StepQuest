using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : Character
{
   protected override void Start()
    {
        this.level = 1;
        this.damage = 2;
        this.maxHealth = 10;
        this.health = this.maxHealth;
        this.defensePower = 1;
        this.speed = 2;
        this.maxEnergy = 5;
        this.energy = maxEnergy;

    }
   
    public  List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.SlimeAttackSkill,
        
        
    };

    public  List<SkillType> LockedSkills => new List<SkillType>
    {
        
    };

    public  Skill GetSkillInstance(SkillType skillType)
    {
        switch(skillType)
        {
            case SkillType.SlimeAttackSkill:
                return new SlimeAttackSkill();
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
