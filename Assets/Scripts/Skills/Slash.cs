using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    public Slash()
    {
        skillName = "Slash";
        description = "A swift slash that deals damage equal to the user's attack power.";
        energyCost = 5; // You can set an appropriate energy cost
        skillLevel = 1; // Setting default level, this might get overridden by GetSkillLevel()
        requiresMovement = true; // Assuming the skill requires the user to move towards the target
        skillExecutionComplete = false;
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return user.attackPower; // 100% of user's attack power
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        skillExecutionComplete = false;
        int baseDamage = CalculateBaseDamage(user);

        // Let's assume you have a method in BattleManager for handling timing events
        yield return battleManager.PlayerActiveTimeEvent(0.0f, 0.7f, (result) =>
        {
             HandleTimingResultForPlayerAttack(user, target, "Attack1Trigger", result, baseDamage);
             
        
        });

        skillExecutionComplete = true; // Mark the skill as executed once completed
    }
}

       
      
   
    
