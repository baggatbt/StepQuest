using UnityEngine;

public static class ProgressionEvents
{
    // Call this after a player Knight skill resolves
    public static void OnKnightSkillUsed(Character user, SkillType usedSkill, TimingEventResult result)
    {
        if (user is Companion comp && comp.characterData != null && comp.heroID == "Knight")
        {
            var mastery = comp.characterData.classMastery;
            if (mastery == null) return;

            float baseXP = 8f;                       // per skill use – tweak to taste
            float timingBonus = (result == TimingEventResult.Good) ? 4f : 0f;

            mastery.AddKnightXP(baseXP + timingBonus);
            mastery.AddSkillXP(usedSkill, 6f + timingBonus);

            comp.characterData.SaveData();
        }
    }
}
