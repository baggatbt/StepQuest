using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAssignmentManager : MonoBehaviour
{
    public static SkillAssignmentManager Instance;

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
        currentlySelectedSkill = null; // Reset.
        return true;
    }
    return false;
}


}

