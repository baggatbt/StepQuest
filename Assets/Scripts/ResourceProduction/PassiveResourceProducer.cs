using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Converts real-world steps → resources.
/// Listens to PlayerData.OnStepsAdded so it also works while the app is closed
/// (when you reconcile steps at startup).
/// </summary>
public class PassiveResourceProducer : MonoBehaviour
{
    [Header("Resource")]
    public ResourceType        type            = ResourceType.Wood;
    public Item                payoutItem;                   // Wood SO
    public int                 baseStepsPerItem = 100;       // “1 wood per 100 steps”
    [Tooltip("Fraction fewer steps per upgrade level. 0.1 = 10 % faster")]
    public float               reductionPerLvl = 0.10f;
    [Tooltip("Extra items created each time a payout happens")]
    public int                 amountBase      = 1;
    public int                 amountPerLvl    = 0;         // e.g. +1 wood per lvl

    [Header("UI (optional)")]
    public Slider              progressSlider;               // shows % to next payout
    public TextMeshProUGUI     progressLabel;

    //──────── internal state (saved)
    [SerializeField] private int   rateLevel       = 0;      // upgrades bought
    [SerializeField] private float stepProgress    = 0;      // unspent steps

    //──────── life-cycle
    private void OnEnable () => PlayerData.OnStepsAdded += OnSteps;
    private void OnDisable() => PlayerData.OnStepsAdded -= OnSteps;
    private void Start      () { Load(); RefreshUI(); }

    //──────── step tick
    private void OnSteps(int added)
    {
        Debug.Log(
    $"PRP ▶ added:{added}  base:{baseStepsPerItem}  reduction:{reductionPerLvl}  " +
    $"rateLevel:{rateLevel}  StepsPerItem:{StepsPerItem()}  payQty:{ItemsPerPayout()}");

        stepProgress += added;
        float needed = StepsPerItem();

        // produce as many times as we can (handles big offline bursts)
        while (stepProgress >= needed)
        {
            stepProgress -= needed;
            GiveLoot();
        }

        RefreshUI();
        Save();
    }

    //──────── helpers
    private float StepsPerItem() =>
        baseStepsPerItem * Mathf.Pow(1f - reductionPerLvl, rateLevel);

    private int ItemsPerPayout() => amountBase + rateLevel * amountPerLvl;

    private void GiveLoot()
    {
        int qty = ItemsPerPayout();
        for (int i = 0; i < qty; i++)
            GameManager.Instance.AddItem(payoutItem);
    }

    private void RefreshUI()
    {
        if (progressSlider)
        {
            progressSlider.maxValue = StepsPerItem();
            progressSlider.value    = stepProgress;
        }
        if (progressLabel)
            progressLabel.text =
                $"{(int)stepProgress}/{(int)StepsPerItem()} steps";
    }

    //──────── persistence (PlayerPrefs is fine here; swap for your save-obj if you prefer)
    private const string LvlKey  = "WoodRateLvl";
    private const string ProgKey = "WoodRateProg";

    private void Save()
    {
        PlayerPrefs.SetInt   (LvlKey , rateLevel);
        PlayerPrefs.SetFloat (ProgKey, stepProgress);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        rateLevel    = PlayerPrefs.GetInt  (LvlKey , 0);
        stepProgress = PlayerPrefs.GetFloat(ProgKey, 0f);
    }

    //──────── API for the upgrade button
    public int  GetRateLevel()                => rateLevel;
    public void IncreaseRateLevel(int delta)  { rateLevel += delta; Save(); }
}
