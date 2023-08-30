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
        
        
        yield return battleManager.PlayerActiveTimeEvent(0.0f, 1.0f, (result) =>
    {
        user.animator.SetTrigger("Attack1Trigger");
        user.isAttacking = true;
    


        int damage = PlayerData.Instance.attackPower;



        if (result == TimingEventResult.Perfect)
        {
            target.TakeDamage(damage + ((int)System.Math.Round(PlayerData.Instance.attackPower * .5)));
            AudioManager.instance.PlaySlashSound();
            Debug.Log("Slash perfect! " + target.name + " takes " + damage + " damage.");
            user.GainEnergy(2);
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
           
            
            
        }
        else if (result == TimingEventResult.Good)
        {
            target.TakeDamage(damage);
            AudioManager.instance.PlaySlashSound();
            Debug.Log("Slash successful! " + target.name + " takes " + damage + " damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
            

        }
        else
        {
            
            Debug.Log("Slash missed! " + target.name + " takes 0 damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
        }
    });
    }
 
}
}



/*
        // Create an instance of Airborne with a duration of 2 seconds
        Airborne airborneEffect = new Airborne(1.0f);
        battleManager.statusEffectController.AddEffect(airborneEffect, target);

        */