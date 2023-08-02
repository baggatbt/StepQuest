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
    Debug.Log("Button clicked for skill: " + skill.skillName);

    if(skill.energyCost <= battleManager.player.energy)
    {
        if (!battleManager.player.isAttacking && !battleManager.IsAnyEnemyAttacking())
        {
           battleManager.player.SpendEnergy(skill.energyCost);
            Debug.Log("Energy after deduction: " + battleManager.player.energy);

            battleManager.skillQueue.Enqueue(skill); 
            Debug.Log("Skill " + skill.skillName + " added to queue. Current queue size: " + battleManager.skillQueue.Count);

            // Maybe give some visual feedback that the skill is queued?
        }
        else 
        {
            Debug.Log("Cannot queue skill due to some condition (isAttacking or IsAnyEnemyAttacking).");
        }
    }
    else
    {
        Debug.Log("Not enough energy to queue this skill!");
    }
}


}
