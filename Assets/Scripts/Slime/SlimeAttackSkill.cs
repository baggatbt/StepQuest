using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    private TimingEventResult result;
    public SlimeAttackSkill()
    {
        skillName = "Slime Attack";
        description = "The slime attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        energyCost = 0;

    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        // First Timing Event
       
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "SlimeAttack1Trigger");
    }

    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {

        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));

    }


    private void HandleTimingResult(Character user, Character target, string trigger)
    {
        switch (result)
        {
            case TimingEventResult.Perfect:
                Debug.Log("Perfect Block!");
                user.animator.SetTrigger(trigger);
                target.TakeDamage(user.damage - user.damage);
                target.animator.SetTrigger("BlockTrigger");
                AudioManager.instance.PlayBlockSound();
                user.isAttacking = false;
                break;
            case TimingEventResult.Good:
                Debug.Log("Good Hit!");
                user.animator.SetTrigger(trigger);
                target.TakeDamage(user.damage - 1);
                AudioManager.instance.PlaySlashSound();
                AudioManager.instance.PlayPlayerIsHitSound();
                user.isAttacking = false;
                break;
            case TimingEventResult.Miss:
                user.animator.SetTrigger(trigger);
                target.TakeDamage(user.damage);
                AudioManager.instance.PlayPlayerIsHitSound();
                user.isAttacking = false;
                break;
        }
    }
    
 }
    

