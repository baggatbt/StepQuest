/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRepository : MonoBehaviour
{
    public GameObject swordWavePrefab;

    public static Dictionary<SkillType, Skill> AvailableSkills { get; private set; }

   public enum SkillType
    {
        None,
        Slash,
        TripleHit,
        SwordWave,
        GuardSkill,
        ShieldSlam,
        SlimeAttackSkill
    }

    private void Awake()
    {

        AvailableSkills = new Dictionary<SkillType, Skill>
        {
            { SkillType.Slash, new Slash() },
            { SkillType.TripleHit, new TripleHitSkill() },
            { SkillType.SwordWave, new SwordWave() }, // Use the initialized reference here.
            { SkillType.GuardSkill, new GuardSkill() },
            { SkillType.ShieldSlam, new ShieldSlam() },
            { SkillType.Slime, new ShieldSlam() }
        };
    }
}

*/