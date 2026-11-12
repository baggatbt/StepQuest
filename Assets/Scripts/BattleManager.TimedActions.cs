using System;
using System.Collections;
using UnityEngine;

public struct TimedActionResult
{
    public TimingEventResult grade;
    public float seconds;
    public int presses;
    public bool cancelled;
}

public partial class BattleManager
{
    CombatActionPrompt timedActionPrompt;
    public bool CanPlanAction(Character actor) => actor != null && actor.health > 0 && !actor.isAttacking && !actor.isMoving
        && activePlayer == actor && ShouldShowCombatUI();
    public bool CanResolveAction(Character actor) => isActiveAndEnabled && isBattleStarted && !battleFinished && !battleLost
        && actor != null && actor.health > 0 && activePlayer == actor && state == BattleState.PlayerTurn && Time.timeScale > 0;

    CombatActionPrompt TimedPrompt()
    {
        if (timedActionPrompt != null) return timedActionPrompt;
        var canvas = companionSkillsPanel != null ? companionSkillsPanel.GetComponentInParent<Canvas>() : popupCanvas;
        if (canvas == null) return null;
        return timedActionPrompt = new CombatActionPrompt(canvas.rootCanvas);
    }

    // Overload the existing hold/release timing event; legacy Knight calls keep
    // their animation-driven behavior and existing signature unchanged.
    public IEnumerator PlayerHoldReleaseTimeEvent(Character user, ChargeTimingSettings settings,
        Action<TimedActionResult> callback, Action<float> charging = null)
    {
        var prompt = TimedPrompt();
        if (prompt == null) { callback(new TimedActionResult { cancelled = true }); yield break; }
        prompt.Open("FIREBALL?", "HOLD - release inside the gold zone", settings.Start / settings.Maximum, settings.End / settings.Maximum);
        float waiting = 0, held = 0;
        bool started = false;
        try
        {
            while (CanResolveAction(user) && !prompt.Control.Cancelled)
            {
                if (!started)
                {
                    if (prompt.Control.PressCount > 0) started = true;
                    else
                    {
                        waiting += Time.unscaledDeltaTime;
                        if (waiting >= Mathf.Max(.1f, settings.inputWaitTimeoutSeconds)) break;
                        yield return null; continue;
                    }
                }
                if (!prompt.Control.IsHeld) break;
                held = Mathf.Min(settings.Maximum, held + Time.unscaledDeltaTime);
                prompt.SetProgress(held / settings.Maximum); charging?.Invoke(held / settings.Maximum);
                prompt.SetInstruction(held < settings.Start ? "CHARGING - release in the gold zone"
                    : held <= settings.End ? "PERFECT - RELEASE NOW!" : "OVERCHARGED - the book is burning up!");
                if (held >= settings.Maximum) break;
                yield return null;
            }
            bool cancelled = !CanResolveAction(user) || prompt.Control.Cancelled;
            callback(new TimedActionResult { seconds = held, grade = ActionTimingRules.Charge(held, settings), cancelled = cancelled });
        }
        finally { prompt.Control.Accepting = false; }
    }

    // Same TimingEventResult contract as PlayerActiveTimeEvent, with a compact
    // control-side indicator and a configurable, touch-friendly perfect zone.
    public IEnumerator PlayerActiveTimeEvent(Character user, SweepTimingSettings settings, Action<TimedActionResult> callback)
    {
        var prompt = TimedPrompt();
        if (prompt == null) { callback(new TimedActionResult { cancelled = true }); yield break; }
        prompt.Open("METEOR", "TAP when the marker reaches the gold zone", settings.Start, settings.End);
        float elapsed = 0;
        try
        {
            while (CanResolveAction(user) && !prompt.Control.Cancelled && elapsed < settings.Duration)
            {
                if (prompt.Control.PressCount > 0) break;
                elapsed = Mathf.Min(settings.Duration, elapsed + Time.unscaledDeltaTime);
                prompt.SetProgress(elapsed / settings.Duration); yield return null;
            }
            callback(new TimedActionResult { seconds = elapsed,
                grade = prompt.Control.PressCount > 0 ? ActionTimingRules.Sweep(elapsed / settings.Duration, settings) : TimingEventResult.Miss,
                cancelled = !CanResolveAction(user) || prompt.Control.Cancelled });
        }
        finally { prompt.Control.Accepting = false; }
    }

    public IEnumerator PlayerRapidInputEvent(Character user, float seconds, int maximumPresses, float minimumInterval,
        Func<int, bool> onPress, Action<TimedActionResult> callback)
    {
        var prompt = TimedPrompt();
        if (prompt == null) { callback(new TimedActionResult { cancelled = true }); yield break; }
        prompt.Open("MAGIC MISSILE", "TAP repeatedly - these missiles are fists", 0, 0);
        float elapsed = 0;
        int presses = 0, consumed = 0;
        var session = new RapidInputSession(seconds, maximumPresses, minimumInterval);
        int cap = session.MaximumPresses;
        float duration = session.Duration;
        try
        {
            while (CanResolveAction(user) && !prompt.Control.Cancelled && elapsed < duration && presses < cap)
            {
                if (prompt.Control.PressCount > consumed)
                {
                    consumed = prompt.Control.PressCount;
                    if (session.TryPress(elapsed))
                    {
                        presses = session.Presses;
                        prompt.SetInstruction("PUNCHES " + presses + " / " + cap);
                        if (!onPress(presses)) break;
                    }
                }
                elapsed += Time.unscaledDeltaTime; prompt.SetProgress(elapsed / duration); yield return null;
            }
            callback(new TimedActionResult { seconds = elapsed, presses = presses,
                grade = presses >= cap ? TimingEventResult.Good : presses == 0 ? TimingEventResult.Miss : TimingEventResult.Early,
                cancelled = !CanResolveAction(user) || prompt.Control.Cancelled });
        }
        finally { prompt.Control.Accepting = false; }
    }

    public void ReportTimedFeedback(string message, TimingEventResult grade)
    {
        timedActionPrompt?.Feedback(message);
    }
    public void CancelTimedAction() { timedActionPrompt?.Close(); }
    void OnDisable()
    {
        CancelTimedAction();
        foreach (var player in playerParty)
            if (player is Buffmage) { player.GetComponent<BuffmagePresentation>()?.ResetPresentation(); player.isAttacking = false; }
    }
    void OnDestroy() { timedActionPrompt?.Dispose(); }
}
