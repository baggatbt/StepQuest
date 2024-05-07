using UnityEngine;
using UnityEngine.UI;

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
