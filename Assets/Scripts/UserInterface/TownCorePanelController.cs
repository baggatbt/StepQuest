using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TownCorePanelController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI stepCapText;
    public TextMeshProUGUI upgradeCostText; // shows copper cost for next level
    public Button upgradeButton;
    public TextMeshProUGUI feedbackText;     // optional: show “Not enough copper” message

    private void OnEnable()
    {
        RefreshUI();
    }

    /// <summary>
    /// Call this from the Upgrade button’s OnClick().
    /// </summary>
    public void OnUpgradeButtonPressed()
    {
        if (TownCore.Instance == null)
            return;

        if (TownCore.Instance.IsMaxLevel())
        {
            feedbackText.text = "TownCore is already at MAX level.";
            return;
        }

        int costCopper = TownCore.Instance.GetUpgradeCopperCost();
        if (PlayerData.Instance.totalCopper >= costCopper)
        {
            bool didUpgrade = TownCore.Instance.UpgradeTownCore();
            if (didUpgrade)
            {
                feedbackText.text = "Upgraded!";
                RefreshUI();
            }
            else
            {
                feedbackText.text = "Upgrade failed.";
            }
        }
        else
        {
            feedbackText.text = $"Need {costCopper} copper to upgrade.";
        }
    }

    /// <summary>
    /// Call this whenever the panel opens, or after a successful upgrade.
    /// </summary>
    private void RefreshUI()
    {
        if (TownCore.Instance == null)
            return;

        int lvl = TownCore.Instance.level;
        levelText.text = $"Level: {lvl}";

        int cap = TownCore.Instance.GetCurrentStepCap();
        stepCapText.text = $"Step Cap: {cap}";

        if (TownCore.Instance.IsMaxLevel())
        {
            upgradeCostText.text = "MAX LEVEL";
            upgradeButton.interactable = false;
        }
        else
        {
            int nextCost = TownCore.Instance.GetUpgradeCopperCost();
            upgradeCostText.text = $"Cost: {nextCost} copper";
            upgradeButton.interactable = true;
        }
    }
}
