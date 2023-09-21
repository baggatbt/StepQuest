using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    private int numberOfAttacks;
    public Slash()
    {
        skillName = "Slash";
        description = "A swift slash that deals damage equal to the user's attack power.";
        energyCost = 0; 
        skillLevel = 1; 
        requiresMovement = true; 
        skillExecutionComplete = false;
        numberOfAttacks = 2;
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return user.attackPower; // 100% of user's attack power
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        skillExecutionComplete = false;
        int baseDamage = CalculateBaseDamage(user);
        user.isAttacking = true;
        
        for (int i = 0; i < numberOfAttacks; i++)
        {

        
        yield return battleManager.PlayerActiveTimeEvent(0.0f, 0.7f, (result) =>
        {
             HandleTimingResultForPlayerAttack(user, target, /*"Attack1Trigger" ,*/ result, baseDamage);
             
           
        });
        }
        yield return new WaitForSeconds(.5f);
        user.isAttacking = false;
        

      

        skillExecutionComplete = true; // Mark the skill as executed once completed
    }
}

       
      
   
    
