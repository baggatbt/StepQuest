using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    public Slash()
    {
        name = "Slash";
        description = "A powerful slashing attack.";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
    yield return battleManager.PlayerActiveTimeEvent(0.0f, 1.0f, (result) =>
    {
        user.animator.SetTrigger("Attack1Trigger");

        if (result == TimingEventResult.Perfect || result == TimingEventResult.Good)
        {
            target.TakeDamage(2);
            Debug.Log("Slash successful! " + target.name + " takes 2 damage.");
            user.animator.SetTrigger("AttackFailTrigger");
        }
        else
        {
            target.TakeDamage(1);
            Debug.Log("Slash missed! " + target.name + " takes 1 damage.");
            user.animator.SetTrigger("AttackFailTrigger");
        }
    });

    skillExecutionComplete = true;
}
}
