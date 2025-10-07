using System.Collections;
using UnityEngine;

public class AttackMotionController : MonoBehaviour
{
    [Header("Assign the child that holds SpriteRenderer/Animator")]
    public Transform visual;

    [Header("Ease for motion")]
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Vector3 baseLocal;

    void Awake()
    {
        if (visual != null) baseLocal = visual.localPosition;
    }

    public void ResetVisual()
    {
        if (visual != null) visual.localPosition = baseLocal;
    }

    // Quick straight-line nudge
    public IEnumerator DashLocal(Vector2 deltaLocal, float duration)
    {
        if (visual == null) yield break;

        Vector3 start = visual.localPosition;
        Vector3 end   = start + (Vector3)deltaLocal;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            visual.localPosition = Vector3.LerpUnclamped(start, end, ease.Evaluate(t));
            yield return null;
        }
        visual.localPosition = end;
    }

    // Small hop arc for “jump into” feel
    public IEnumerator HopLocal(Vector2 deltaLocal, float peakHeight, float duration)
    {
        if (visual == null) yield break;

        Vector3 start = visual.localPosition;
        Vector3 end   = start + (Vector3)deltaLocal;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            float u = Mathf.Clamp01(t);
            float y = 4f * peakHeight * u * (1f - u); // 0→peak→0 parabola
            Vector3 p = Vector3.LerpUnclamped(start, end, ease.Evaluate(u));
            p.y += y;
            visual.localPosition = p;
            yield return null;
        }
        visual.localPosition = end;
    }
}
