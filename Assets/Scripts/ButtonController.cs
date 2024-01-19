using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class ButtonController : MonoBehaviour
{
    
    public BattleManager battleManager;
    public GameObject skillButtonPrefab;
    public Companion companion2;
    public Companion companion1;
    public GameObject skillDescriptionPanel; // UI Panel to show the skill description
    public TextMeshProUGUI skillDescriptionText; // Text component to show the description
    public List<Button> skillButtons = new List<Button>();
    public Companion activeCompanion;
    private Companion previousActiveCompanion; // For switching the skill panel

    

    public GameObject skillSelectionPanel;
    public GameObject companionSkillSelectionPanel;
   
    private Skill skill;
    

    void Start()
    {
       activeCompanion = battleManager.activePlayer as Companion;
    }

   void Update()
{
    if (battleManager.isBattleStarted)
    {
        // Check if the active player has changed
        Companion currentActiveCompanion = battleManager.activePlayer as Companion;
        if (currentActiveCompanion != null && currentActiveCompanion != previousActiveCompanion)
        {
            previousActiveCompanion = currentActiveCompanion;
            UpdateUI();
        }

        // UI panel activation/deactivation
        bool isCompanionActive = battleManager.activePlayer is Companion;
        companionSkillSelectionPanel.SetActive(isCompanionActive);
        skillSelectionPanel.SetActive(!isCompanionActive);
    }
}

   private void UpdateUI()
{
    activeCompanion = previousActiveCompanion;
    PopulateSkillPanelWithCompanionSkills();
}
    
    private UnityEngine.Events.UnityAction GetSkillAction(Skill currentSkill)
{
    return () => { SelectAndUseSkill(currentSkill); };
}




private IEnumerator ShowSkillDescriptionAfterDelay(Skill skill)
{
    yield return new WaitForSeconds(1.0f); // Wait for 1 second
    skillDescriptionText.text = skill.description;
    skillDescriptionPanel.SetActive(true);
}

private Coroutine holdCoroutine; // To keep track of the coroutine

private void OnSkillButtonHold(Skill skill)
{
    holdCoroutine = StartCoroutine(ShowSkillDescriptionAfterDelay(skill));
}


private void OnSkillButtonRelease()
{
    if (holdCoroutine != null)
    {
        StopCoroutine(holdCoroutine);
        holdCoroutine = null;
    }
    skillDescriptionPanel.SetActive(false);
}

 
public void PopulateSkillPanelWithCompanionSkills()
{
    // Get the active companion from the battle manager
   // Companion activeCompanion = battleManager.activePlayer as Companion;
    if (activeCompanion == null)
    {
        Debug.LogError("Active player is not a Companion.");
        return;
    }

    List<SkillType> availableSkills = activeCompanion.AvailableSkills;

    // Clear existing skill buttons
    foreach (Transform child in companionSkillSelectionPanel.transform)
    {
        Destroy(child.gameObject);
    }
    skillButtons.Clear();

    foreach (SkillType skillType in availableSkills)
    {
        GameObject newButtonObj = Instantiate(skillButtonPrefab, companionSkillSelectionPanel.transform);
        Button buttonComponent = newButtonObj.GetComponent<Button>();
        if (buttonComponent != null)
        {
            skillButtons.Add(buttonComponent);
            
            Skill currentSkill = activeCompanion.GetSkillInstance(skillType);

            TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = currentSkill.skillName;
            }

            Skill buttonSkill = currentSkill;

            buttonComponent.onClick.AddListener(() => 
            {
                SelectAndUseSkill(buttonSkill);
            });

            // Add event listeners for pointer down and up
            EventTrigger eventTrigger = newButtonObj.AddComponent<EventTrigger>();

            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnSkillButtonHold(buttonSkill); });
            eventTrigger.triggers.Add(pointerDown);

            var pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnSkillButtonRelease(); });
            eventTrigger.triggers.Add(pointerUp);
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

    if(skill.energyCost <= battleManager.activePlayer.energy)
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
