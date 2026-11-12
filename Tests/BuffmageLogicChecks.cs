using System;

public static class BuffmageLogicChecks
{
    static int checks;
    static void Check(bool condition, string message)
    { checks++; if (!condition) throw new Exception(message); }
    public static void Main()
    {
        var charge = new ChargeTimingSettings();
        Check(ActionTimingRules.Charge(0, charge) == TimingEventResult.Early, "A quick tap still throws an early book.");
        Check(ActionTimingRules.Charge(.899f, charge) == TimingEventResult.Early, "Before gold is early.");
        Check(ActionTimingRules.Charge(.9f, charge) == TimingEventResult.Good, "Perfect zone includes its opening boundary.");
        Check(ActionTimingRules.Charge(1.15f, charge) == TimingEventResult.Good, "Center of charge window is perfect.");
        Check(ActionTimingRules.Charge(1.4f, charge) == TimingEventResult.Good, "Perfect zone includes its closing boundary.");
        Check(ActionTimingRules.Charge(1.401f, charge) == TimingEventResult.Late, "After gold is overcharged.");
        Check(ActionTimingRules.Charge(charge.Maximum, charge) == TimingEventResult.Late, "Auto-resolution is overcharged.");
        Check(charge.End - charge.Start >= .4f, "Default perfect window is comfortable on touch.");
        Check(ActionTimingRules.ChargeMultiplier(.8f, charge) > ActionTimingRules.ChargeMultiplier(.1f, charge), "Holding builds early power.");
        Check(ActionTimingRules.ChargeMultiplier(1.1f, charge) > ActionTimingRules.ChargeMultiplier(0, charge), "Perfect beats early.");
        Check(ActionTimingRules.ChargeMultiplier(1.1f, charge) > ActionTimingRules.ChargeMultiplier(2.2f, charge), "Perfect beats overcharge.");
        charge.earlyMinimumMultiplier = 10; charge.earlyMaximumMultiplier = -1; charge.overchargedMultiplier = 10;
        Check(ActionTimingRules.ChargeMultiplier(0, charge) <= ActionTimingRules.ChargeMultiplier(.8f, charge), "Misconfigured early bounds still build power.");
        Check(ActionTimingRules.ChargeMultiplier(2, charge) < charge.perfectMultiplier, "Overcharge cannot exceed perfect through tuning mistakes.");
        charge.maximumHoldSeconds = -5; charge.perfectStartSeconds = -2; charge.perfectEndSeconds = -1;
        Check(charge.Maximum >= .1f && charge.Start > 0 && charge.End <= charge.Maximum, "Invalid durations are bounded.");

        var sweep = new SweepTimingSettings();
        Check(ActionTimingRules.Sweep(.57f, sweep) == TimingEventResult.Early, "Meteor early tap.");
        Check(ActionTimingRules.Sweep(.58f, sweep) == TimingEventResult.Good, "Meteor opening edge.");
        Check(ActionTimingRules.Sweep(.7f, sweep) == TimingEventResult.Good, "Meteor perfect tap.");
        Check(ActionTimingRules.Sweep(.82f, sweep) == TimingEventResult.Good, "Meteor closing edge.");
        Check(ActionTimingRules.Sweep(.83f, sweep) == TimingEventResult.Late, "Meteor late tap.");
        sweep.durationSeconds = 0; sweep.perfectStart = 2; sweep.perfectEnd = -1;
        Check(sweep.Duration > 0 && sweep.End >= sweep.Start && sweep.End <= 1, "Malformed sweep settings stay bounded.");

        var rapid = new RapidInputSession(2.2f, 6, .12f);
        Check(rapid.TryPress(0), "First punch is accepted.");
        Check(!rapid.TryPress(.01f), "Repeated events cannot bypass the punch interval.");
        for (int i = 1; i <= 5; i++) Check(rapid.TryPress(i * .2f), "A deliberate press contributes one punch.");
        Check(!rapid.TryPress(1.4f) && rapid.Presses == 6, "Punch damage has a hard six-hit cap.");
        Check(!new RapidInputSession(2, 6, .12f).TryPress(2), "No punches after the action window.");
        var bounded = new RapidInputSession(-1, 1000, -1);
        Check(bounded.Duration > 0 && bounded.MaximumPresses == 30 && bounded.MinimumInterval >= .05f, "Rapid-input tuning has hard safety bounds.");

        var cooldowns = new SkillCooldownLedger();
        cooldowns.Start(17, 0); Check(cooldowns.Remaining(17) == 0, "Fireball remains the zero-cooldown basic action.");
        cooldowns.Start(18, 1); cooldowns.Start(19, 2); cooldowns.TickRound();
        Check(cooldowns.Remaining(18) == 1 && cooldowns.Remaining(19) == 2, "Cooldowns block the intended next rounds.");
        cooldowns.TickRound(); Check(cooldowns.Remaining(18) == 0 && cooldowns.Remaining(19) == 1, "One-round cooldown expires first.");
        cooldowns.TickRound(); Check(cooldowns.Remaining(19) == 0, "Two-round cooldown expires.");
        cooldowns.Start(19, 2); cooldowns.Clear(); Check(cooldowns.Remaining(19) == 0, "Battle reset clears cooldowns.");

        Check(HeroAvailability.IsAvailable(false, true, true, false), "Editor testing unlock works.");
        Check(HeroAvailability.IsAvailable(false, true, false, true), "Development Build testing unlock works.");
        Check(!HeroAvailability.IsAvailable(false, true, false, false), "Testing never unlocks a release build.");
        Check(!HeroAvailability.IsAvailable(false, false, true, true), "Disabling the flag restores locked behavior.");
        Check(HeroAvailability.IsAvailable(true, false, false, false), "A permanent quest unlock works in production.");
        Console.WriteLine("PASS: " + checks + " Buffmage timing, rapid-input, cooldown, and unlock-policy checks.");
    }
}
