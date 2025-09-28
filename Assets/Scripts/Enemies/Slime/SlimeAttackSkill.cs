using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    // small hop to “land on” the target while the anim plays
    private readonly float hopDistance = 0.35f;
    private readonly float hopHeight = 0.22f;
    private readonly float hopDuration = 0.18f;

    private int numberOfAttacksPossible;

    public SlimeAttackSkill()
    {
        skillName = "Slime Pounce";
        description = "The slime hops and lands on the target.";
        requiresMovement = true;  // walk up first (BattleManager handles this)
        numberOfAttacksPossible = 1;
    }

    protected override int CalculateBaseDamage(Character user)
    {
        // Light hit by default; tweak to your liking
        return Mathf.Max(1, Mathf.RoundToInt(user.attackPower * 0.9f));
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;
        user.damageApplied = false;

        int baseDamage = CalculateBaseDamage(user);

        // Trigger your slime attack animation (set this trigger in Animator)
        user.animator.SetTrigger("SlimeJumpAttackTrigger");

        // Add a tiny forward “landing” hop while the animation plays
        yield return user.StartCoroutine(HopNudgeTowardTarget(user, target, hopDistance, hopHeight, hopDuration));

        // Timing + damage (matches your Goblin flow)
        for (int i = 0; i < numberOfAttacksPossible; i++)
        {
            yield return TimingManager.Instance.HandleTimingWindow(user, target, baseDamage, (TimingEventResult result) =>
            {
                HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
                battleManager.CameraShakeMagnitude(result);
            });
        }

        // Wait for the animation’s end flag (your Animator should set user.isAnimationDone=true on the final event)
        yield return new WaitUntil(() => user.isAnimationDone);

        user.isAnimationDone = false;
        user.isAttacking = false;

        target.CheckForDeath();
    }

    private IEnumerator HopNudgeTowardTarget(Character user, Character target, float distance, float height, float duration)
    {
        Vector3 start = user.transform.position;
        Vector3 dir = (target.transform.position - start).normalized;

        // forward hop
        yield return ParabolicMove(user.transform, start, start + dir * distance, height, duration);
        // quick return so enemy ends where BattleManager expects
        yield return ParabolicMove(user.transform, user.transform.position, start, height * 0.7f, duration * 0.8f);
    }

    private IEnumerator ParabolicMove(Transform t, Vector3 from, Vector3 to, float height, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float p = Mathf.Clamp01(timer / duration);

            Vector3 pos = Vector3.Lerp(from, to, p);
            pos.y += height * Mathf.Sin(Mathf.PI * p);

            t.position = pos;
            yield return null;
        }
        t.position = to;
    }
}
