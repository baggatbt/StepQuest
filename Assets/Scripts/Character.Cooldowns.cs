public partial class Character
{
    readonly SkillCooldownLedger skillCooldowns = new SkillCooldownLedger();
    public int SkillCooldownRemaining(SkillType type) => skillCooldowns.Remaining((int)type);
    public bool IsSkillReady(Skill skill) => skill != null && SkillCooldownRemaining(skill.Type) == 0;
    public void StartSkillCooldown(Skill skill)
    {
        if (skill != null) skillCooldowns.Start((int)skill.Type, skill.cooldownTurns);
    }
    public void TickSkillCooldowns()
    {
        skillCooldowns.TickRound();
    }
    public void ClearSkillCooldowns() { skillCooldowns.Clear(); }
}
