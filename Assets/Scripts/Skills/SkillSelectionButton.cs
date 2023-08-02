using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

[RequireComponent(typeof(Button))]
public class SkillSelectionButton : MonoBehaviour
{
    public SkillType thisSkillType;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectSkill);
    }

    private void SelectSkill()
    {
        SkillAssignmentManager.Instance.SetSelectedSkill(thisSkillType);
    }
}