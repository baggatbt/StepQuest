using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    private int numberOfSlashes;

    public Slash()
    {
        skillName = "Slash";
        description = "A powerful slashing attack.";
        requiresMovement = true;
        energyCost = 0;
        skillLevel = 1; 
        numberOfSlashes = 1;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
    for (int i = 0; i < numberOfSlashes; i++)
    {
       
    
        user.isAttacking = true;

        // Wait for a short delay, then start the timing event. 
        // to match when I want the timing event to occur during the animation.
        yield return new WaitForSeconds(0.1f);  // Adjust this value based on animation's timing

        yield return battleManager.PlayerActiveTimeEvent(0.0f, 0.7f, (result) =>
        {
            HandleTimingResultForPlayerAttack(user, target, "Attack1Trigger", result);

            // Trigger the end/fail animation here instead of repeating it for each result.
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
        });
    }
}
}



/*
        // Create an instance of Airborne with a duration of 2 seconds
        Airborne airborneEffect = new Airborne(1.0f);
        battleManager.statusEffectController.AddEffect(airborneEffect, target);

        */