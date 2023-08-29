using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class CompanionButtonController : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public BattleManager battleManager;
    public Button[] skillButtons; // An array of buttons. Assign these in the Unity Editor.
    public int slotIndex; //This indicates which slot this button corresponds to.
   
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

    if(skill.energyCost <= battleManager.companion.energy)
    {
        if (!battleManager.companion.isAttacking && !battleManager.IsAnyEnemyAttacking())
        {
           battleManager.companion.SpendEnergy(skill.energyCost);
            Debug.Log("Energy after deduction: " + battleManager.companion.energy);

            battleManager.skillQueue.Enqueue(skill);
            // After adding skill to the queue, disable the button using its slot index
            battleManager.skillButtons[slotIndex].interactable = false;
            Debug.Log("Skill " + skill.skillName + " added to queue. Current queue size: " + battleManager.skillQueue.Count);

            // Add the skill icon to the top of the screen to show its position in queue. Eventually let the user tap it to remove it
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
