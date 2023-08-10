using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWave : Skill
{
    private GameObject swordWavePrefab;
    

    public SwordWave(GameObject swordWavePrefab)
    {
        skillName = "SwordWave";
        description = "Release a powerful wave from your sword.";
        this.swordWavePrefab = swordWavePrefab;
        requiresMovement = false;
        energyCost = 2;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 1.0f, (result) =>
        {
            user.animator.SetTrigger("SwordWaveTrigger");

            switch (result)
            {
                case TimingEventResult.Perfect:
                     foreach (var enemy in battleManager.enemies)
                    {
                    enemy.TakeDamage(2);
                    }
                    Debug.Log("SwordWave Perfect! " + target.name + " takes 2 damage.");
                    user.GainEnergy((energyCost / 2));
                    user.animator.SetTrigger("AttackFailTrigger");

                    // Use Object.Instantiate to spawn the sword wave
                    GameObject swordWave = Object.Instantiate(swordWavePrefab, user.transform.position, Quaternion.identity);

                    // Use the direction of the user character to set the direction of the sword wave.
                    swordWave.transform.forward = user.transform.forward;
                    break;

                case TimingEventResult.Good:
                    Debug.Log("SwordWave Good! " + target.name + " takes 1 damage.");
                    user.animator.SetTrigger("AttackFailTrigger");
                    target.TakeDamage(1);
                    break;

                case TimingEventResult.Miss:
                    Debug.Log("SwordWave missed! " + target.name + " takes no damage.");
                    user.animator.SetTrigger("AttackFailTrigger");
                    break;
            }
        });

        yield return new WaitForSeconds(1.0f);
        skillExecutionComplete = true;
        user.isAttacking = false;
    }



}

