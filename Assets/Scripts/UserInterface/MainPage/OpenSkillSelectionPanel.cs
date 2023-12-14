using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSkillSelectionPanel : MonoBehaviour
{
    public GameObject knightSkillPanel;
    public GameObject wizardSkillPanel;
    public GameObject adventurerSkillPanel;
    public GameObject missionGUI;
    public GameObject companionGUI;
    public GameObject characterGUI;
    public GameObject overworldMapGUI;

    public void openSkillSelectionGUI()
    {
        //Case: if adventurer, adventurerskillpanel .setActive
        //Case: if class = knight, knightskillpanel . set active
        //etc etc
        switch (PlayerData.Instance.heroID)
        {
            case "Knight":
                knightSkillPanel.SetActive(true);
                break;
            case "Wizard":
                adventurerSkillPanel.SetActive(true);
                break;
            default:
                Debug.LogError("Unknown job class: ");
                break;
        }
        
    }

    public void OpenKnightSkillPanel()
    {
        knightSkillPanel.SetActive(true);
    }

    public void OpenWizardSkillPanel()
    {
        wizardSkillPanel.SetActive(true);
    }

    public void closeSkillSelectionGUI()
    {
        knightSkillPanel.SetActive(false);
        adventurerSkillPanel.SetActive(false);
        wizardSkillPanel.SetActive(false);
    }

    public void openMissionInterface()
    {
        missionGUI.SetActive(true);
    }

    public void closeMissionInterface()
    {
        missionGUI.SetActive(false);
    }

    public void openCompanionInterface()
    {
        companionGUI.SetActive(true);
    }

    public void closeCompanionInterface()
    {
        companionGUI.SetActive(false);
    }

    public void openCharacterInterface()
    {
        characterGUI.SetActive(true);
    }

    public void closeCharacterInterface()
    {
        characterGUI.SetActive(false);
    }

    public void openOverworldMapInterface()
    {
        overworldMapGUI.SetActive(true);
    }

    public void closeOverworldMapInterface()
    {
        overworldMapGUI.SetActive(false);
    }
   
}
