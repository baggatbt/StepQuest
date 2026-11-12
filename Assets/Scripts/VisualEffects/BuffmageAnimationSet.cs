using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Buffmage animation set")]
public sealed class BuffmageAnimationSet : ScriptableObject
{
    public RuntimeAnimatorController controller;
    [Header("Generated calibration against the Knight battle sprite")]
    public float authoredWorldHeight = 3.9375f;
    public float punchReach = 2.2f;
    public AnimationClip idle, bookWindup, bookHold, bookThrow, punchLeft, punchRight;
    public AnimationClip meteorReady, meteorLaunch, meteorAir, meteorSlam, meteorRecover, hurt, death;
    [Header("Contact timings, in seconds within the corresponding clip")]
    public float bookReleaseSeconds = 1f / 12f;
    public float leftPunchImpactSeconds = 2f / 24f;
    public float rightPunchImpactSeconds = 1f / 24f;
    public float meteorImpactSeconds = 1f / 12f;
    public float deathHoldSeconds = .2f;
    public float Duration(AnimationClip clip, float fallback) => clip != null ? Mathf.Max(.02f, clip.length) : fallback;
}
