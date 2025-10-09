using System.Collections;
using UnityEngine;

public static class BattleSeq
{
    // Simple timer step
    public static IEnumerator Wait(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime; // use unscaled if you do hitpause elsewhere
            yield return null;
        }
    }

    // Move the VISUAL child's localPosition along a parabola to 'toLocal'
    public static IEnumerator HopArc(Transform visual, Vector3 toLocal, float height, float duration)
    {
        Vector3 fromLocal = visual.localPosition;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            Vector3 pos = Vector3.Lerp(fromLocal, toLocal, p);
            pos.y += 4f * height * p * (1f - p); // smooth arc
            visual.localPosition = pos;
            yield return null;
        }
        visual.localPosition = toLocal;
    }

    // Linear slide of the VISUAL child's localPosition to 'toLocal'
    public static IEnumerator SlideLinear(Transform visual, Vector3 toLocal, float duration)
    {
        Vector3 fromLocal = visual.localPosition;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            visual.localPosition = Vector3.Lerp(fromLocal, toLocal, p);
            yield return null;
        }
        visual.localPosition = toLocal;
    }

    // Temporarily bump sorting order while overlapping the target
    public static IEnumerator WithSortingBump(SpriteRenderer sr, int bump, IEnumerator inner)
    {
        if (sr == null) { yield return inner; yield break; }
        int orig = sr.sortingOrder;
        sr.sortingOrder = orig + bump;
        yield return inner;
        sr.sortingOrder = orig;
    }
}
