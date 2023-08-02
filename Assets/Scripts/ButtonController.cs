using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class ButtonController : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public BattleManager battleManager;
    public int slotIndex; // Add this line. This indicates which slot this button corresponds to.
   
    private Skill skill;

       void Start()
    {
        skill = SkillAssignmentManager.Instance.GetSkillForSlot(slotIndex);
        if(skill != null)
        {
            buttonText.text = skill.skillName; // Adjust based on your Skill class.
        }
        else
        {
            Debug.LogError("No skill assigned to slot " + slotIndex);
            buttonText.text = "Unassigned"; // Or another default text.
        }
    }


    public void OnButtonClick()
    {
        if (!battleManager.player.isAttacking && !battleManager.IsAnyEnemyAttacking())
        {
            // Set the skill to execute and start the attack
            battleManager.player.currentSkill = skill;
            battleManager.PlayerAction();
        }
        else 
        {
            Debug.Log("Error, no skill selected or another condition preventing skill execution.");
        }
    }
}
