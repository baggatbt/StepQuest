using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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
    public Knight knight;
    
    private void Awake()
    {
        knight = GameManager.Instance.knight;
        UpdateUI();
        
    }
    private void UpdateUI()
    {  
        if (stepsText != null) stepsText.text =  PlayerData.Instance.inGameSteps.ToString();  
        UpdateKnightStatsUI();
        AddKnightBtnListeners();

    }
    private void AddKnightBtnListeners()
    {
        knightHPBtn.onClick.AddListener(() => SpendStatPoint("HP"));
        knightMPBtn.onClick.AddListener(() => SpendStatPoint("MP"));
        knightATKBtn.onClick.AddListener(() => SpendStatPoint("ATK"));
        knightDEFBtn.onClick.AddListener(() => SpendStatPoint("DEF"));
        knightSPDBtn.onClick.AddListener(() => SpendStatPoint("SPD"));
    }
    void SpendStatPoint(string stat)
    {
        if (knight.heroStatPoints > 0)
        {
            switch (stat)
            {
                case "HP":
                    knight.maxHealth += 2;
                    break;
                case "MP":
                    knight.maxEnergy += 1;
                    break;
                case "ATK":
                    knight.attackPower += 1;
                    break;
                case "DEF":
                    knight.defensePower += 3;
                    break;
                case "SPD":
                    knight.speed += 1;
                    break;
            }
            knight.heroStatPoints -= 1;
        }
        UpdateUI();
        GameManager.Instance.SaveAllCompanionData();
    }
    

    public void UpdateKnightStatsUI()
    {
        if (knight != null)
        {
            if (knightHPText != null) knightHPText.text = "HP: " + knight.maxHealth.ToString();
            if (knightMPText != null) knightMPText.text = "MP: " + knight.maxEnergy.ToString();
            if (knightAtkText != null) knightAtkText.text = "ATK: " + knight.attackPower.ToString();
            if (knightDefText != null) knightDefText.text = "DEF: " + knight.defensePower.ToString();
            if (knightSpdText != null) knightSpdText.text = "SPD: " + knight.speed.ToString();
            if (knightSPText != null) knightSPText.text = "SP: " + knight.heroStatPoints.ToString(); 
        }
        else
        {
            // Handle the case where knightData is null (e.g., clear the text or show default values)
        }
    }
    public Button knightHPBtn;
    public Button knightMPBtn;
    public Button knightATKBtn;
    public Button knightDEFBtn;
    public Button knightSPDBtn;
    public void KnightSpendStatPoints()
    {
        if (knight.heroStatPoints > 0)
        {
            
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
