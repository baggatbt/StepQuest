using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRepository : MonoBehaviour
{
    public GameObject swordWavePrefab;

    public static Dictionary<SkillType, Skill> AvailableSkills { get; private set; }

    private SkillManager skillManager; // Create a reference for the SkillManager here.

    private void Awake()
    {
        skillManager = FindObjectOfType<SkillManager>(); // Initialize the reference.
        
        if(skillManager == null)
        {
            Debug.LogError("SkillManager not found in the scene!");
            return;
        }

        AvailableSkills = new Dictionary<SkillType, Skill>
        {
            { SkillType.Slash, new Slash() },
            { SkillType.TripleHit, new TripleHitSkill() },
            { SkillType.SwordWave, new SwordWave(skillManager) } // Use the initialized reference here.
        };
    }
}

