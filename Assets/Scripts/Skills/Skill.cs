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
    public int requiredLevel;  // New field
    protected TimingEventResult result; 

      // New virtual function for calculating base damage.
    protected virtual int CalculateBaseDamage(Character user)
    {
        return PlayerData.Instance.attackPower;
    }


    

    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);

    public void HandleAoeAttack(Character user, List<Character> enemies, TimingEventResult timingResult, int baseDamage)
{
    float damageMultiplier = 1.0f;
    
    result = timingResult;
    baseDamage = user.attackPower;

    foreach (Character target in enemies)
    {
        if (result == TimingEventResult.Miss)
        {
             target.animator.SetTrigger("IsHurtTrigger");
             AudioManager.instance.PlaySlashSound();
        }
        else if (result == TimingEventResult.Perfect)
        {
            damageMultiplier = 1.5f;  // Boost damage by 50%                      
            target.TakeDamage((int)(baseDamage * damageMultiplier),user); // Apply damage boost
        }
        else 
        {
            damageMultiplier = 1.25f;  // Boost damage by 25%                      
            target.TakeDamage((int)(baseDamage * damageMultiplier),user); // Apply damage boost
        }
        target.CheckForDeath();
    }
}


    public void HandleTimingResultForEnemyAttack(Character user, Character target,TimingEventResult timingResult, int baseDamage)
{
        float damageMultiplier = 1.0f;
        
        result = timingResult;
        baseDamage = user.attackPower;
    
    switch (result)
    {
        case TimingEventResult.Perfect:
            Debug.Log("Perfect Block!");
            damageMultiplier = 0.5f;  // Reduce damage by 50%
          //  user.animator.SetTrigger(trigger);
            target.TakeDamage((int)(user.attackPower * damageMultiplier),user); // Apply damage multiplier
           user.GainEnergy(1);
            target.animator.SetTrigger("BlockTrigger");      
            AudioManager.instance.PlayBlockSound();
            break;
        case TimingEventResult.Good:
            Debug.Log("Good Block!");   
            damageMultiplier = 0.75f;  // Reduce damage by 25%
           // user.animator.SetTrigger(trigger);            
            target.TakeDamage((int)(user.attackPower * damageMultiplier),user); // Apply damage multiplier
           user.GainEnergy(1);
            target.animator.SetTrigger("BlockTrigger");
            AudioManager.instance.PlayBlockSound();     
            break;
        case TimingEventResult.Miss:
          //  user.animator.SetTrigger(trigger);
            target.TakeDamage(user.attackPower, user); // Full damage as there's no reduction 
            user.GainEnergy(1);
            target.animator.SetTrigger("IsHurtTrigger");       
            break;
    }
}
    

    public void HandleTimingResultForPlayerAttack(Character user, Character target, TimingEventResult timingResult, int baseDamage)
{
    float damageMultiplier = 1.0f;
    
    result = timingResult;
    baseDamage = user.attackPower;
    
    switch (result)
    {
        case TimingEventResult.Perfect:
            Debug.Log("Perfect Hit!");
            damageMultiplier = 1.5f;  // Boost damage by 50%
            target.TakeDamage((int)(baseDamage * damageMultiplier), user);
            target.animator.SetTrigger("IsHurtTrigger");
            user.PlayCriticalHitSound(); // Play critical hit sound
            break;
        case TimingEventResult.Good:
            Debug.Log("Good Hit!");
            damageMultiplier = 1.25f;  // Boost damage by 25%
            target.animator.SetTrigger("IsHurtTrigger");
            target.TakeDamage((int)(baseDamage * damageMultiplier), user);
            user.PlayHitSound(); // Play normal hit sound
            break;
        case TimingEventResult.Miss:
            Debug.Log("Missed!");
            target.animator.SetTrigger("IsHurtTrigger");
            target.TakeDamage(baseDamage, user); // No damage boost
            // Optionally play a miss sound or no sound
            break;
    }
}


public void HandlePlayerRangedAttack(Character user, Projectile projectile, TimingEventResult result)
    {
        float damageMultiplier = 1.0f;
        switch (result)
        {
            case TimingEventResult.Perfect:
                damageMultiplier = 1.5f; // Boost damage by 50%
                user.PlayCriticalHitSound(); // Play critical hit sound
                break;
            case TimingEventResult.Good:
                damageMultiplier = 1.25f; // Boost damage by 25%
                user.PlayHitSound();
                break;
            case TimingEventResult.Miss:
                // Optional: Reduce damage or keep as it is
                break;
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
