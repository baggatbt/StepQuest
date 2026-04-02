using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUpgradeButton : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CrafterGridController crafterGrid;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text costText;

    [Header("Upgrade Data")]
    [SerializeField] private BuildingUpgradeCost upgradeCost;

    [Header("Runtime")]
    [SerializeField] private int currentLevel = 1;

    private void OnEnable()
    {
        StartCoroutine(RefreshNextFrame());
    }

    private void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradePressed);
        }

        StartCoroutine(RefreshNextFrame());
    }

    private IEnumerator RefreshNextFrame()
    {
        yield return null;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (costText != null)
            costText.text = BuildCostString();

        if (upgradeButton != null && crafterGrid != null && upgradeCost != null)
        {
            bool canAfford = crafterGrid.CanAffordUpgrade(upgradeCost);
            upgradeButton.interactable = canAfford;
            Debug.Log($"[GeneratorUpgradeButton] RefreshUI -> interactable = {canAfford}");
        }
    }

    private void OnUpgradePressed()
    {
        if (crafterGrid == null || upgradeCost == null)
            return;

        bool paid = crafterGrid.TryPayUpgradeCost(upgradeCost);
        if (!paid)
        {
            Debug.Log("Not enough materials.");
            RefreshUI();
            return;
        }

        currentLevel = upgradeCost.targetLevel;
        Debug.Log($"Upgraded to level {currentLevel}");

        RefreshUI();
    }

    private string BuildCostString()
    {
        if (upgradeCost == null)
            return "No cost set";

        System.Text.StringBuilder sb = new();

        foreach (var req in upgradeCost.gridCosts)
        {
            if (req == null) continue;
            sb.AppendLine($"{req.quantity}x {req.entityType}");
        }

        foreach (var req in upgradeCost.inventoryCosts)
        {
            if (req == null || req.item == null) continue;
            sb.AppendLine($"{req.quantity}x {req.item.itemName}");
        }

        return sb.ToString().TrimEnd();
    }
}