using System;

[Serializable]
public sealed class ChargeTimingSettings
{
    public float perfectStartSeconds = .9f;
    public float perfectEndSeconds = 1.4f;
    public float maximumHoldSeconds = 2.2f;
    public float inputWaitTimeoutSeconds = 6f;
    public float earlyMinimumMultiplier = .55f, earlyMaximumMultiplier = .95f;
    public float perfectMultiplier = 1.55f, overchargedMultiplier = .7f;
    public float Maximum => Math.Max(.1f, maximumHoldSeconds);
    public float Start => Math.Clamp(perfectStartSeconds, .01f, Maximum - .01f);
    public float End => Math.Clamp(perfectEndSeconds, Start, Maximum - .001f);
}

[Serializable]
public sealed class SweepTimingSettings
{
    public float durationSeconds = 1.6f;
    public float perfectStart = .58f, perfectEnd = .82f;
    public float Duration => Math.Max(.1f, durationSeconds);
    public float Start => Math.Clamp(perfectStart, 0f, 1f);
    public float End => Math.Clamp(perfectEnd, Start, 1f);
}

// Pure timing rules shared by the BattleManager timing-event overloads and tests.
public static class ActionTimingRules
{
    public static TimingEventResult Charge(float seconds, ChargeTimingSettings settings)
        => seconds < settings.Start ? TimingEventResult.Early
            : seconds <= settings.End ? TimingEventResult.Good : TimingEventResult.Late;

    public static float ChargeMultiplier(float seconds, ChargeTimingSettings settings)
    {
        var grade = Charge(seconds, settings);
        float perfect = Math.Max(0, settings.perfectMultiplier);
        if (grade == TimingEventResult.Good) return perfect;
        float ceiling = perfect * .95f;
        if (grade == TimingEventResult.Late) return Math.Clamp(settings.overchargedMultiplier, 0, ceiling);
        float progress = Math.Clamp(seconds / settings.Start, 0f, 1f);
        float minimum = Math.Clamp(settings.earlyMinimumMultiplier, 0, ceiling);
        float maximum = Math.Clamp(settings.earlyMaximumMultiplier, minimum, ceiling);
        return minimum + (maximum - minimum) * progress;
    }

    public static TimingEventResult Sweep(float progress, SweepTimingSettings settings)
        => progress < settings.Start ? TimingEventResult.Early
            : progress <= settings.End ? TimingEventResult.Good : TimingEventResult.Late;
}

public sealed class RapidInputSession
{
    public readonly int MaximumPresses;
    public readonly float Duration, MinimumInterval;
    public int Presses { get; private set; }
    float previous = float.NegativeInfinity;
    public RapidInputSession(float seconds, int cap, float interval)
    {
        Duration = Math.Clamp(seconds, .1f, 10f);
        MaximumPresses = Math.Clamp(cap, 1, 30);
        MinimumInterval = Math.Max(.05f, interval);
    }
    public bool TryPress(float elapsed)
    {
        if (elapsed < 0 || elapsed >= Duration || Presses >= MaximumPresses || elapsed - previous < MinimumInterval) return false;
        previous = elapsed; Presses++; return true;
    }
}
