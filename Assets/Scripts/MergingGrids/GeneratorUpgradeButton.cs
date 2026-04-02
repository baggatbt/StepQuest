using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUpgradeButton : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CrafterGridController crafterGrid;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text buttonLabelText;

    [Header("Generator")]
    [SerializeField] private CrafterEntityType generatorType = CrafterEntityType.WoodGenerator;

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
        if (crafterGrid == null)
            return;

        int currentLevel = crafterGrid.GetGeneratorLevel(generatorType);
        int maxLevel = crafterGrid.GetGeneratorMaxLevel(generatorType);
        CrafterGeneratorUpgradeCostEntry nextCost = crafterGrid.GetNextGeneratorUpgradeCost(generatorType);

        if (levelText != null)
            levelText.text = $"Lv. {currentLevel}";

        bool isMaxed = currentLevel >= maxLevel || nextCost == null;

        if (buttonLabelText != null)
        {
            buttonLabelText.text = isMaxed
                ? "MAX"
                : $"Upgrade to Lv. {currentLevel + 1}";
        }

        if (costText != null)
        {
            costText.text = isMaxed
                ? "Max level reached"
                : BuildCostString(nextCost);
        }

        if (upgradeButton != null)
        {
            bool canAfford = !isMaxed && crafterGrid.CanAffordUpgrade(nextCost);
            upgradeButton.interactable = canAfford;
        }
    }

    private void OnUpgradePressed()
    {
        if (crafterGrid == null)
            return;

        int currentLevel = crafterGrid.GetGeneratorLevel(generatorType);
        int maxLevel = crafterGrid.GetGeneratorMaxLevel(generatorType);

        if (currentLevel >= maxLevel)
        {
            RefreshUI();
            return;
        }

        CrafterGeneratorUpgradeCostEntry nextCost = crafterGrid.GetNextGeneratorUpgradeCost(generatorType);
        if (nextCost == null)
        {
            RefreshUI();
            return;
        }

        bool paid = crafterGrid.TryPayUpgradeCost(nextCost);
        if (!paid)
        {
            RefreshUI();
            return;
        }

        crafterGrid.SetGeneratorLevel(generatorType, nextCost.targetLevel);
        RefreshUI();
    }

    private string BuildCostString(CrafterGeneratorUpgradeCostEntry costEntry)
    {
        if (costEntry == null)
            return "No upgrade cost";

        StringBuilder sb = new StringBuilder();

        if (costEntry.gridCosts != null)
        {
            for (int i = 0; i < costEntry.gridCosts.Length; i++)
            {
                GridRequirement req = costEntry.gridCosts[i];
                if (req == null)
                    continue;

                sb.AppendLine($"{req.quantity}x {req.entityType}");
            }
        }

        if (costEntry.inventoryCosts != null)
        {
            for (int i = 0; i < costEntry.inventoryCosts.Length; i++)
            {
                InventoryRequirement req = costEntry.inventoryCosts[i];
                if (req == null || req.item == null)
                    continue;

                sb.AppendLine($"{req.quantity}x {req.item.itemName}");
            }
        }

        string result = sb.ToString().TrimEnd();
        return string.IsNullOrEmpty(result) ? "Free" : result;
    }
}