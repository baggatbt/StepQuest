using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class RateUpgradeButton : MonoBehaviour
{
    public PassiveResourceProducer producer;         // drag the Tree panel here
    public long   baseCost       = 100;              // gold for level 0→1
    public float  costMultiplier = 2f;               // exponential growth
    public TextMeshProUGUI label;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(TryUpgrade);
        RefreshLabel();
    }

    private void TryUpgrade()
    {
        int  lvl  = producer.GetRateLevel();
        long cost = CalcCost(lvl);

        if (PlayerData.Instance.totalCopper >= cost)
        {
            PlayerData.Instance.totalCopper -= cost;
            producer.IncreaseRateLevel(1);
            PlayerData.Instance.SavePlayerData();
            RefreshLabel();
        }
        else
        {
            Debug.Log("Not enough gold.");
        }
    }

    private void RefreshLabel()
    {
        int  lvl  = producer.GetRateLevel();
        long cost = CalcCost(lvl);
        if (label)
            label.text = $"Upgrade Rate (Lv {lvl})\nCost: {CurrencyManager.GetMultiCoinString(cost)}";
    }

    private long CalcCost(int lvl) =>
        Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, lvl));
}
