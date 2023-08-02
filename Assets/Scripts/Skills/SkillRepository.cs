using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRepository : MonoBehaviour
{
    public GameObject swordWavePrefab;

    public static Dictionary<SkillType, Skill> AvailableSkills { get; private set; }


    private void Awake()
    {
        AvailableSkills = new Dictionary<SkillType, Skill>
        {
            { SkillType.Slash, new Slash() },
            { SkillType.TripleHit, new TripleHitSkill() },
            { SkillType.SwordWave, new SwordWave(swordWavePrefab) }
        };
    }
}

