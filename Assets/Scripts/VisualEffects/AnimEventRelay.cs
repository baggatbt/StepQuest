using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-400)]
public class AnimEventRelay : MonoBehaviour
{
    private Character _character;
    private ActorVisuals _visuals;
    private BattleManager _bm;

    // public read-only so skills can poll if needed
    public bool HasEnded { get; private set; }

    void Awake()
    {
        _character = GetComponentInParent<Character>();
        _visuals   = GetComponentInParent<ActorVisuals>();
        _visuals?.EnsureCached();
        _bm = FindObjectOfType<BattleManager>();
        HasEnded = false;
    }

    // ───────── Animation Events (call these from the clip) ─────────
    public void OpenTimingWindow()
    {
        if (_bm == null) return;
        if (_bm.outerCircle) _bm.outerCircle.SetActive(true);
        if (_bm.innerCircle) _bm.innerCircle.SetActive(true);
    }

    public void CloseTimingWindow()
    {
        if (_bm == null) return;
        if (_bm.outerCircle) _bm.outerCircle.SetActive(false);
        if (_bm.innerCircle) _bm.innerCircle.SetActive(false);
    }

    public void AnimationEnded()
    {
        HasEnded = true;
    }

    // Reset before playing a new attack
    public void ResetEndFlag()
    {
        HasEnded = false;
    }

    /// <summary>
    /// Wait until AnimationEnded() fires, or the animator leaves the given state,
    /// or a timeout is reached. Prevents soft-locks if the last-frame event is skipped.
    /// </summary>
    public IEnumerator WaitForAnimEndSafe(Animator anim, string stateName, int layer = 0, float timeout = 1.5f)
    {
        HasEnded = false;

        // If no animator/state provided, just wait on the flag with timeout
        float t = 0f;
        int stateHash = !string.IsNullOrEmpty(stateName) ? Animator.StringToHash(stateName) : 0;

        // optional: wait until we actually enter the state (briefly)
        float enterWait = 0.25f;
        while (enterWait > 0f && anim != null && !string.IsNullOrEmpty(stateName))
        {
            var s = anim.GetCurrentAnimatorStateInfo(layer);
            if (s.shortNameHash == stateHash) break;
            enterWait -= Time.deltaTime;
            yield return null;
        }

        // Now wait for end conditions
        while (t < timeout)
        {
            // 1) Animation event fired
            if (HasEnded) break;

            if (anim != null)
            {
                var s = anim.GetCurrentAnimatorStateInfo(layer);

                // 2) We left the state (crossfade/transition) → treat as finished
                if (!string.IsNullOrEmpty(stateName) && s.shortNameHash != stateHash)
                    break;

                // 3) State finished a cycle (normalizedTime >= 1)
                if (s.normalizedTime >= 1f)
                    break;
            }

            t += Time.deltaTime;
            yield return null;
        }
        HasEnded = false; // reset for next attack
    }
}
