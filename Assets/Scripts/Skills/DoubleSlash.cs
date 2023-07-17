using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;



public class DoubleSlash : Skill
{
    public DoubleSlash()
    {
        attackStages = new List<AttackStage>
        {
            new AttackStage { animationTrigger = "Attack1Trigger", timingWindowStart = 0.0f, timingWindowEnd = 1.0f, damage = 1 },
            new AttackStage { animationTrigger = "Attack2Trigger", timingWindowStart = 0.0f, timingWindowEnd = 0.0f, damage = 2 }
        };
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        for (int i = 0; i < attackStages.Count; i++)
        {
            var stage = attackStages[i];
            user.animator.SetTrigger(stage.animationTrigger);
            target.TakeDamage(stage.damage);

            // Don't wait for timing event on the last stage
            if (i != attackStages.Count - 1) 
            {
                yield return battleManager.StartCoroutine(
                    battleManager.PlayerActiveTimeEvent(stage.timingWindowStart, stage.timingWindowEnd, (timingResult) => {
                        // Exit the loop early if the player misses the timing window
                        if (timingResult == TimingEventResult.Miss)
                        {
                            return;
                        }
                    })
                );
            }
        }

        user.isAttacking = false;
    }
}
