using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    
    public BattleManager battleManager;
    public GameObject skillButtonPrefab;
    public Companion companion;

    public List<Button> skillButtons = new List<Button>();

    

    public GameObject skillSelectionPanel;
    public GameObject companionSkillSelectionPanel;
   
    private Skill skill;
    

    void Start()
    {
       PopulateSkillPanelWithPlayerSkills();
       PopulateSkillPanelWithCompanionSkills();
    }

    void Update()
    {
        if (battleManager.activePlayer == battleManager.player)
        {
            companionSkillSelectionPanel.SetActive(false);
            skillSelectionPanel.SetActive(true);
        }

        if (battleManager.activePlayer == battleManager.companion)
        {
            companionSkillSelectionPanel.SetActive(true);
            skillSelectionPanel.SetActive(false);
        }

    }
    
    private UnityEngine.Events.UnityAction GetSkillAction(Skill currentSkill)
{
    return () => { SelectAndUseSkill(currentSkill); };
}

public void PopulateSkillPanelWithPlayerSkills()
{
    List<SkillType> availableSkills = PlayerData.Instance.CurrentJob.AvailableSkills;
   // will need to modify playerdata and handling List<SkillType> availableSkills2 = PlayerData.Instance.CurrentJob2.AvailableSkills;


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

            // Create a new local variable to hold the currentSkill value
            Skill buttonSkill = currentSkill;

            // Add a listener to the button to handle its click action
            buttonComponent.onClick.AddListener(() => 
            {
                SelectAndUseSkill(buttonSkill);
            });
        }
    }
}

public void PopulateSkillPanelWithCompanionSkills()
{
    List<SkillType> availableSkills = companion.AvailableSkills;
   


    foreach (SkillType skillType in availableSkills)
    {
        // Instantiate a new button
        GameObject newButtonObj = Instantiate(skillButtonPrefab, companionSkillSelectionPanel.transform);
        Debug.Log("Tried to make skill buttons fam");
        // Get the Button component
        Button buttonComponent = newButtonObj.GetComponent<Button>();
        if (buttonComponent != null)
        {
            skillButtons.Add(buttonComponent); // Add the button to the list
            
            // Get the Skill instance from the CurrentJob based on skillType
            Skill currentSkill = companion.GetSkillInstance(skillType);

            // Set the button's text to the skill's name
            TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = currentSkill.skillName;
            }

            // Create a new local variable to hold the currentSkill value
            Skill buttonSkill = currentSkill;

            // Add a listener to the button to handle its click action
            buttonComponent.onClick.AddListener(() => 
            {
                SelectAndUseSkill(buttonSkill);
            });
        }
    }
}



public void SelectAndUseSkill(Skill selectedSkill)
{
    if (battleManager.skillQueue.Count == 0)
    {
        //No skill has been selected yet
         skill = selectedSkill;
         battleManager.skillQueue.Enqueue(skill);
         OnButtonClick();
    }
    else
    { //Replacing the old skill
    battleManager.skillQueue.Dequeue();
    skill = selectedSkill;
    battleManager.skillQueue.Enqueue(skill);
    OnButtonClick();
    }
    
}

        


    public void OnButtonClick()
{
    Debug.Log("Button clicked for skill: " + skill.skillName);

    if(skill.energyCost <= PlayerData.Instance.teamEnergy)
    {
       // skillSelectionPanel.SetActive(false);
        if (!battleManager.activePlayer.isAttacking && !battleManager.IsAnyEnemyAttacking())
        {
            if (!battleManager.isSkillSelected)
            {
                
               

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
    public void testButtonClick() //set test button to this methd
    {
        skill = new GuardSkill();
        OnButtonClick();
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
