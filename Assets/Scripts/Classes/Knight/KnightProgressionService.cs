
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class KnightProgressionService
{
    // Simple bookkeeping so we don’t double-unlock. Just store ids in PlayerPrefs.
    static HashSet<string> applied = new();

    static bool WasApplied(string id)
    {
        if (applied.Contains(id)) return true;
        if (PlayerPrefs.GetInt("KP_" + id, 0) == 1) { applied.Add(id); return true; }
        return false;
    }
    static void MarkApplied(string id) { applied.Add(id); PlayerPrefs.SetInt("KP_" + id, 1); }

    public static void TryApply(CharacterData cd)
    {
        if (cd.heroID != "Knight") return;
        var m = cd.classMastery;
        if (m == null) return;

        // MLv 2: unlock TripleHit
        ApplyAt(() => m.knightLevel >= 2, "K.M2.Triple", () => cd.UnlockSkill(SkillType.TripleHit));

        // MLv 3 choice gate – store chosen path once the player picks it (UI sets PlayerPrefs "KPath" = "A" or "B")
        string path = PlayerPrefs.GetString("KPath", ""); // "" = not chosen yet
        if (!string.IsNullOrEmpty(path) && m.knightLevel >= 3)
        {
            if (path == "A") ApplyAt(() => true, "K.M3A.Slam", () => cd.UnlockSkill(SkillType.ShieldSlam));
            if (path == "B") ApplyAt(() => true, "K.M3B.SBreak", () => cd.UnlockSkill(SkillType.SpeedBreak));
        }

        // MLv 4: Riposte passive (boost reflect % a bit)
        ApplyAt(() => m.knightLevel >= 4, "K.M4.Riposte", () => {
            cd.damageReflectionPercentage += 0.05f; // +5%
        });

        // MLv 5: Sword Wave
        ApplyAt(() => m.knightLevel >= 5, "K.M5.SwordWave", () => cd.UnlockSkill(SkillType.SwordWave));

        // MLv 6: branch passives
        if (path == "A")
            ApplyAt(() => m.knightLevel >= 6, "K.M6A.Bulwark", () => {/* widen block window / reduce dmg in your timing code */});
        if (path == "B")
            ApplyAt(() => m.knightLevel >= 6, "K.M6B.Vanguard", () => {/* widen crit window / add opener bonus */});

        // MLv 8: Heroic Guard
        ApplyAt(() => m.knightLevel >= 8, "K.M8.HeroicGuard", () => cd.UnlockSkill(SkillType.GuardSkill));

        // MLv 10: Oathbreaker ultimate
        ApplyAt(() => m.knightLevel >= 10, "K.M10.Oathbreaker", () => {/* unlock ultimate skill variant or tag */});
    }

    static void ApplyAt(System.Func<bool> cond, string id, System.Action apply)
    {
        if (WasApplied(id)) return;
        if (!cond()) return;
        apply?.Invoke();
        cdSave();
        MarkApplied(id);

        void cdSave() { /* no-op; caller saved via cd.UnlockSkill -> SaveData() */ }
    }
}
