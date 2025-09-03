using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClassMastery
{
    // One entry per class; we only need Knight now
    public int knightLevel = 1;
    public float knightXP = 0f;

    // Per-skill ranks (SkillType -> 1..5)
    [Serializable] public class SkillRank { public SkillType skill; public int rank = 1; public float xp = 0f; }
    public List<SkillRank> skillRanks = new();

    const float PerSkillRankXP = 100f;     // tune
    static readonly float[] KnightLevelXP = { 0, 120, 240, 420, 650, 950, 1350, 1850, 2450, 3150, 4000 }; // MLv1..10

    public int GetSkillRank(SkillType s)
    {
        var r = skillRanks.Find(x => x.skill == s);
        if (r == null) { r = new SkillRank { skill = s, rank = 1, xp = 0 }; skillRanks.Add(r); }
        return r.rank;
    }

    public void AddKnightXP(float amount)
    {
        knightXP += amount;
        while (knightLevel < 10 && knightXP >= KnightLevelXP[knightLevel])
            knightLevel++;
    }

    public void AddSkillXP(SkillType s, float amount)
    {
        var r = skillRanks.Find(x => x.skill == s);
        if (r == null) { r = new SkillRank { skill = s, rank = 1, xp = 0 }; skillRanks.Add(r); }

        r.xp += amount;
        while (r.rank < 5 && r.xp >= PerSkillRankXP * r.rank)
            r.rank++;
    }
}
