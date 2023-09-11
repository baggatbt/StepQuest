using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    
    public BattleManager battleManager;
    public GameObject skillButtonPrefab;

    public List<Button> skillButtons = new List<Button>();

    

    public GameObject skillSelectionPanel;
   
    private Skill skill;
    private bool isAwaitingConfirmation = false;

    void Start()
    {
       PopulateSkillPanelWithPlayerSkills();
    }
    
    public void PopulateSkillPanelWithPlayerSkills()
{
    List<SkillType> availableSkills = PlayerData.Instance.CurrentJob.AvailableSkills;

    foreach (SkillType skillType in availableSkills)
    {
        // Instantiate a new button
        GameObject newButtonObj = Instantiate(skillButtonPrefab, skillSelectionPanel.transform);
        
        // Get the Button component
        Button buttonComponent = newButtonObj.GetComponent<Button>();
        if (buttonComponent != null)
        {
            skillButtons.Add(buttonComponent); // Add the button to the list
            
            // Get the Skill instance from the CurrentJob based on skillType
            Skill currentSkill = PlayerData.Instance.CurrentJob.GetSkillInstance(skillType);

            // Set the button's text to the skill's name
            TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = currentSkill.skillName;
            }

            // Add a listener to the button to handle its click action
            buttonComponent.onClick.AddListener(() => 
            {
                SelectAndUseSkill(currentSkill);
            });
        }
    }
}


public void SelectAndUseSkill(Skill selectedSkill)
{
    skill = selectedSkill;
    OnButtonClick();
}

        


    public void OnButtonClick()
{
    Debug.Log("Button clicked for skill: " + skill.skillName);

    if(skill.energyCost <= battleManager.player.energy)
    {
        skillSelectionPanel.SetActive(false);
        if (!battleManager.player.isAttacking && !battleManager.IsAnyEnemyAttacking())
        {
            if (!battleManager.isSkillSelected)
            {
                // Set skill to the temporary variable and set the flag
                battleManager.player.SpendEnergy(skill.energyCost);
                Debug.Log("Energy after deduction: " + battleManager.player.energy);
                battleManager.skillQueue.Enqueue(skill);
                

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
    // Set the skill
    if (skill != null)
    {
        OnButtonClick();
    }
    else
    {
        //Default skill to use if nothing selected, eventually needs to be assigned based on player job.
        skill = new Slash();
        Debug.Log("Using Slash.");
        OnButtonClick();
    }

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
