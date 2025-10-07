using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttackMotionController))]
public class GoblinAttackMotion : MonoBehaviour
{
    public Transform target;           // set by the skill before playing the clip

    [Header("Lunge tuning")]
    [Tooltip("How far forward in local X the goblin nudges when the strike begins.")]
    public float lungeDistance = 0.6f;
    [Tooltip("How long the nudge lasts.")]
    public float lungeTime = 0.12f;
    [Tooltip("Optional hop height. 0 = flat dash.")]
    public float hopPeak = 0.15f;

    AttackMotionController motion;

    void Awake()
    {
        motion = GetComponent<AttackMotionController>();
    }

    // === called by Animation Event at the strike begin frame ===
    public void BeginLunge()
    {
        if (target == null || motion == null) return;

        // Determine left/right toward target; only use X (side-on battle)
        float dir = Mathf.Sign((target.position - transform.position).x);
        Vector2 delta = new Vector2(dir * Mathf.Abs(lungeDistance), 0f);

        // Use a tiny hop for punchiness (set hopPeak=0 for flat dash)
        if (hopPeak > 0f)
            StartCoroutine(motion.HopLocal(delta, hopPeak, lungeTime));
        else
            StartCoroutine(motion.DashLocal(delta, lungeTime));
    }

    // Optional: at the end of the clip, snap visual back to its idle anchor
    public void ReturnToIdle()
    {
        motion?.ResetVisual();
    }
}
