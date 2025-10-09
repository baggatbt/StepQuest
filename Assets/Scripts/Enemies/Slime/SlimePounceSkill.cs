using System.Collections;
using UnityEngine;

public class SlimePounceSkill : Skill
{
    // Tuning knobs (feel free to expose as properties)
    public float arcHeight = 0.65f;
    public float inTime    = 0.18f;
    public float outTime   = 0.16f;
    public float overlapX  = 0.16f;   // how far onto target we “land”
    public int   sortBump  = 8;       // draw over target while on top
    private int numberOfAttacksPossible;
    public SlimePounceSkill()
    {
        skillName = "Pounce";
        description = "Hop onto the target, dealing damage.";
        requiresMovement = true; // keep your existing walk-up behavior
        numberOfAttacksPossible = 1; // if your base Skill uses this
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return Mathf.Max(1, Mathf.RoundToInt(user.attackPower * 1.0f));
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager bm)
    {
        // 0) we are already standing in front—your requiresMovement flow handled that.
        var ua = user.GetComponent<ActorVisuals>();  ua?.EnsureCached();
        var ta = target.GetComponent<ActorVisuals>(); ta?.EnsureCached();

        Transform visual = ua != null ? ua.visual : user.transform;           // fallback safe
        var sr = ua != null ? ua.sr : user.GetComponentInChildren<SpriteRenderer>();
        var animator = ua != null ? ua.animator : user.animator;              // use your existing animator if needed

        Vector3 startLocal = visual.localPosition; // usually (0,0)
        float dir = Mathf.Sign(target.transform.position.x - user.transform.position.x);

        // Compute a small local X shift “onto” the target. We animate only the visual.
        Vector3 landLocal = startLocal + new Vector3(dir * overlapX, 0f, 0f);

        // 1) wind-up: trigger your attack anim (no root motion)
        if (animator != null) animator.SetTrigger("Attack1Trigger");

        // 2) hop in (with sorting bump so we draw over)
        yield return BattleSeq.WithSortingBump(sr, sortBump,
            BattleSeq.HopArc(visual, landLocal, arcHeight, inTime));

        // 3) open action command window while “on” the target
        var result = TimingEventResult.Miss; // adjust if your enum is named differently
        if (bm != null)
        {
            yield return bm.StartCoroutine(bm.PlayerActiveTimeEvent(0.10f, 0.40f, r => result = r));
        }

        // 4) resolve damage using your existing helper(s)
        int dmg = CalculateBaseDamage(user);
        HandleTimingResultForEnemyAttack(user, target, result, dmg);

        // juice: shake + hitpause if you have them
        if (bm != null)
        {
            bm.CameraShakeMagnitude(result);
            yield return bm.StartCoroutine(bm.TimeStop(0.06f, 0.05f));
        }

        // 5) hop back and finish
        yield return BattleSeq.HopArc(visual, startLocal, arcHeight * 0.6f, outTime);

        yield return null;
    }
}
