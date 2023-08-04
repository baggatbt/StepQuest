using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillAssignmentManager : MonoBehaviour
{
    public static SkillAssignmentManager Instance;
    public TextMeshProUGUI assignedSkill1Text;
    public TextMeshProUGUI assignedSkill2Text;
    public TextMeshProUGUI assignedSkill3Text;
   // Dont have a 4th yet public TextMeshProUGUI assignedSkill4Text;

    // This list holds the assignments.
    public List<SkillType> assignedSkills = new List<SkillType>();

    void Awake()
    {
        Instance = this;
    }

    public Skill GetSkillForSlot(int slotIndex)
{
    if (slotIndex >= 0 && slotIndex < assignedSkills.Count)
    {
        SkillType type = assignedSkills[slotIndex];
        if(type == SkillType.None)
        {
            return null;
        }
        if(SkillRepository.AvailableSkills.ContainsKey(type))
        {
            return SkillRepository.AvailableSkills[type];
        }
        else
        {
            Debug.LogError("SkillType " + type + " not found in the repository!");
            return null;
        }
    }
    return null;
}

    private SkillType? currentlySelectedSkill = null;

    public void SetSelectedSkill(SkillType skillType)
    {
        currentlySelectedSkill = skillType;
    }

    public bool TryAssignSelectedSkillToSlot(int slotIndex)
{
    if (currentlySelectedSkill.HasValue)
    {
        while (assignedSkills.Count <= slotIndex) // Ensure list size.
        {
            assignedSkills.Add(SkillType.None); // Assuming 'None' is a default value in your SkillType enum.
        }
        
        assignedSkills[slotIndex] = currentlySelectedSkill.Value;
        
        // Update the slot's text.
        UpdateSkillText(slotIndex);

        currentlySelectedSkill = null; // Reset.
        return true;
    }
    return false;
}

private void UpdateSkillText(int slotIndex)
{
    Skill assignedSkill = GetSkillForSlot(slotIndex);
    if (assignedSkill == null) return;

    switch (slotIndex)
    {
        case 0:
            if (assignedSkill1Text != null)
                assignedSkill1Text.text = assignedSkill.skillName;
            break;
        case 1:
            if (assignedSkill2Text != null)
                assignedSkill2Text.text = assignedSkill.skillName;
            break;
        case 2:
            if (assignedSkill3Text != null)
                assignedSkill3Text.text = assignedSkill.skillName;
            break;
        // Add more cases as you add more slots
    }
}



}

