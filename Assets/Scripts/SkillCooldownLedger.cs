using System.Collections.Generic;

// Battle-local cooldown state; never serialized into character progression.
public sealed class SkillCooldownLedger
{
    readonly Dictionary<int, int> remaining = new Dictionary<int, int>();
    public int Remaining(int skill) => remaining.TryGetValue(skill, out var turns) ? turns : 0;
    public void Start(int skill, int turns) { if (turns > 0) remaining[skill] = turns + 1; }
    public void TickRound()
    {
        var keys = new List<int>(remaining.Keys);
        foreach (int key in keys) if (--remaining[key] <= 0) remaining.Remove(key);
    }
    public void Clear() { remaining.Clear(); }
}
