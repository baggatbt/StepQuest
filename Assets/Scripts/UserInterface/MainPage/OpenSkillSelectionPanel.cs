using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSkillSelectionPanel : MonoBehaviour
{
    public GameObject skillSelectionGUI;
    public GameObject missionGUI;

    public void openSkillSelectionGUI()
    {
        //Case: if adventurer, adventurerskillpanel .setActive
        //Case: if class = knight, knightskillpanel . set active
        //etc etc
        skillSelectionGUI.SetActive(true);
    }

    public void closeSkillSelectionGUI()
    {
        skillSelectionGUI.SetActive(false);
    }

    public void openMissionInterface()
    {
        missionGUI.SetActive(true);
    }

    public void closeMissionInterface()
    {
        missionGUI.SetActive(false);
    }
   
}
