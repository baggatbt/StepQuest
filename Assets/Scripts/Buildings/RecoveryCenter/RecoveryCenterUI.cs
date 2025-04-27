using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecoveryCenterUI : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private RecoveryCenter  recoveryCenter;
    [SerializeField] private TMP_Dropdown    heroDropdown;
    [SerializeField] private Slider          staminaSlider;
    [SerializeField] private Button          confirmButton;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private TextMeshProUGUI resultLabel;

    private Companion selectedHero;

    // ─────────────────────────────────────────────────────────────
   // RecoveryCenterUI.cs  – replace Start() with OnEnable()
private void OnEnable()
{
    PopulateDropdown();                            // build list
    heroDropdown.onValueChanged.AddListener(OnHeroChanged);
    staminaSlider.onValueChanged.AddListener(UpdateCostLabel);
    confirmButton.onClick.AddListener(OnConfirm);

    if (heroDropdown.options.Count > 0)
        OnHeroChanged(0);
    else
        DisableUI();
}


    // ─────────────────────────────────────────────────────────────
    private void PopulateDropdown()
    {
        heroDropdown.ClearOptions();

        var heroes = GameManager.Instance.currentParty;   // ← now using currentParty
        foreach (var h in heroes)
            heroDropdown.options.Add(new TMP_Dropdown.OptionData(h.heroID));

        heroDropdown.RefreshShownValue();
    }

    private void OnHeroChanged(int index)
    {
        var heroes = GameManager.Instance.currentParty;
        if (index < 0 || index >= heroes.Count) return;   // safety check

        selectedHero = heroes[index];

        int missing = selectedHero.maxStamina - selectedHero.stamina;
        staminaSlider.maxValue = missing;
        staminaSlider.value    = missing;                 // default to full restore
        UpdateCostLabel(staminaSlider.value);
    }

    private void UpdateCostLabel(float sliderValue)
    {
        int costPerPoint = recoveryCenter.StepCostCurrentLevel;
        int totalCost    = Mathf.RoundToInt(sliderValue) * costPerPoint;
        costLabel.text   = $"{totalCost} Steps";
    }

    private void OnConfirm()
    {
        if (selectedHero == null) return;
        Debug.Log("Confirm pressed");   
        int amount = Mathf.RoundToInt(staminaSlider.value);
        if (amount <= 0) return;

        if (recoveryCenter.TryRecover(selectedHero, amount))
            resultLabel.text = $"Restored {amount} stamina!";
        else
            resultLabel.text = "Not enough steps!";
    }

    private void DisableUI()
    {
        staminaSlider.interactable = false;
        confirmButton.interactable = false;
        costLabel.text   = "—";
        resultLabel.text = "No heroes in party";
    }
}
