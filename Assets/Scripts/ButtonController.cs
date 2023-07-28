using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public BattleManager battleManager;
    public SkillType skillType; // Skill type that this button will trigger.
    public GameObject swordWavePrefab; // assign this in the Inspector


    public enum SkillType
    {
        Slash,
        TripleHit, // Add more as needed...
        SwordWave,
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

            case SkillType.SwordWave:
                skill = new SwordWave(swordWavePrefab);
                break;


            // Add more cases as needed...

            default:
                Debug.LogError("Invalid skill type: " + skillType);
                break;
        }
    }


    private Skill requestedSkill; // This skill is the one that the player chooses next during an ongoing attack.

   public void OnButtonClick()
{
    if ((!battleManager.player.isAttacking && !battleManager.IsAnyEnemyAttacking()) || battleManager.player.currentSkill.canChain)
    {
        // Set the skill to execute and start the attack
        battleManager.player.currentSkill = skill;
        battleManager.PlayerAttack();
    }
    else if (battleManager.player.isAttacking) // If the player is currently attacking and chooses another skill
    {
        battleManager.requestedSkill = skill;
    }
}


}