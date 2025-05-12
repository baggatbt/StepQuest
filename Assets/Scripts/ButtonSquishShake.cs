using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Makes a UI Button squash (scale) and then shake when clicked.
/// Attach this to the same GameObject that holds the Button component.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonSquishShake : MonoBehaviour, IPointerClickHandler
{
    [Header("Squish (scale)")]
    [Tooltip("Minimum scale on click (e.g., 0.85 shrinks to 85%).")]
    [Range(0.5f, 1f)] public float squishScale = 0.85f;
    [Tooltip("Time to squish down and back up.")]
    public float squishDuration = 0.1f;

    [Header("Shake (position)")]
    [Tooltip("Total time the shake lasts.")]
    public float shakeDuration = 0.25f;
    [Tooltip("How far in pixels to shake left/right.")]
    public float shakeMagnitude = 10f;
    [Tooltip("How many shakes per second.")]
    public float shakeFrequency = 25f;

    RectTransform rect;         // cached for speed
    Vector3 origScale;          // original local scale
    Vector2 origPos;            // original anchored position

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        origScale = rect.localScale;
        origPos = rect.anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();                 // cancel mid-animation re-clicks
        StartCoroutine(SquishAndShake());
    }

    IEnumerator SquishAndShake()
    {
        // ───── Squish ─────
        float t = 0f;
        while (t < squishDuration)
        {
            t += Time.unscaledDeltaTime;     // UI should ignore timescale pauses
            float p = t / squishDuration;    // 0 → 1
            // Ease: fast-in, fast-out (y = 1 - |2x-1|^2)
            float ease = 1f - Mathf.Pow(Mathf.Abs(2f * p - 1f), 2f);
            float s = Mathf.Lerp(1f, squishScale, ease);
            rect.localScale = origScale * s;
            yield return null;
        }
        rect.localScale = origScale;

        // ───── Shake ─────
        t = 0f;
        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / shakeDuration; // 0 → 1
            float damper   = 1f - progress;     // fade out

            // Sine oscillation left/right
            float offset = Mathf.Sin(progress * shakeFrequency * 2f * Mathf.PI) *
                           shakeMagnitude * damper;
            rect.anchoredPosition = origPos + new Vector2(offset, 0f);

            yield return null;
        }
        rect.anchoredPosition = origPos;    // ensure exact reset
    }
}
