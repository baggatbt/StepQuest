using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSkillSelectionPanel : MonoBehaviour
{
    public GameObject skillSelectionGUI;

    public void openSkillSelectionGUI()
    {
        skillSelectionGUI.SetActive(true);
    }

    public void closeSkillSelectionGUI()
    {
        skillSelectionGUI.SetActive(false);
    }
   
}
