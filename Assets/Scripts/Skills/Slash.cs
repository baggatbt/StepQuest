using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    public Slash()
    {
        name = "Slash";
        description = "A powerful slashing attack.";
        requiresMovement = true;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{   
    yield return battleManager.PlayerActiveTimeEvent(0.0f, 1.0f, (result) =>
    {
        user.animator.SetTrigger("Attack1Trigger");
        user.isAttacking = true;

        if (result == TimingEventResult.Perfect)
        {
            target.TakeDamage(2);
            Debug.Log("Perfect Slash successful! " + target.name + " takes 2 damage.");
            canChain = true;
            user.animator.SetTrigger("AttackFailTrigger");
            
        }
        else if (result == TimingEventResult.Good)
        {
            target.TakeDamage(2);
            Debug.Log("Slash successful! " + target.name + " takes 2 damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;

        }
        else
        {
            target.TakeDamage(1);
            Debug.Log("Slash missed! " + target.name + " takes 1 damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
           
        }
    });
    yield return new WaitForSeconds(0.5f); //The delay util the player attacks
    
    skillExecutionComplete = true;
   
}
}
