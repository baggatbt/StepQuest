using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : Skill
{
 
    public Taunt()
    {
        skillName = "Taunt";
        description = "Forces enemies to target this character";
        energyCost = 2; 
        energyGain = 0;
        skillLevel = 1; 
        requiresMovement = false; 
        skillExecutionComplete = false;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        int baseDamage = 0;
        user.isAttacking = true;
        user.isAnimationDone = false;
        HandleAoeAttack(user, battleManager.enemies, result, baseDamage);
        Debug.Log("Taunt skill applied");
        user.isAttacking = false;
        user.animator.SetTrigger("GuardTrigger");
        yield return new WaitUntil(() => user.isAnimationDone == true);
        
    }
    
}
