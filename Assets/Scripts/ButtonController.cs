using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public BattleManager battleManager;
    public Button[] skillButtons;
    public int slotIndex;

    public GameObject skillSelectionPanel;
   
    private Skill skill;
    private bool isAwaitingConfirmation = false;

    void Start()
    {
        skill = SkillAssignmentManager.Instance.GetSkillForSlot(slotIndex);
        if(skill != null )
        {
            Debug.LogError("doot");
           // buttonText.text = skill.skillName;
        }
        else
        {
            Debug.LogError("No skill assigned to slot " + slotIndex);
          //  buttonText.text = "Unassigned";
        }
    }

    public void OnButtonClick()
{
    Debug.Log("Button clicked for skill: " + skill.skillName);

    if(skill.energyCost <= battleManager.player.energy)
    {
        if (!battleManager.player.isAttacking && !battleManager.IsAnyEnemyAttacking())
        {
            if (!battleManager.isSkillSelected)
            {
                // Set skill to the temporary variable and set the flag
                battleManager.player.SpendEnergy(skill.energyCost);
                Debug.Log("Energy after deduction: " + battleManager.player.energy);
                battleManager.skillQueue.Enqueue(skill);
                battleManager.skillButtons[slotIndex].interactable = false;
                Debug.Log("Skill " + skill.skillName + " added to queue. Current queue size: " + battleManager.skillQueue.Count);
                
                // Setting flag to indicate skill has been selected
                battleManager.isSkillSelected = true;
            }
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


    public void AttackButtonClick()
{
    // Set the skill to TripleHitSkill
    skill = new TripleHitSkill();
    Debug.Log("Using TripleHitSkill.");

    // Execute the skill
    OnButtonClick();
}


    IEnumerator ResetConfirmationState()
    {
        yield return new WaitForSeconds(2);  // Waits for 2 seconds
        isAwaitingConfirmation = false;
    }



    public void OpenSkillPanel()
    {
        skillSelectionPanel.SetActive(true);
    }

    public void CloseSkillPanel()
    {
        skillSelectionPanel.SetActive(false);
    }
}
