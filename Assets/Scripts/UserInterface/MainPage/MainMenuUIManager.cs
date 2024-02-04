using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenuUIManager : MonoBehaviour
{
    public GameObject knightSkillPanel;
    public GameObject wizardSkillPanel;
    public GameObject archerSkillPanel;
    public GameObject adventurerSkillPanel;
    public GameObject forestTownPanel;
    public GameObject missionGUI;
    public GameObject companionGUI;
    public GameObject characterGUI;
    public GameObject overworldMapGUI;
    public TextMeshProUGUI stepsText;

    //KNIGHT 
    public Knight knight;
    public TextMeshProUGUI knightHPText;
    public TextMeshProUGUI knightMPText;
    public TextMeshProUGUI knightAtkText;
    public TextMeshProUGUI knightDefText;
    public TextMeshProUGUI knightSpdText;
    public TextMeshProUGUI knightSPText;
    public Button knightHPBtn;
    public Button knightMPBtn;
    public Button knightATKBtn;
    public Button knightDEFBtn;
    public Button knightSPDBtn;

    //ARCHER
    public Archer archer;
    public TextMeshProUGUI archerHPText;
    public TextMeshProUGUI archerMPText;
    public TextMeshProUGUI archerAtkText;
    public TextMeshProUGUI archerDefText;
    public TextMeshProUGUI archerSpdText;
    public TextMeshProUGUI archerSPText;
    public Button archerHPBtn;
    public Button archerMPBtn;
    public Button archerATKBtn;
    public Button archerDEFBtn;
    public Button archerSPDBtn;
    
    private void Awake()
    {
        knight = GameManager.Instance.knight;
        archer = GameManager.Instance.archer;
        UpdateUI();
        
    }
    private void Update()
    {
         if (stepsText != null) stepsText.text =  PlayerData.Instance.inGameSteps.ToString();  
    }
    private void UpdateUI()
    {  
        UpdateKnightStatsUI();
        UpdateArcherStatsUI();
        AddKnightBtnListeners();
        AddArcherBtnListeners();

    }
    private void AddKnightBtnListeners()
    {
        knightHPBtn.onClick.AddListener(() => SpendStatPoint("HP"));
        knightMPBtn.onClick.AddListener(() => SpendStatPoint("MP"));
        knightATKBtn.onClick.AddListener(() => SpendStatPoint("ATK"));
        knightDEFBtn.onClick.AddListener(() => SpendStatPoint("DEF"));
        knightSPDBtn.onClick.AddListener(() => SpendStatPoint("SPD"));
    }
    private void AddArcherBtnListeners()
    {
        archerHPBtn.onClick.AddListener(() => SpendStatPoint("HP"));
        archerMPBtn.onClick.AddListener(() => SpendStatPoint("MP"));
        archerATKBtn.onClick.AddListener(() => SpendStatPoint("ATK"));
        archerDEFBtn.onClick.AddListener(() => SpendStatPoint("DEF"));
        archerSPDBtn.onClick.AddListener(() => SpendStatPoint("SPD"));
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
    void SpendArcherStatPoint(string stat)
    {
        if (archer.heroStatPoints > 0)
        {
            switch (stat)
            {
                case "HP":
                    archer.maxHealth += 2;
                    break;
                case "MP":
                    archer.maxEnergy += 1;
                    break;
                case "ATK":
                    archer.attackPower += 1;
                    break;
                case "DEF":
                    archer.defensePower += 3;
                    break;
                case "SPD":
                    archer.speed += 1;
                    break;
            }
            archer.heroStatPoints -= 1;
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
    public void UpdateArcherStatsUI()
    {
        if (archer != null)
        {
            if (archerHPText != null) archerHPText.text = "HP: " + archer.maxHealth.ToString();
            if (archerMPText != null) archerMPText.text = "MP: " + archer.maxEnergy.ToString();
            if (archerAtkText != null) archerAtkText.text = "ATK: " + archer.attackPower.ToString();
            if (archerDefText != null) archerDefText.text = "DEF: " + archer.defensePower.ToString();
            if (archerSpdText != null) archerSpdText.text = "SPD: " + archer.speed.ToString();
            if (archerSPText != null) archerSPText.text = "SP: " + archer.heroStatPoints.ToString(); 
        }
        else
        {
            // Handle the case where archerData is null (e.g., clear the text or show default values)
        }
    }
    
   

    public void openSkillSelectionGUI()
    {
        
        switch (PlayerData.Instance.heroID)
        {
            case "Knight":
                knightSkillPanel.SetActive(true);
                break;
            case "Wizard":
                wizardSkillPanel.SetActive(true);
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
    public void OpenArcherSkillPanel()
    {
        archerSkillPanel.SetActive(true);
    }
    

    public void closeSkillSelectionGUI()
    {
        knightSkillPanel.SetActive(false);
        archerSkillPanel.SetActive(false);
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

    public void openForestTownPanel()
    {
        forestTownPanel.SetActive(true);
    }

    public void closeForestTownPanel()
    {
        forestTownPanel.SetActive(false);
    }
   
}
