using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject knightSkillPanel;
    public GameObject wizardSkillPanel;
    public GameObject adventurerSkillPanel;
    public GameObject missionGUI;
    public GameObject companionGUI;
    public GameObject characterGUI;
    public GameObject overworldMapGUI;
    public TextMeshProUGUI stepsText;
    public TextMeshProUGUI knightHPText;
    public TextMeshProUGUI knightMPText;
    public TextMeshProUGUI knightAtkText;
    public TextMeshProUGUI knightDefText;
    public TextMeshProUGUI knightSpdText;
    public TextMeshProUGUI knightSPText;
    
    private void Awake()
    {
        UpdateUI();
    }
    private void UpdateUI()
    {  
        if (stepsText != null) stepsText.text =  PlayerData.Instance.inGameSteps.ToString();  
        UpdateKnightStatsUI();

    }
    

    public void UpdateKnightStatsUI()
    {
       
        Knight knight = GameManager.Instance.knight;
        
        if (knight != null)
        {
            if (knightHPText != null) knightHPText.text = "HP: " + knight.maxHealth.ToString();
            if (knightMPText != null) knightMPText.text = "MP: " + knight.maxEnergy.ToString();
            if (knightAtkText != null) knightAtkText.text = "ATK: " + knight.attackPower.ToString();
            if (knightDefText != null) knightDefText.text = "DEF: " + knight.defensePower.ToString();
            if (knightSpdText != null) knightSpdText.text = "SPD: " + knight.speed.ToString();
            if (knightSPText != null) knightSPText.text = "SP: " + knight.heroSkillPoints.ToString(); 
        }
        else
        {
            // Handle the case where knightData is null (e.g., clear the text or show default values)
        }
    }

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
