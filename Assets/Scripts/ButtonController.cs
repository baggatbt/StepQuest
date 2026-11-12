using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StepQuest.Adventure;

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
    readonly Dictionary<Button, Skill> buttonSkills = new Dictionary<Button, Skill>();
    readonly Dictionary<Button, TMP_Text> cooldownLabels = new Dictionary<Button, TMP_Text>();
    readonly Dictionary<Button, Image> skillArtwork = new Dictionary<Button, Image>();
    RpgUi rpgUi;

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
        foreach (var pair in buttonSkills)
        {
            if (pair.Key == null) continue;
            var owner = activeCompanion;
            int remaining = owner != null ? owner.SkillCooldownRemaining(pair.Value.Type) : 0;
            pair.Key.interactable = owner != null && battleManager.CanPlanAction(owner)
                && owner.energy >= pair.Value.energyCost && remaining == 0;
            if (skillArtwork.TryGetValue(pair.Key, out var artwork) && artwork != null)
                artwork.color = pair.Key.interactable ? Color.white : new Color(.45f, .45f, .45f);
            if (cooldownLabels.TryGetValue(pair.Key, out var label) && label != null)
                label.text = remaining > 0 ? "CD " + remaining : "MP " + pair.Value.energyCost;
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
        buttonSkills.Clear(); cooldownLabels.Clear(); skillArtwork.Clear();

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
            buttonSkills[buttonComponent] = capturedSkill;

            Image buttonImage = newButtonObj.GetComponent<Image>();
            if (buttonImage != null && capturedSkill.iconImage != null)
            {
                buttonImage.sprite = capturedSkill.iconImage;
            }
            if (activeCompanion is Buffmage && buttonImage != null)
            {
                var canvas = buttonComponent.GetComponentInParent<Canvas>();
                if (rpgUi == null) rpgUi = new RpgUi(Mathf.Max(1, ((RectTransform)canvas.rootCanvas.transform).rect.width / 480f));
                // Legacy skill prefabs carry a large local scale. Size their
                // text/frame in local button units to avoid scaling twice.
                rpgUi.Scale = Mathf.Max(.1f, buttonComponent.GetComponent<RectTransform>().rect.width / 96f);
                foreach (var oldText in newButtonObj.GetComponentsInChildren<TMP_Text>(true)) oldText.gameObject.SetActive(false);
                rpgUi.ApplyButton(buttonComponent, RpgButtonKind.Secondary);
                var icon = RpgUi.Rect("Skill artwork", buttonComponent.transform).gameObject.AddComponent<Image>();
                icon.sprite = capturedSkill.iconImage; icon.preserveAspect = true; icon.raycastTarget = false;
                skillArtwork[buttonComponent] = icon;
                icon.rectTransform.anchorMin = new Vector2(.12f, .26f); icon.rectTransform.anchorMax = new Vector2(.88f, .92f);
                var cost = rpgUi.Text(buttonComponent.transform, "MP " + capturedSkill.energyCost, 22);
                cost.alignment = TextAlignmentOptions.Center;
                cost.rectTransform.anchorMin = new Vector2(.08f, .04f); cost.rectTransform.anchorMax = new Vector2(.92f, .29f);
                cooldownLabels[buttonComponent] = cost;
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
        if (skillToQueue == null || !battleManager.CanPlanAction(battleManager.activePlayer)
            || !battleManager.activePlayer.IsSkillReady(skillToQueue)) return;

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
        if (rpgUi != null)
        {
            var frame = skillDescriptionPanel != null ? skillDescriptionPanel.GetComponent<Image>() : null;
            if (frame != null)
            {
                rpgUi.Scale = Mathf.Max(.1f, frame.rectTransform.rect.width / 420f);
                rpgUi.ApplyFrame(frame, rpgUi.Window);
            }
            if (skillTitleText != null) { skillTitleText.font = rpgUi.Font; skillTitleText.color = rpgUi.Theme.light; }
            if (skillDescriptionText != null) { skillDescriptionText.font = rpgUi.Font; skillDescriptionText.color = rpgUi.Theme.muted; }
        }

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
    void OnDestroy() { rpgUi?.Dispose(); }

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
