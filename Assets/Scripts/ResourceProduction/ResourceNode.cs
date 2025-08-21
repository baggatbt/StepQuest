using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ResourceNode : MonoBehaviour
{
    [Header("UI Popups")]
    public DamagePopup       damagePopupPrefab;
    public FloatingItemPopup itemPopupPrefab;
    public Canvas            uiCanvas;

    [Header("Drop UI")]
    public Sprite            itemIcon;

    //────────── Node config
    public ResourceType type = ResourceType.Wood;
    public int          maxHP          = 20;
    public Item         payoutItem;
    public int          payoutMin      = 3;
    public int          payoutMax      = 6;

    [Min(0)] public int stepCost = 0;       // steps to start minigame

    private string skillID => type switch
    {
        ResourceType.Wood => "Woodcutting",
        ResourceType.Ore  => "Mining",
        _                 => "Unknown"
    };

    [SerializeField] private int xpPerPlay   = 5;   // XP for attempting (awarded on non-miss)
    [SerializeField] private int xpOnBreak   = 20;
    [SerializeField] private int requiredSkillLevel = 1;

    [Header("HP UI")]
    public Slider           hpSlider;
    public TextMeshProUGUI  hpText;

    [Header("Minigame & Prompt")]
    public HarvestPrompt     promptPanel;     // same as before
    public TimingBarController minigame;      // <— use the new script


    [Header("Minigame Damage Multipliers")]
    public float okMultiplier   = 1.0f;
    public float goodMultiplier = 1.5f;
    public float missMultiplier = 0.0f;

    //────────── Runtime
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
        if (hpSlider) { hpSlider.maxValue = maxHP; hpSlider.value = currentHP; }
        if (hpText)   hpText.text = $"{currentHP}/{maxHP}";

        GetComponent<Button>().onClick.AddListener(OnTapped);
    }

    void OnTapped()
    {
        // 1) Skill gate
        if (TaskSkillManager.Instance.GetLevel(skillID) < requiredSkillLevel)
        {
            Debug.Log($"Need {requiredSkillLevel}+ {skillID} to attempt this node.");
            return;
        }

        // 2) Open confirm prompt (we do NOT apply damage yet)
        if (promptPanel == null)
        {
            Debug.LogWarning("HarvestPrompt is not wired in the inspector.");
            return;
        }

        string body = (stepCost <= 0)
            ? "Play a timing minigame to harvest?"
            : $"Pay {stepCost:N0} steps to play a timing minigame and harvest?";

        promptPanel.Open(
            "Harvest",
            body,
            confirm: TryStartMinigame,
            cancel: null
        );
    }

    void TryStartMinigame()
{
    if (stepCost > 0 && !PlayerData.Instance.UseSteps(stepCost))
    {
        Debug.Log("Not enough steps to play minigame.");
        return;
    }

    if (!minigame)
    {
        Debug.LogWarning("TimingBarController is not wired in the inspector.");
        return;
    }

    // one session → 3 attempts
    minigame.attemptsPerSeries = 3;
    minigame.OnSeriesFinished  = OnSeriesFinished;
    minigame.OpenAndStartSeries(minigame.attemptsPerSeries);
}

void OnSeriesFinished(TimingResult[] results)
{
    // aggregate multiplier across the 3 tries (reward at end)
    float totalMult = 0f;
    int successes = 0;

    foreach (var r in results)
    {
        switch (r)
        {
            case TimingResult.Good: totalMult += goodMultiplier; successes++; break;
            case TimingResult.Okay: totalMult += okMultiplier;   successes++; break;
            case TimingResult.Miss: totalMult += missMultiplier; break;
        }
    }

    int baseDmg  = PlayerHarvestStats.Instance.GetTapDamage(type);
    int finalDmg = Mathf.Max(0, Mathf.RoundToInt(baseDmg * totalMult)); // sum of per-attempt multipliers

    // XP: award per successful attempt
    if (successes > 0)
    {
        int xpGain = xpPerPlay * successes;
        TaskSkillManager.Instance.AddXP(skillID, xpGain);
        Debug.Log($"+{xpGain} {skillID} XP (minigame x{successes})");
    }

    if (finalDmg > 0) ApplyDamage(finalDmg);
    else              ShowZeroPopup();
}

void OnMinigameFinished(TimingResult result)
{
    float mult = result switch
    {
        TimingResult.Good => goodMultiplier,
        TimingResult.Okay => okMultiplier,
        _                 => missMultiplier
    };

    int baseDmg = PlayerHarvestStats.Instance.GetTapDamage(type);
    int finalDmg = Mathf.Max(0, Mathf.RoundToInt(baseDmg * mult));

    if (result != TimingResult.Miss)
    {
        TaskSkillManager.Instance.AddXP(skillID, xpPerPlay);
        Debug.Log($"+{xpPerPlay} {skillID} XP (minigame)");
    }

    if (finalDmg > 0) ApplyDamage(finalDmg);
    else              ShowZeroPopup();
}

    void ShowZeroPopup()
    {
        if (!damagePopupPrefab || !uiCanvas) return;

        var popup = Instantiate(damagePopupPrefab, uiCanvas.transform, false);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        popup.GetComponent<RectTransform>().anchoredPosition = screenPos - new Vector3(Screen.width, Screen.height) * 0.5f;
        popup.Setup(0);
    }

    void ApplyDamage(int dmg)
    {
        currentHP = Mathf.Max(0, currentHP - dmg);

        if (hpSlider) hpSlider.value = currentHP;
        if (hpText)   hpText.text    = $"{currentHP}/{maxHP}";

        var popup = Instantiate(damagePopupPrefab, uiCanvas.transform, false);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        popup.GetComponent<RectTransform>().anchoredPosition = screenPos - new Vector3(Screen.width, Screen.height) * 0.5f;
        popup.Setup(dmg);

        if (currentHP == 0)
        {
            TaskSkillManager.Instance.AddXP(skillID, xpOnBreak);
            Debug.Log($"+{xpOnBreak} {skillID} XP (break)");
            GiveLoot();
            Respawn();
        }
    }

    void GiveLoot()
    {
        int qty = Random.Range(payoutMin, payoutMax + 1);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        screenPos -= new Vector3(Screen.width, Screen.height) * 0.5f;

        for (int i = 0; i < qty; i++)
        {
            GameManager.Instance.AddItem(payoutItem);

            var popup = Instantiate(itemPopupPrefab, uiCanvas.transform, false);
            popup.GetComponent<RectTransform>().anchoredPosition = screenPos;
            popup.Initialize(itemIcon);
        }
    }

    void Respawn()
    {
        currentHP = maxHP;
        if (hpSlider) hpSlider.value = currentHP;
        if (hpText)   hpText.text    = $"{currentHP}/{maxHP}";
    }
}
