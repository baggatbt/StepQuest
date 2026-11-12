using System;
using StepQuest.Adventure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation only. Timing and damage stay in the existing BattleManager/Skill flow.
public sealed class CombatActionPrompt : IDisposable
{
    readonly RpgUi ui;
    readonly GameObject root;
    readonly TMP_Text heading, actionLabel;
    readonly RectTransform fill, zone, marker;
    readonly Button button;
    public readonly CombatActionControl Control;

    public CombatActionPrompt(Canvas canvas)
    {
        float scale = Mathf.Max(1, ((RectTransform)canvas.transform).rect.width / 480f);
        ui = new RpgUi(scale);
        var panel = ui.Panel(canvas.transform, "Timed combat action"); root = panel.gameObject;
        var r = panel.rectTransform;
        float safeBottom = Screen.height > 0 ? Screen.safeArea.yMin / Screen.height : 0;
        r.anchorMin = new Vector2(.04f, safeBottom); r.anchorMax = new Vector2(.96f, safeBottom);
        r.offsetMin = new Vector2(0, 8 * scale); r.offsetMax = new Vector2(0, 140 * scale);
        heading = ui.Text(r, "", 18, ui.Theme.gold);
        Place(heading.rectTransform, new Vector2(.04f, .76f), new Vector2(.96f, .96f));
        heading.alignment = TextAlignmentOptions.Center;
        var track = ui.Panel(r, "Timing track", true);
        Place(track.rectTransform, new Vector2(.05f, .56f), new Vector2(.95f, .72f));
        fill = Bar(track.transform, "Charge", ui.Theme.accent);
        zone = Bar(track.transform, "Perfect release zone", ui.Theme.gold);
        marker = Bar(track.transform, "Timing marker", ui.Theme.light);
        var outline = marker.gameObject.AddComponent<Outline>();
        outline.effectColor = ui.Theme.ink; outline.effectDistance = Vector2.one * scale;
        button = ui.Button(r, "", () => { }, RpgButtonKind.Primary);
        Place(button.GetComponent<RectTransform>(), new Vector2(.04f, .07f), new Vector2(.96f, .49f));
        actionLabel = button.GetComponentInChildren<TMP_Text>();
        Control = button.gameObject.AddComponent<CombatActionControl>();
        root.SetActive(false);
    }
    RectTransform Bar(Transform parent, string name, Color color)
    {
        var rect = RpgUi.Rect(name, parent); var image = rect.gameObject.AddComponent<Image>();
        image.color = color; image.raycastTarget = false; return rect;
    }
    static void Place(RectTransform rect, Vector2 min, Vector2 max)
    { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }
    public void Open(string title, string instruction, float perfectStart, float perfectEnd)
    {
        root.SetActive(true); root.transform.SetAsLastSibling();
        Control.ResetInput(); button.interactable = true;
        heading.text = instruction; actionLabel.text = title;
        Place(zone, new Vector2(perfectStart, .2f), new Vector2(perfectEnd, .8f));
        zone.gameObject.SetActive(perfectEnd > perfectStart);
        SetProgress(0);
    }
    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        Place(fill, new Vector2(.01f, .2f), new Vector2(Mathf.Max(.01f, progress), .8f));
        Place(marker, new Vector2(Mathf.Max(0, progress - .004f), .06f), new Vector2(Mathf.Min(1, progress + .004f), .94f));
    }
    public void SetInstruction(string text) { heading.text = text; }
    public void Feedback(string text)
    { heading.text = text; button.interactable = false; Control.Accepting = false; }
    public void Close() { Control.Cancel(); root.SetActive(false); }
    public void Dispose() { if (root != null) UnityEngine.Object.Destroy(root); ui.Dispose(); }
}
