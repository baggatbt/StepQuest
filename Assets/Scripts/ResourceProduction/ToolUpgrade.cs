using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ToolUpgrade : MonoBehaviour
{
    [Header("Upgrade Target")]
    public ResourceType   type  = ResourceType.Wood;

    [Header("Economy")]
    public int            baseCost      = 50;   // first upgrade
    public float          costMultiplier = 1.75f;

    [Header("UI")]
    public TextMeshProUGUI costText;

    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(AttemptUpgrade);
        RefreshUI();
    }

    private void AttemptUpgrade()
    {
        int currentLvl = PlayerHarvestStats.Instance.GetToolLevel(type);
        long cost      = GetCost(currentLvl);

        if (PlayerData.Instance.totalCopper >= cost)
        {
            PlayerData.Instance.totalCopper -= cost;
            PlayerHarvestStats.Instance.IncreaseToolLevel(type);
            PlayerData.Instance.SavePlayerData();
            RefreshUI();
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    private void RefreshUI()
    {
        int   lvl  = PlayerHarvestStats.Instance.GetToolLevel(type);
        long  next = GetCost(lvl);
        if (costText) costText.text = $"Upgrade Axe (Lv {lvl})\nCost: {CurrencyManager.GetMultiCoinString(next)}";
    }

    private long GetCost(int currentLevel)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }
}
