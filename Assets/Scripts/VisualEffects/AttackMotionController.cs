using System.Collections;
using UnityEngine;

/// <summary>
/// Reusable, animation-event friendly motion helper for in-contact moves
/// like hops, lunges, knockbacks, bounces. It moves the attacker's transform
/// along simple tweens/parabolas and then snaps back to the contact anchor.
/// </summary>
public class AttackMotionController : MonoBehaviour
{
    [Header("Refs")]
    public Character owner;                // auto-filled on Awake
    public Transform visualRoot;           // optional: if you want to move only a child
                                           // leave null to move whole transform

    [Header("Contact Anchor (auto)")]
    public Vector3 contactAnchor;          // set when MoveToTarget() finishes
    public Transform currentTarget;        // set by the Skill before attacking

    [Header("Defaults")]
    public float defaultLungeDistance = 0.5f;
    public float defaultLungeTime     = 0.15f;
    public float defaultHopApex       = 0.75f;
    public float defaultHopTime       = 0.28f;
    public float defaultStickTime     = 0.06f;   // “squish” on the target before bouncing back
    public float defaultReturnTime    = 0.18f;

    // State
    private bool isBusy;

    void Awake()
    {
        if (!owner) owner = GetComponent<Character>();
        if (!visualRoot) visualRoot = transform; // move whole character by default
    }

    /// <summary>Call this right after MoveToTarget completes.</summary>
    public void SetContactAnchorFromCurrent()
    {
        contactAnchor = visualRoot.position;
    }

    /// <summary>
    /// Animation Event helper: lunge forward a small distance towards the target, then snap back to anchor.
    /// </summary>
    public void AnimEvent_Lunge()
    {
        if (!currentTarget || isBusy) return;
        StartCoroutine(LungeTowardTargetRoutine(defaultLungeDistance, defaultLungeTime));
    }

    /// <summary>
    /// Animation Event helper: hop in a short arc onto target, stick briefly, then return to anchor.
    /// </summary>
    public void AnimEvent_HopOntoTarget()
    {
        if (!currentTarget || isBusy) return;
        StartCoroutine(HopOntoTargetAndBackRoutine(defaultHopApex, defaultHopTime, defaultStickTime, defaultReturnTime));
    }

    /// <summary>Reset visualRoot to contact anchor (useful safeguard at the end of an anim).</summary>
    public void AnimEvent_ResetToContact()
    {
        visualRoot.position = contactAnchor;
    }

    // ---------- Routines ----------

    private IEnumerator LungeTowardTargetRoutine(float distance, float time)
    {
        isBusy = true;

        Vector3 start = visualRoot.position;
        Vector3 dir   = (currentTarget.position - start).normalized;
        Vector3 peak  = contactAnchor + dir * distance;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / time);
            visualRoot.position = Vector3.Lerp(start, peak, EaseOutCubic(p));
            yield return null;
        }

        // return quickly to anchor
        t = 0f;
        const float backTime = 0.08f;
        while (t < backTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / backTime);
            visualRoot.position = Vector3.Lerp(peak, contactAnchor, EaseInCubic(p));
            yield return null;
        }

        visualRoot.position = contactAnchor;
        isBusy = false;
    }

    private IEnumerator HopOntoTargetAndBackRoutine(float apex, float hopTime, float stickTime, float returnTime)
    {
        isBusy = true;

        Vector3 start = visualRoot.position;              // contact anchor
        Vector3 end   = GetLandingPointOnTarget();        // where we “land” on the target

        // Forward arc (parabola in XY; if you’re strictly 2D, we simulate arc by Y offset)
        float t = 0f;
        while (t < hopTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / hopTime);

            Vector3 pos = Vector3.Lerp(start, end, p);
            float yArc  = ParabolaY(p) * apex;           // little hump
            pos += Vector3.up * yArc;

            visualRoot.position = pos;
            yield return null;
        }
        visualRoot.position = end;

        // Brief “stick/squish” on the target
        if (stickTime > 0f) yield return new WaitForSeconds(stickTime);

        // Return to contact anchor
        t = 0f;
        while (t < returnTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / returnTime);
            visualRoot.position = Vector3.Lerp(end, contactAnchor, EaseInOutCubic(p));
            yield return null;
        }

        visualRoot.position = contactAnchor;
        isBusy = false;
    }

    private Vector3 GetLandingPointOnTarget()
    {
        // Land slightly “into” the target from anchor, based on their centers.
        Vector3 dir = (currentTarget.position - contactAnchor).normalized;
        float   pad = 0.12f; // tiny overlap looks like a real stomp
        return currentTarget.position - dir * pad;
    }

    // ---------- Easing / Arc helpers ----------

    private static float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);
    private static float EaseInCubic (float x) => x * x * x;
    private static float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
    }

    // returns 0→1→0, nice arc shape
    private static float ParabolaY(float x) => -4f * (x - 0.5f) * (x - 0.5f) + 1f;
}
