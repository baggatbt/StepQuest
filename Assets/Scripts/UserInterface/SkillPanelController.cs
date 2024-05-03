using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelController : MonoBehaviour
{
    public GameObject skillButtonPrefab; // Assign in the inspector
    public Transform skillContainer; // Assign the SkillContainer transform in the inspector

    private Companion currentCompanion; // This will hold the reference to the current companion

    // Call this function to open the skill panel and populate it with skills
    public void SkillPanelOpen(Companion companion)
    {
        currentCompanion = companion;
        PopulateSkills();
    }

    // Creates a skill button for each skill the companion has
    private void PopulateSkills()
{
    // Clear existing buttons first
    foreach (Transform child in skillContainer)
    {
        Destroy(child.gameObject);
    }

    // Generate new buttons
    foreach (SkillType skillType in currentCompanion.AllSkills)
    {
        GameObject newButton = Instantiate(skillButtonPrefab, skillContainer);
        // Use GetComponentInChildren to find the TextMeshPro component in the new button
        var textMesh = newButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = skillType.ToString(); // Set the text of the TMP component
        }
        else
        {
            Debug.LogError("TextMeshPro component not found on the skill button!");
        }
        Skill skill = currentCompanion.GetSkillInstance(skillType);
        // Additional setup for the button, like adding listeners, can go here
    }
}


  
}
