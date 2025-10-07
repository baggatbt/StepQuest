using System.Collections;
using UnityEngine;

public class AttackMotionController : MonoBehaviour
{
    [Header("Assign the child that holds SpriteRenderer/Animator")]
    public Transform visual;

    [Header("Ease")]
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

    public IEnumerator ReturnToBase(float duration)
    {
        if (visual == null) yield break;
        Vector3 s = visual.localPosition, e = baseLocal; float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            visual.localPosition = Vector3.LerpUnclamped(s, e, ease.Evaluate(t));
            yield return null;
        }
        visual.localPosition = e;
    }

    // Straight dash in local space
    public IEnumerator DashLocal(Vector2 deltaLocal, float duration)
    {
        if (!visual) yield break;
        Vector3 s = visual.localPosition, e = s + (Vector3)deltaLocal; float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            visual.localPosition = Vector3.LerpUnclamped(s, e, ease.Evaluate(t));
            yield return null;
        }
        visual.localPosition = e;
    }

    // Parabolic hop in local space
    public IEnumerator HopLocal(Vector2 deltaLocal, float peakHeight, float duration)
    {
        if (!visual) yield break;
        Vector3 s = visual.localPosition, e = s + (Vector3)deltaLocal; float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            float u = Mathf.Clamp01(t);
            float y = 4f * peakHeight * u * (1f - u); // 0→peak→0
            Vector3 p = Vector3.LerpUnclamped(s, e, ease.Evaluate(u));
            p.y += y;
            visual.localPosition = p;
            yield return null;
        }
        visual.localPosition = e;
    }
}
