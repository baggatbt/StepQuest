using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    public SlimeAttackSkill()
    {
        name = "Slime Attack";
        description = "The slime attacks the player. The damage can be reduced by timely action.";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        Debug.Log("Using basic attack");
        // Start the attack animation
        user.animator.SetTrigger("SlimeAttack1Trigger");

        // Create a timing window for the player to reduce damage
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(0.0f, 1.0f, (result) =>
        {
            if (result == TimingEventResult.Perfect)
            {
                Debug.Log("Perfect Timing! Damage dodged.");
                target.TakeDamage(user.damage - user.damage);
            }
            else if (result == TimingEventResult.Good)
            {
                Debug.Log("Good Timing! Damage reduced, but not by much.");
                target.TakeDamage(user.damage);
            }
            else if (result == TimingEventResult.Miss)
            {
                Debug.Log("Miss Timing! Full damage taken.");
                target.TakeDamage(user.damage);
            }
           

            user.currentSkill.skillExecutionComplete = true;
        }));
        user.isAttacking = false;
    }
}
