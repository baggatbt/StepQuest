using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public BattleManager battleManager;
    public SkillType skillType; // Skill type that this button will trigger.

    public enum SkillType
    {
        Slash,
        TripleHit, // Add more as needed...
    }

    private Skill skill;

    void Awake()
    {
        switch (skillType)
        {
            case SkillType.Slash:
                skill = new Slash();
                break;

            case SkillType.TripleHit:
                skill = new TripleHitSkill();
                break;

            // Add more cases as needed...

            default:
                Debug.LogError("Invalid skill type: " + skillType);
                break;
        }
    }

    public void OnButtonClick()
    {
        if (!battleManager.player.isAttacking && !battleManager.enemy.isAttacking)
        {
            battleManager.player.currentSkill = skill;
            battleManager.PlayerAttack();
        }
    }
}