using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    private TimingEventResult result;

    public TripleHitSkill()
    {
        skillName = "Triple Slash";
        description = "Slash up to three times with good timing";
        requiresMovement = true;
        energyCost = 2;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
       
        
        // First Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "Attack1Trigger");
        if (result == TimingEventResult.Miss)
            yield break;
            yield return  user.animationEnded == true;


        if (target.health >= 1)
        {
        // Second Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target,"Attack2Trigger");
        if (result == TimingEventResult.Miss)
            yield break;
            yield return  user.animationEnded == true;
        }
        
        if (target.health >= 1)
        {
        // Third Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "Attack3Trigger");
        user.isAttacking = false;
        yield return user.animationEnded == false; //Resets the animation flag
        }
        
        else 
        {
            user.isAttacking = false;
            user.animator.SetTrigger("AttackFailTrigger");
            yield return user.animationEnded == false; //Resets the animation flag
        }
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
        int damage = PlayerData.Instance.attackPower;
        switch (result)
        {
            case TimingEventResult.Perfect:
                Debug.Log("Perfect Hit!");
                user.animator.SetTrigger(trigger);
                target.TakeDamage(damage + ((int)System.Math.Round(PlayerData.Instance.attackPower * .5)));
                AudioManager.instance.PlaySlashSound();
                user.GainEnergy((energyCost / 2));
                break;
            case TimingEventResult.Good:
                Debug.Log("Good Hit!");
                user.animator.SetTrigger(trigger);
                target.TakeDamage(damage);
                AudioManager.instance.PlaySlashSound();
                break;
            case TimingEventResult.Miss:
                Debug.Log("Missed!");
                user.animator.SetTrigger("AttackFailTrigger");
                user.isAttacking = false;
                break;
        }
    }
}