using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWave : Skill
{
    private GameObject swordWavePrefab;

    public SwordWave(GameObject swordWavePrefab)
    {
        name = "SwordWave";
        description = "Release a powerful wave from your sword.";
        this.swordWavePrefab = swordWavePrefab;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 0.0f, 1.0f, (result) =>
        {
            user.animator.SetTrigger("SwordWaveTrigger");

            switch (result)
            {
                case TimingEventResult.Perfect:
                    target.TakeDamage(2);
                    Debug.Log("SwordWave Perfect! " + target.name + " takes 2 damage.");
                    // Use Object.Instantiate to spawn the sword wave
                    GameObject swordWave = Object.Instantiate(swordWavePrefab, user.transform.position, Quaternion.identity);
                    break;

                case TimingEventResult.Good:
                    target.TakeDamage(1);
                    Debug.Log("SwordWave Good! " + target.name + " takes 1 damage.");
                    break;

                case TimingEventResult.Miss:
                    Debug.Log("SwordWave missed! " + target.name + " takes no damage.");
                    user.animator.SetTrigger("AttackFailTrigger");
                    break;
            }
        });

        skillExecutionComplete = true;
    }
}
