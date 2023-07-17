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
    Vector3 targetPosition = target.transform.position;
    Vector3 attackerPosition = new Vector3(targetPosition.x - 1f, targetPosition.y, targetPosition.z); // Offset the attacker's position

    user.transform.position = attackerPosition;

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

    // Move the attacker back to its original position
    user.transform.position = user.originalPosition;
}

}
