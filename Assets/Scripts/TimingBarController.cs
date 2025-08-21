using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Movement")]
    public float sweepSeconds = 1.2f;
    public bool recomputeEveryFrame = true;

    [Header("Bounds / Insets")]
    public float edgePadding  = 0f;                     // gap inside travelArea
    public float innerLeftInset = 0f, innerRightInset = 0f; // if the bar sprite has borders

    [Header("Target Window")]
    [Tooltip("Initial target width as % of travelArea width (min..max).")]
    public Vector2 targetWidthPct = new Vector2(0.15f, 0.30f);
    [Tooltip("Minimum allowed target width as % of travelArea width.")]
    public float minTargetWidthPct = 0.06f;
    [Tooltip("If the player hits Okay/Good, next attempt width *= this.")]
    [Range(0.3f, 0.95f)] public float successShrinkFactor = 0.6f;

    [Header("Indicator Sizing")]
    public bool forceIndicatorSize = true;
    public float indicatorWidthPx  = 24f;
    public float indicatorHeightPx = -1f;   // -1 = match travelArea height, 0 = keep, >0 = px

    [Header("Series (multi-attempt)")]
    [Min(1)] public int attemptsPerSeries = 3;

    [Header("Debug")]
    public bool logOnChange = false;
    public RectTransform leftMarker, rightMarker;

    // Callbacks
    public Action<TimingResult[]> OnSeriesFinished;   // <-- use this
    public Action<TimingResult>   OnFinished;         // (legacy single attempt)

    // runtime
    float t, leftX, rightX;
    bool running;
    float _lastTAW=-1, _lastIndW=-1, _lastPad=-1, _lastL=-1, _lastR=-1;

    // series state
    readonly List<TimingResult> _results = new();
    float _currentTargetWidthPx;

    void Awake()
    {
        if (tapAnywhereButton) tapAnywhereButton.onClick.AddListener(HandleTap);

        if (!travelArea)
        {
            travelArea = new GameObject("TravelArea", typeof(RectTransform)).GetComponent<RectTransform>();
            travelArea.SetParent(track, false);
        }
        NormalizeTravelArea();

        if (indicator && indicator.parent != travelArea) indicator.SetParent(travelArea, false);
        if (targetZone && targetZone.parent != travelArea) targetZone.SetParent(travelArea, false);

        if (track && !track.GetComponent<Mask>() && !track.GetComponent<RectMask2D>())
            track.gameObject.AddComponent<RectMask2D>();

        PrepChild(indicator);
        PrepChild(targetZone);
    }

    void NormalizeTravelArea()
    {
        travelArea.anchorMin = new Vector2(0f, 0.5f);
        travelArea.anchorMax = new Vector2(1f, 0.5f);
        travelArea.pivot     = new Vector2(0.5f, 0.5f);
        travelArea.anchoredPosition = Vector2.zero;
        travelArea.sizeDelta = Vector2.zero;

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
    public void OpenAndStart() => OpenAndStartSeries(1);

    public void OpenAndStartSeries(int attempts)
    {
        attemptsPerSeries = Mathf.Max(1, attempts);

        gameObject.SetActive(true);
        Show(true);

        Canvas.ForceUpdateCanvases();
        NormalizeTravelArea();
        EnsureIndicatorSize();
        RecalcTravel();

        _results.Clear();
        SetupInitialTarget();
        SnapLeft();

        running = true;
        StartCoroutine(RecalcNextFrame());
    }

    IEnumerator RecalcNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        NormalizeTravelArea();
        EnsureIndicatorSize();
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

    // ─────────── Sizing & travel ───────────
    void EnsureIndicatorSize()
    {
        if (!indicator || !forceIndicatorSize) return;

        float w = Mathf.Max(1f, indicatorWidthPx);
        indicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);

        if (indicatorHeightPx > 0f)
            indicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, indicatorHeightPx);
        else if (indicatorHeightPx < 0f && travelArea)
            indicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(1f, travelArea.rect.height));
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

    void SnapLeft() { indicator.anchoredPosition = new Vector2(leftX, 0f); t = 0f; }

    // ─────────── Target placement & shrinking ───────────
    void SetupInitialTarget()
    {
        float taw = travelArea.rect.width;
        float pct = Mathf.Clamp01(UnityEngine.Random.Range(targetWidthPct.x, targetWidthPct.y));
        _currentTargetWidthPx = Mathf.Max(taw * minTargetWidthPct, taw * pct);
        PlaceTargetAtRandomX();
    }

    void ShrinkTargetForSuccess()
    {
        float taw = travelArea.rect.width;
        float minPx = taw * minTargetWidthPct;
        _currentTargetWidthPx = Mathf.Max(minPx, _currentTargetWidthPx * successShrinkFactor);
        PlaceTargetAtRandomX();
    }

    void PlaceTargetAtRandomX()
    {
        targetZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _currentTargetWidthPx);

        float half = _currentTargetWidthPx * 0.5f;
        float minCx = leftX + half;
        float maxCx = rightX - half;
        float cx = (minCx <= maxCx) ? UnityEngine.Random.Range(minCx, maxCx) : (leftX + rightX) * 0.5f;
        targetZone.anchoredPosition = new Vector2(cx, 0f);
    }

    // ─────────── Input ───────────
    void HandleTap()
    {
        if (!running) return;

        // grade
        float ix = indicator.anchoredPosition.x;
        float cx = targetZone.anchoredPosition.x;
        float halfTZ = targetZone.rect.width * 0.5f;
        float d = Mathf.Abs(ix - cx);

        TimingResult result =
            (d <= halfTZ * 0.33f) ? TimingResult.Good :
            (d <= halfTZ)         ? TimingResult.Okay :
                                    TimingResult.Miss;

        _results.Add(result);

        bool success = (result != TimingResult.Miss);
        bool more    = _results.Count < attemptsPerSeries;

        if (success && more)
        {
            // make next one harder and keep going
            ShrinkTargetForSuccess();
            SnapLeft();
            return;
        }

        if (more)
        {
            // miss → same width, just reroll position
            PlaceTargetAtRandomX();
            SnapLeft();
            return;
        }

        // finished the series
        running = false;
        OnSeriesFinished?.Invoke(_results.ToArray());
        // legacy single-attempt callback (report last)
        OnFinished?.Invoke(result);

        Close();
    }
}
