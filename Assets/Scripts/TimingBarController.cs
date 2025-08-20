using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public enum TimingResult { Miss, Okay, Good }

public class TimingBarController : MonoBehaviour
{
    [Header("Panel")]
    public CanvasGroup panel;
    public Button tapAnywhereButton;

    [Header("Refs")]
    public RectTransform track;        // white bar (Image)
    public RectTransform travelArea;   // full inner width
    public RectTransform indicator;    // moving pill (Image)
    public RectTransform targetZone;   // colored window (Image)

    [Header("Tuning")]
    public float sweepSeconds = 1.2f;
    public float edgePadding  = 0f;      // extra gap inside travelArea
    public float innerLeftInset = 0f, innerRightInset = 0f; // if bar sprite has fat borders
    public Vector2 targetWidthPct = new Vector2(0.15f, 0.30f);
    public bool recomputeEveryFrame = true;

    [Header("Indicator Size (fixes your issue)")]
    public bool forceIndicatorSize = true;
    public float indicatorWidthPx  = 24f;   // ← set what you want (e.g., 16–32)
    public float indicatorHeightPx = 0f;    // 0 = keep current; >0 = force; -1 = match travelArea height

    [Header("Debug")]
    public bool logOnChange = true;
    public RectTransform leftMarker, rightMarker; // optional endpoint markers

    public Action<TimingResult> OnFinished;

    float t, leftX, rightX;
    bool running;
    float _lastTAW=-1, _lastIndW=-1, _lastPad=-1, _lastL=-1, _lastR=-1;

    void Awake()
    {
        if (tapAnywhereButton) tapAnywhereButton.onClick.AddListener(HandleTap);

        // Ensure hierarchy
        if (!travelArea)
        {
            travelArea = new GameObject("TravelArea", typeof(RectTransform)).GetComponent<RectTransform>();
            travelArea.SetParent(track, false);
        }
        NormalizeTravelArea();

        if (indicator && indicator.parent != travelArea) indicator.SetParent(travelArea, false);
        if (targetZone && targetZone.parent != travelArea) targetZone.SetParent(travelArea, false);

        // Clip (optional)
        if (track && !track.GetComponent<Mask>() && !track.GetComponent<RectMask2D>())
            track.gameObject.AddComponent<RectMask2D>();

        // Left-anchored children + ignore layout + sit on the rail
        PrepChild(indicator);
        PrepChild(targetZone);
    }

    void NormalizeTravelArea()
    {
        travelArea.anchorMin = new Vector2(0f, 0.5f);
        travelArea.anchorMax = new Vector2(1f, 0.5f);
        travelArea.pivot     = new Vector2(0.5f, 0.5f);
        travelArea.anchoredPosition = Vector2.zero;
        travelArea.sizeDelta = Vector2.zero;                // Left/Right = 0
        var le = travelArea.GetComponent<LayoutElement>() ?? travelArea.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;
        travelArea.localScale = Vector3.one;
    }

    void PrepChild(RectTransform rt)
    {
        if (!rt) return;
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 0.5f);  // left-anchored
        rt.pivot     = new Vector2(0.5f, 0.5f);
        var le = rt.GetComponent<LayoutElement>() ?? rt.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;
        var a = rt.anchoredPosition; a.y = 0f; rt.anchoredPosition = a;
        rt.localScale = Vector3.one;
    }

    void OnRectTransformDimensionsChange() { _lastTAW = -1f; }

    void Update()
    {
        if (!running || sweepSeconds <= 0f) return;

        if (recomputeEveryFrame) TryRecalcIfChanged();

        t += Time.unscaledDeltaTime;
        float u = Mathf.PingPong(t / sweepSeconds, 1f);
        float x = Mathf.Lerp(leftX, rightX, u);
        indicator.anchoredPosition = new Vector2(x, 0f);

        if (leftMarker)  leftMarker.anchoredPosition  = new Vector2(leftX, 0f);
        if (rightMarker) rightMarker.anchoredPosition = new Vector2(rightX, 0f);
    }

    // ─────────── Public API ───────────
    public void OpenAndStart()
    {
        gameObject.SetActive(true);
        Show(true);

        Canvas.ForceUpdateCanvases();
        NormalizeTravelArea();
        EnsureIndicatorSize();     // <<< enforce sane width BEFORE measuring
        RecalcTravel();
        RandomizeTarget();
        SnapLeft();
        running = true;

        StartCoroutine(RecalcNextFrame());
    }

    IEnumerator RecalcNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        NormalizeTravelArea();
        EnsureIndicatorSize();     // <<< enforce again after any late layout
        RecalcTravel();
        SnapLeft();
    }

    public void Close() { running = false; Show(false); }

    void Show(bool on)
    {
        if (panel)
        {
            panel.alpha = on ? 1 : 0;
            panel.interactable = on;
            panel.blocksRaycasts = on;
        }
        gameObject.SetActive(on);
    }

    // ─────────── Calc helpers ───────────
    void EnsureIndicatorSize()
    {
        if (!indicator || !forceIndicatorSize) return;

        // Width: fixed pixels (prevents “stretched to parent” leftovers like your 829px)
        float w = Mathf.Max(1f, indicatorWidthPx);
        indicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);

        // Height: keep, or force, or match travelArea height
        if (indicatorHeightPx > 0f)
        {
            indicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, indicatorHeightPx);
        }
        else if (indicatorHeightPx < 0f && travelArea) // -1 → match travel height
        {
            indicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, travelArea.rect.height);
        }
    }

    void TryRecalcIfChanged()
    {
        float taw = travelArea.rect.width;
        float ind = indicator.rect.width;
        if (!Mathf.Approximately(taw,_lastTAW) || !Mathf.Approximately(ind,_lastIndW) ||
            !Mathf.Approximately(edgePadding,_lastPad) ||
            !Mathf.Approximately(innerLeftInset,_lastL) || !Mathf.Approximately(innerRightInset,_lastR))
        {
            RecalcTravel();
        }
    }

    void RecalcTravel()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(travelArea);

        float taw     = travelArea.rect.width;
        float halfInd = indicator.rect.width * 0.5f;
        float pad     = Mathf.Max(0f, edgePadding);
        float insetL  = Mathf.Max(0f, innerLeftInset);
        float insetR  = Mathf.Max(0f, innerRightInset);

        leftX  = insetL + halfInd + pad;
        rightX = taw - (insetR + halfInd + pad);
        if (rightX < leftX) { float mid = taw * 0.5f; leftX = rightX = mid; }

        if (logOnChange)
            Debug.Log($"[TimingBar] travelW={taw:F1} indW={indicator.rect.width:F1} leftX={leftX:F1} rightX={rightX:F1} span={(rightX-leftX):F1}");

        _lastTAW = taw; _lastIndW = indicator.rect.width; _lastPad = edgePadding; _lastL = innerLeftInset; _lastR = innerRightInset;
    }

    void RandomizeTarget()
    {
        float taw = travelArea.rect.width;
        float pct = Mathf.Clamp01(UnityEngine.Random.Range(targetWidthPct.x, targetWidthPct.y));
        float w   = pct * taw;
        targetZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);

        float halfTZ = w * 0.5f;
        float minCx  = leftX + halfTZ;
        float maxCx  = rightX - halfTZ;
        float cx     = (minCx <= maxCx) ? UnityEngine.Random.Range(minCx, maxCx) : taw * 0.5f;

        targetZone.anchoredPosition = new Vector2(cx, 0f);
    }

    void SnapLeft() { indicator.anchoredPosition = new Vector2(leftX, 0f); t = 0f; }

    void HandleTap()
    {
        if (!running) return;
        running = false;

        float ix = indicator.anchoredPosition.x;
        float cx = targetZone.anchoredPosition.x;
        float halfTZ = targetZone.rect.width * 0.5f;

        var result =
            (Mathf.Abs(ix - cx) <= halfTZ * 0.33f) ? TimingResult.Good :
            (Mathf.Abs(ix - cx) <= halfTZ)         ? TimingResult.Okay :
                                                     TimingResult.Miss;

        OnFinished?.Invoke(result);
        Close();
    }
}
