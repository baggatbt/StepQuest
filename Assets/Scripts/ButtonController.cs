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

    void Start()
    {
        activeCompanion = battleManager != null ? battleManager.activePlayer as Companion : null;
        previousActiveCompanion = activeCompanion;

        if (skillDescriptionPanel != null)
            skillDescriptionPanel.SetActive(false);

        if (skillTitleText != null)
            skillTitleText.text = "";

        if (skillDescriptionText != null)
            skillDescriptionText.text = "";
            UpdateUI();

    }

    void Update()
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
            if (companionSkillSelectionPanel != null) companionSkillSelectionPanel.SetActive(isCompanionActive);
            if (skillSelectionPanel != null) skillSelectionPanel.SetActive(!isCompanionActive);
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
            Debug.LogError("Active player is not a Companion.");
            return;
        }

        if (companionSkillSelectionPanel == null || skillButtonPrefab == null)
        {
            Debug.LogError("companionSkillSelectionPanel or skillButtonPrefab not assigned.");
            return;
        }

        foreach (Transform child in companionSkillSelectionPanel.transform)
            Destroy(child.gameObject);

        skillButtons.Clear();

        if (activeCompanion.characterData == null)
        {
            Debug.LogError($"[{name}] activeCompanion.characterData is null.");
            return;
        }

        Debug.Log($"Populating skills for {activeCompanion.heroID}. Equipped: {activeCompanion.characterData.equippedSkills.Count}");

        foreach (SkillType skillType in activeCompanion.characterData.equippedSkills)
        {
            Skill skillInstance = activeCompanion.GetSkillInstance(skillType);
            if (skillInstance == null) continue;
            if (!skillInstance.isActiveSkill) continue;

            GameObject newButtonObj = Instantiate(skillButtonPrefab, companionSkillSelectionPanel.transform);
            Button buttonComponent = newButtonObj.GetComponent<Button>();

            if (buttonComponent != null)
            {
                Skill capturedSkill = skillInstance;

                buttonComponent.onClick.AddListener(() => SelectAndUseSkill(capturedSkill));
                skillButtons.Add(buttonComponent);

                if (capturedSkill.iconImage != null)
                {
                    Image buttonImage = newButtonObj.GetComponent<Image>();
                    if (buttonImage != null) buttonImage.sprite = capturedSkill.iconImage;
                }

                Debug.Log("Button created for: " + capturedSkill.skillName);
            }
        }
    }

    public void SelectAndUseSkill(Skill skillToQueue)
    {
        if (battleManager == null || battleManager.activePlayer == null)
        {
            Debug.LogError("BattleManager/activePlayer missing.");
            return;
        }

        bool hasEnergy = (skillToQueue.energyCost <= battleManager.activePlayer.energy);

        if (battleManager.skillQueue.Count > 0 && selectedSkill == skillToQueue)
        {
            if (skillDescriptionPanel != null) skillDescriptionPanel.SetActive(false);

            if (skillTitleText != null)
                skillTitleText.text = "";

            if (skillDescriptionText != null)
                skillDescriptionText.text = "";

            Debug.Log("Same skill selected again: " + selectedSkill.skillName + " (waiting for target tap)");
            return;
        }

        if (battleManager.skillQueue.Count > 0)
        {
            battleManager.skillQueue.Clear();
            battleManager.isSkillSelected = false;
        }

        selectedSkill = skillToQueue;

        if (skillDescriptionPanel != null) skillDescriptionPanel.SetActive(true);

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

    private string BuildSkillDescription(Skill s, Character user)
    {
        if (s == null) return "";

        int finalSpeed = user != null ? s.GetEffectiveSpeed(user.speed) : 0;

        return $"POW: {s.power}   SPD: {finalSpeed}   COST: {s.energyCost}";
    }

    public void OpenSkillPanel()
    {
        if (skillSelectionPanel != null) skillSelectionPanel.SetActive(true);
    }

    public void CloseSkillPanel()
    {
        if (skillSelectionPanel != null) skillSelectionPanel.SetActive(false);
    }
}