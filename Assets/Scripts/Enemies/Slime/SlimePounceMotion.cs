using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttackMotionController))]
public class SlimePounceMotion : MonoBehaviour
{
    [Header("Runtime target (set by skill)")]
    public Transform target;

    [Header("Tuning")]
    public float jumpTime = 0.35f;   // overall travel time
    public float hopPeak  = 0.9f;    // arc height (world units)
    [Range(0.5f, 1f)] public float reachFactor = 0.9f; // how close to center on target X

    AttackMotionController motion;
    bool landed;

    void Awake() => motion = GetComponent<AttackMotionController>();

    // ── Animation Events ─────────────────────────────────────────────

    // Call at the start of upward motion
    public void BeginPounce()
    {
        if (!target || motion == null) return;

        // Move toward the target's X, stay on the same Y lane (visual moves only)
        float dir  = Mathf.Sign((target.position - transform.position).x == 0 ? 1 : (target.position - transform.position).x);
        float dist = Mathf.Abs(target.position.x - transform.position.x) * reachFactor;
        Vector2 delta = new Vector2(dir * dist, 0f);

        StartCoroutine(motion.HopLocal(delta, hopPeak, jumpTime));
    }

    // Call on the exact landing/impact frame
    public void LandImpact() => landed = true;

    // Optional: at the end of recovery
    public void ReturnToIdle()
    {
        if (motion != null) StartCoroutine(motion.ReturnToBase(0.12f));
    }

    // ── Skill waits on this ──────────────────────────────────────────
    public IEnumerator WaitForLanding()
    {
        landed = false;
        while (!landed) yield return null;
    }
}
