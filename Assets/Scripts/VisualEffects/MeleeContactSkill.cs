using System.Collections;
using UnityEngine;

public enum MotionStyle { None, HopArc, Dash }

public class MeleeContactSkill : Skill
{
    // ── Tuning ─────────────────────────────────────────────────────
    public MotionStyle motion = MotionStyle.None;
    public float arcHeight   = 0.6f;
    public float inTime      = 0.18f;
    public float outTime     = 0.16f;
    public float overlapX    = 0.16f;   // how far "onto" the target we land (local X push)
    public int   sortingBump = 8;       // draw over target while overlapping
    public float dashInTime  = 0.14f;   // for MotionStyle.Dash

    public float dmgMultiplier = 1.0f;

    // Animator params/state (MUST match your controller)
    public string attackTrigger   = "Attack1Trigger"; // trigger param name
    public string attackStateName = "Attack1";        // short state name of the attack state
    public float  endWaitTimeout  = 2.0f;             // safety timeout (seconds)

    public MeleeContactSkill()
    {
        requiresMovement = true; // keep your current walk-to-target flow
    }

    protected override int CalculateBaseDamage(Character user)
        => Mathf.Max(1, Mathf.RoundToInt(user.attackPower * dmgMultiplier));

    public override IEnumerator Execute(Character user, Character target, BattleManager bm)
    {
        // ── Cache helpers ──────────────────────────────────────────
        var ua = user.GetComponent<ActorVisuals>();  ua?.EnsureCached();
        var ta = target.GetComponent<ActorVisuals>(); ta?.EnsureCached();

        Transform visual = ua != null ? ua.visual : user.transform; // visual child we move
        var sr       = ua != null ? ua.sr       : user.GetComponentInChildren<SpriteRenderer>();
        var animator = ua != null ? ua.animator : user.animator;

        // Relay lives on the Visual (where the Animator is)
        var relay = (ua != null && ua.visual != null)
            ? ua.visual.GetComponent<AnimEventRelay>()
            : user.GetComponentInChildren<AnimEventRelay>();

        // Record local start so we can hop back later
        Vector3 startLocal = visual.localPosition; // usually (0,0)

        // Compute a small local X shift “onto” the target. We animate only the visual.
        float dir = Mathf.Sign(target.transform.position.x - user.transform.position.x);
        Vector3 landLocal = startLocal + new Vector3(dir * overlapX, 0f, 0f);

        // ── 1) Prep & play attack ─────────────────────────────────
        relay?.ResetEndFlag();
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);

        // ── 2) Motion in (visual-only), with optional sorting raise ─
        IEnumerator goIn =
            motion switch
            {
                MotionStyle.HopArc => BattleSeq.HopArc(visual, landLocal, arcHeight, inTime),
                MotionStyle.Dash   => BattleSeq.SlideLinear(visual, landLocal, dashInTime),
                _                  => BattleSeq.Wait(0.01f)
            };
        yield return BattleSeq.WithSortingBump(sr, sortingBump, goIn);

        // ── 3) Action command during contact (keep if you run the minigame here) ─
        // If you're opening/closing the window via animation events, this still runs the minigame while it's open.
        var result = TimingEventResult.Miss;  // ← rename if your enum differs
        if (bm != null)
            yield return bm.StartCoroutine(bm.PlayerActiveTimeEvent(0.10f, 0.40f, r => result = r));

        // ── 4) Resolve hit + juice ────────────────────────────────
        int dmg = CalculateBaseDamage(user);
        HandleTimingResultForEnemyAttack(user, target, result, dmg); // ← rename if your helper differs

        if (bm != null)
        {
            bm.CameraShakeMagnitude(result);
            yield return bm.StartCoroutine(bm.TimeStop(0.06f, 0.05f));
        }

        // ── 5) Wait for animation to end (event/exit/timeout) ─────
        if (relay != null && animator != null)
            yield return relay.WaitForAnimEndSafe(animator, attackStateName, 0, endWaitTimeout);
        else
            yield return BattleSeq.Wait(0.2f); // never hang

        // ── 6) Motion out (return visual to start) ────────────────
        if (motion == MotionStyle.HopArc)
            yield return BattleSeq.HopArc(visual, startLocal, arcHeight * 0.6f, outTime);
        else if (motion == MotionStyle.Dash)
            yield return BattleSeq.SlideLinear(visual, startLocal, 0.12f);
        // MotionStyle.None → nothing to do
        
    }
}
