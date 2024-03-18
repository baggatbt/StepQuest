using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct AttackStage
{
    public string animationTrigger;
    public float timingWindowStart;
    public float timingWindowEnd;
    public int damage;
}

public enum SkillType
    {
        None,
        Slash,
        TripleHit,
        SwordWave,
        GuardSkill,
        ShieldSlam,
        SlimeCompanionBasicAttack,
        WizardBasicAttack,
        FirePillar,
        Taunt,
        ShootArrow,
        ArrowRain,
        MeleeCombo,
    }



public abstract class Skill
{
    public string skillName;
    public string description;
    public bool skillExecutionComplete; // Flag to track the completion of skill execution
    public bool requiresMovement; // Indicates if the attack requires movement towards the target
//got rid of can hit behind
    public int energyCost;
    public int energyGain;
    public int skillLevel;
    public bool noZoom; //Allows zoom to be disabled
    public int requiredLevel;  // New field\
    public Sprite iconImage; // Field to store the icon image associated with the skill
    public float skillDamageModifier;
    
    protected TimingEventResult result; 

      // New virtual function for calculating base damage.
    protected virtual int CalculateBaseDamage(Character user)
    {
        return PlayerData.Instance.attackPower;
    }
    public Sprite LoadIconImage(string path)
    {
        return Resources.Load<Sprite>(path);
    }


    

    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);

    public void HandleAoeAttack(Character user, List<Character> enemies, TimingEventResult timingResult, int baseDamage)
{
    float damageTimingMultiplier = 1.0f;
    
    result = timingResult;
    int skillBaseDamage = baseDamage;

    foreach (Character target in enemies)
    {
        if (result == TimingEventResult.Good)
        {
             target.animator.SetTrigger("IsHurtTrigger");
             damageTimingMultiplier = 1.25f;  // Boost damage by 25%                      
             target.TakeDamage((int)(skillBaseDamage * damageTimingMultiplier),user); // Apply damage boost
             user.PlayCriticalHitSound(); // Play critical hit sound
        }
        else 
        {
                                  
            target.TakeDamage((int)skillBaseDamage,user); // Apply damage boost
        }
       
    }
}


    public void HandleTimingResultForEnemyAttack(Character user, Character target,TimingEventResult timingResult, int baseDamage)
{
        float damageMultiplier = 1.0f;
        
        result = timingResult;
        baseDamage = user.attackPower;
    
    if (result == TimingEventResult.Good)
    {
            Debug.Log("Good Block!");   
            damageMultiplier = 0.75f;  // Reduce damage by 25%
           // user.animator.SetTrigger(trigger);            
            target.TakeDamage((int)(user.attackPower * damageMultiplier),user); // Apply damage multiplier
            user.GainEnergy(1);
            target.animator.SetTrigger("BlockTrigger");
            AudioManager.instance.PlayBlockSound();       
    }
    else 
    {
            target.TakeDamage((int)(user.attackPower * damageMultiplier),user); // Apply damage multiplier
            user.GainEnergy(1);
            target.animator.SetTrigger("IsHurtTrigger");
            AudioManager.instance.PlayBlockSound();      
    }
}
    

    public void HandleTimingResultForPlayerAttack(Character user, Character target, TimingEventResult timingResult, int baseDamage)
{
    float damageMultiplier = 1.0f;
    
    result = timingResult;
    baseDamage = baseDamage;
    
    if (result == TimingEventResult.Good)
    {
            Debug.Log("Good Hit!");
            damageMultiplier = 1.25f;  // Boost damage by 25%
            target.TakeDamage((int)(baseDamage * damageMultiplier), user);
            target.animator.SetTrigger("IsHurtTrigger");
            user.PlayCriticalHitSound(); // Play critical hit sound
           
    }
    else 
    {
            target.TakeDamage((int)(baseDamage * damageMultiplier), user);
            target.animator.SetTrigger("IsHurtTrigger");
            user.PlayHitSound(); // Play  hit sound
    }
}


public void HandlePlayerRangedAttack(Character user, Projectile projectile, TimingEventResult result)
    {
        float damageMultiplier = 1.0f;
        if (result == TimingEventResult.Good)
        {
            
                damageMultiplier = 1.25f; // Boost damage by 25%
                user.PlayHitSound();   
        }
        else 
        {
            user.PlayHitSound();
        }

        projectile.damage = (int)(projectile.damage * damageMultiplier);
    }



    public int GetSkillLevel()
    {
        // Check if the player's data contains a level for this skill
        if (PlayerData.Instance.skillLevels.TryGetValue(skillName, out int level))
        {
            // If it does, return that level
            return level;
        }
        else
        {
            // If it doesn't, return a default level 
            return 1;
        }
    }
}
