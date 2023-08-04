using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class SkillSlotButton : MonoBehaviour
{
    public int slotIndex;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AssignSkillToSlot);
    }

    public void AssignSkillToSlot()
    {
        if (SkillAssignmentManager.Instance.TryAssignSelectedSkillToSlot(slotIndex))
        {
            Debug.Log("Assigned the skill");
        }
        else
        {
            // Feedback for unsuccessful assignment, maybe play a sound or show a message.
        }
    }
}
