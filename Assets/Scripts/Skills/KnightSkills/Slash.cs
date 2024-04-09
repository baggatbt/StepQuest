using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    private int numberOfAttacks;
   // GameObject particleEffectPrefab = Resources.Load<GameObject>("PreFab/Projectiles_Effects/skillAttack2");
    public Slash()
    {
        skillName = "Slash";
        description = "Tap at the right time for extra damage";
        energyCost = 0; 
        energyGain = 1;
        skillLevel = 1; 
        requiresMovement = true; 
        skillExecutionComplete = false;
        numberOfAttacks = 1;
        iconImage = LoadIconImage("SkillIcons/weapon1");
        
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return user.attackPower; // 100% of user's attack power
    }

     public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;  // Reset the flag at the start of each attack



        int baseDamage = CalculateBaseDamage(user);

        user.animator.SetTrigger("Attack1Trigger");
        for( int i = 0; i < numberOfAttacks; i++)
        {
             

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.6f);
             //yield return battleManager.StartCoroutine(battleManager.TimeStop(0.1f));
             // Vector3 spawnPosition = target.transform.position;
           // Vector3 offsetPosition = new Vector3(spawnPosition.x, spawnPosition.y + 2, spawnPosition.z);
            // GameObject slashEffect = UnityEngine.Object.Instantiate(particleEffectPrefab, offsetPosition, Quaternion.identity);

             HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
             
             Debug.Log("Timing for player attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             yield return new WaitUntil(() => user.animationDamageTime == true);
             
             
        }
        Debug.Log("waiting on animation to finish");
    
        yield return new WaitUntil(() => user.isAnimationDone == true);
        user.GainEnergy(EnergyGain());
       // target.CheckForDeath();
        user.animationDamageTime = false;
        user.isAnimationDone = false;
        user.isAttacking = false;
        target.CheckForDeath();
    }

    private int EnergyGain()
    {
        if (result == TimingEventResult.Good)
        {
            Debug.Log("2 energy");
            return 2;
        }
        else
        {
            Debug.Log("1 energy");
            return 1;
        }
    }


    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
       
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
            
        }));
         
    }


}