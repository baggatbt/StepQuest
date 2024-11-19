using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



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
        ReflectDamagePassive,
        SpeedBreak,
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
     public bool isActiveSkill = true; //Determines whether the skill as active or passive
    public int requiredLevel;  // New field\
    public Sprite iconImage; // Field to store the icon image associated with the skill
    public float skillDamageModifier;
    public int skillPointCost;
    
    protected TimingEventResult result; 

      // New virtual function for calculating base damage.
    protected virtual int CalculateBaseDamage(Character user)
    {
        return 5;
    }
    public Sprite LoadIconImage(string path)
    {
        return Resources.Load<Sprite>(path);
    }

    


    

    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);

    public virtual void ApplyPassiveEffect(CharacterData characterData)
    {
        // Default implementation can be empty
    }
    

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




     public void HandleTimingResultForEnemyAttack(Character user, Character target, TimingEventResult timingResult, int baseDamage)
    {

        if (user.damageApplied)
    {
        // If damage has already been applied, skip further processing
        return;
    }

        float damageTimingMultiplier = 1.0f;
        int skillBaseDamage = baseDamage;
        int finalDamage;
        
        result = timingResult;
        
        //finalDamage = Math.Max(finalDamage, 1);
        // Gain energy and set animations based on the result
        user.GainEnergy(1);
        
        if (result == TimingEventResult.Good)
        {
            target.didBlock = true;
            damageTimingMultiplier = 0.75f;
            finalDamage = (int)(skillBaseDamage * damageTimingMultiplier);
            target.TakeDamage(finalDamage,user);
             
            target.animator.SetTrigger("BlockTrigger");
            AudioManager.instance.PlayBlockSound();

            //Calculate any reflection if necessary
            int finalReflectedDamage = CalculateReflectDamage(target, finalDamage);
            Debug.Log(finalReflectedDamage);
            if (finalReflectedDamage > 0)
            {
            user.TakeDamage(finalReflectedDamage, target);
            }
            target.didBlock = false;
            
        }
        else 
        {
            target.didBlock = false;
            damageTimingMultiplier = 1.0f;
            finalDamage = skillBaseDamage;
            
            target.TakeDamage((int)skillBaseDamage,user); 
            user.PlayHitSound();
            
           // AudioManager.instance.PlayBlockSound();
        }

        // Mark that damage has been applied to prevent further instances
    user.damageApplied = true;
    }

    private int CalculateReflectDamage(Character user, int damage)
{
    double reflectedDamage = user.damageReflectionPercentage * damage;
    int finalReflectedDamage = Convert.ToInt32(Math.Round(reflectedDamage));
    return finalReflectedDamage; // Return the calculated damage to reflect
}



    

    public void HandleTimingResultForPlayerAttack(Character user, Character target, TimingEventResult timingResult, int baseDamage)
{
    if (user.damageApplied)
    {
        // If damage has already been applied, skip further processing
        return;
    }

    float damageTimingMultiplier = 1.0f;
    float skillBaseDamage = baseDamage; // Use float for baseDamage to allow for fractional multipliers
    int finalDamage;

    if (timingResult == TimingEventResult.Good)
    {
        Debug.Log("Good Hit!");
        damageTimingMultiplier = 1.25f;  // Boost damage by 25%

        // Calculate final damage and round up
        finalDamage = Mathf.CeilToInt(skillBaseDamage * damageTimingMultiplier);
        target.TakeDamage(finalDamage, user);
        user.GainEnergy(1); // Bonus energy for a good hit
        user.PlayCriticalHitSound(); // Play critical hit sound
    }
    else
    {
        damageTimingMultiplier = 1.0f;  // Base damage
        finalDamage = baseDamage; // No need to round since no multiplier
        target.TakeDamage(finalDamage, user);
        user.PlayHitSound(); // Play hit sound
    }

    // Mark that damage has been applied to prevent further instances
    user.damageApplied = true;
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

        projectile.damage = Mathf.CeilToInt(projectile.damage * damageMultiplier);
    }



   
}
