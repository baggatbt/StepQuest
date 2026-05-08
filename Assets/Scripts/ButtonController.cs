using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    [Header("Refs")]
    public BattleManager battleManager;
    public GameObject skillButtonPrefab;

    [Header("Skill Description UI")]
    public GameObject skillDescriptionPanel;
    public TextMeshProUGUI skillTitleText;
    public TextMeshProUGUI skillDescriptionText;

    [Header("Panels")]
    public GameObject skillSelectionPanel;
    public GameObject companionSkillSelectionPanel;

    [Header("State")]
    public List<Button> skillButtons = new List<Button>();
    public Companion activeCompanion;

    private Companion previousActiveCompanion;
    private Skill selectedSkill;

    private void Start()
    {
        activeCompanion = battleManager != null ? battleManager.activePlayer as Companion : null;
        previousActiveCompanion = activeCompanion;

        ClearSkillDescription();
        UpdateUI();
    }

    private void Update()
    {
        if (battleManager == null) return;

        if (battleManager.isBattleStarted)
        {
            Companion currentActiveCompanion = battleManager.activePlayer as Companion;

            if (currentActiveCompanion != null && currentActiveCompanion != previousActiveCompanion)
            {
                previousActiveCompanion = currentActiveCompanion;
                UpdateUI();
            }

            bool isCompanionActive = battleManager.activePlayer is Companion;

            if (companionSkillSelectionPanel != null)
                companionSkillSelectionPanel.SetActive(isCompanionActive);

            if (skillSelectionPanel != null)
                skillSelectionPanel.SetActive(!isCompanionActive);
        }
    }

    private void UpdateUI()
    {
        activeCompanion = previousActiveCompanion;
        PopulateSkillPanelWithCompanionSkills();
    }

    public void PopulateSkillPanelWithCompanionSkills()
    {
        if (activeCompanion == null)
        {
            Debug.LogWarning("Active player is not a Companion yet.");
            return;
        }

        if (companionSkillSelectionPanel == null || skillButtonPrefab == null)
        {
            Debug.LogError("companionSkillSelectionPanel or skillButtonPrefab not assigned.");
            return;
        }

        foreach (Transform child in companionSkillSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        skillButtons.Clear();

        if (activeCompanion.characterData == null)
        {
            Debug.LogError($"[{name}] activeCompanion.characterData is null.");
            return;
        }

        activeCompanion.InitializeSkillsBasedOnLevel();
        activeCompanion.ValidateEquippedSkills();
        activeCompanion.SaveCharacterData();

        Debug.Log($"Populating skills for {activeCompanion.heroID}. Level: {activeCompanion.heroLevel}");
        Debug.Log($"Equipped skills: {string.Join(", ", activeCompanion.characterData.equippedSkills)}");

        foreach (SkillType skillType in activeCompanion.characterData.equippedSkills)
        {
            if (!activeCompanion.AvailableSkills.Contains(skillType))
            {
                Debug.LogWarning($"{skillType} is equipped but not unlocked. Skipping.");
                continue;
            }

            Skill skillInstance = activeCompanion.GetSkillInstance(skillType);
            if (skillInstance == null) continue;
            if (!skillInstance.isActiveSkill) continue;

            GameObject newButtonObj = Instantiate(skillButtonPrefab, companionSkillSelectionPanel.transform);
            Button buttonComponent = newButtonObj.GetComponent<Button>();

            if (buttonComponent == null)
            {
                Debug.LogError("Skill button prefab is missing a Button component.");
                continue;
            }

            Skill capturedSkill = skillInstance;

            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(() => SelectAndUseSkill(capturedSkill));
            skillButtons.Add(buttonComponent);

            Image buttonImage = newButtonObj.GetComponent<Image>();
            if (buttonImage != null && capturedSkill.iconImage != null)
            {
                buttonImage.sprite = capturedSkill.iconImage;
            }

            Debug.Log("Button created for: " + capturedSkill.skillName);
        }
    }

    public void SelectAndUseSkill(Skill skillToQueue)
    {
        if (battleManager == null || battleManager.activePlayer == null)
        {
            Debug.LogError("BattleManager/activePlayer missing.");
            return;
        }

        bool hasEnergy = skillToQueue.energyCost <= battleManager.activePlayer.energy;

        if (battleManager.skillQueue.Count > 0 && selectedSkill == skillToQueue)
        {
            ClearSkillDescription();
            Debug.Log("Same skill selected again: " + selectedSkill.skillName + " (waiting for target tap)");
            return;
        }

        if (battleManager.skillQueue.Count > 0)
        {
            battleManager.skillQueue.Clear();
            battleManager.isSkillSelected = false;
        }

        selectedSkill = skillToQueue;

        if (skillDescriptionPanel != null)
            skillDescriptionPanel.SetActive(true);

        if (skillTitleText != null)
            skillTitleText.text = skillToQueue.skillName;

        if (skillDescriptionText != null)
            skillDescriptionText.text = BuildSkillDescription(skillToQueue, battleManager.activePlayer);

        if (!hasEnergy)
        {
            Debug.Log("Not enough energy to queue: " + skillToQueue.skillName);
            battleManager.isSkillSelected = false;
            return;
        }

        battleManager.skillQueue.Enqueue(skillToQueue);
        battleManager.isSkillSelected = true;

        Debug.Log($"Queued {skillToQueue.skillName}. Queue size: {battleManager.skillQueue.Count}");
    }

    private string BuildSkillDescription(Skill skill, Character user)
    {
        if (skill == null) return "";
        return skill.GetBattlePreviewText(user);
    }

    private void ClearSkillDescription()
    {
        if (skillDescriptionPanel != null)
            skillDescriptionPanel.SetActive(false);

        if (skillTitleText != null)
            skillTitleText.text = "";

        if (skillDescriptionText != null)
            skillDescriptionText.text = "";
    }

    public void OpenSkillPanel()
    {
        if (skillSelectionPanel != null)
            skillSelectionPanel.SetActive(true);
    }

    public void CloseSkillPanel()
    {
        if (skillSelectionPanel != null)
            skillSelectionPanel.SetActive(false);
    }
}