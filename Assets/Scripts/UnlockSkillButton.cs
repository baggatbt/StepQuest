using UnityEngine;
using UnityEngine.UI;

//Add this to the skill button, then set the onclick function to call the parameters set on the button
public class UnlockSkillButton : MonoBehaviour
{
    // Reference to the companion whose skill will be unlocked
    public Companion companion;

    // The type of skill to unlock
    public SkillType skillType;

    // Method to be called when the button is clicked
    public void OnButtonClick()
    {
        Debug.Log("clicked button");
        // Check if the companion reference is set
        if (companion != null && GameManager.Instance.currentCompanion.heroSkillPoints >= 1)
        {
            // Call the UnlockSkill method of the companion
            companion.UnlockSkill(skillType);
            GameManager.Instance.currentCompanion.heroSkillPoints -= 1;
            Debug.Log("unlocked: " + skillType);
        }
        else
        {
            Debug.LogError("Companion reference is not set.");
        }
    }
}
