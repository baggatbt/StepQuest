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
        skillLevel = 1; // You could set a default level or fetch it from the player's saved data
  
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{   
    yield return battleManager.PlayerActiveTimeEvent(0.0f, 1.0f, (result) =>
    {
        user.animator.SetTrigger("Attack1Trigger");
        user.isAttacking = true;

        int damage = (int)System.Math.Round(user.attackPower * (1 + GetSkillLevel() / 10.0));



        if (result == TimingEventResult.Perfect)
        {
            target.TakeDamage(damage);
            Debug.Log("Slash successful! " + target.name + " takes " + damage + " damage.");
            user.GainEnergy((energyCost / 2));
            user.animator.SetTrigger("AttackFailTrigger");
             // Gain +5 skillEXP on perfect
            PlayerData.Instance.IncreaseSkillExp(skillName, 5);
            
            
        }
        else if (result == TimingEventResult.Good)
        {
            target.TakeDamage(damage);
            Debug.Log("Slash successful! " + target.name + " takes " + damage + " damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
            PlayerData.Instance.IncreaseSkillExp(skillName, 3);

        }
        else
        {
            target.TakeDamage(0);
            Debug.Log("Slash missed! " + target.name + " takes 0 damage.");
            user.animator.SetTrigger("AttackFailTrigger");
            user.isAttacking = false;
            PlayerData.Instance.IncreaseSkillExp(skillName, 0);
            
           
        }
    });
   
    
    
   
}
}
