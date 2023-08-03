using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    public Slash()
    {
        skillName = "Slash";
        description = "A powerful slashing attack.";
        requiresMovement = true;
        energyCost = 2;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{   
    yield return battleManager.PlayerActiveTimeEvent(0.0f, 1.0f, (result) =>
    {
        user.animator.SetTrigger("Attack1Trigger");
        user.isAttacking = true;

        if (result == TimingEventResult.Perfect)
        {
            target.TakeDamage(user.attackPower);
            Debug.Log("Slash successful! " + target.name + " takes " + user.attackPower + " damage.");
            user.GainEnergy((energyCost / 2));
            user.animator.SetTrigger("AttackFailTrigger");
            
        }
        else if (result == TimingEventResult.Good)
        {
            target.TakeDamage(user.attackPower);
            Debug.Log("Slash successful! " + target.name + " takes " + user.attackPower + " damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;

        }
        else
        {
            target.TakeDamage(0);
            Debug.Log("Slash missed! " + target.name + " takes 0 damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
            
           
        }
    });
   
    
    
   
}
}
