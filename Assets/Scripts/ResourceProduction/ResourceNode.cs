using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ResourceNode : MonoBehaviour
{
    //───────────────────────────────── Config
    public ResourceType type           = ResourceType.Wood;
    public int          maxHP          = 20;
    public Item         payoutItem;
    public int          payoutMin      = 3;
    public int          payoutMax      = 6;
    [Min(0)] public int stepCost       = 0;     // 0 = free (for testing)

    private string skillID => type switch
    {
        ResourceType.Wood => "Woodcutting",
        ResourceType.Ore  => "Mining",
        _                 => "Unknown"
    };

    [SerializeField] private int xpPerTap  = 5;
    [SerializeField] private int xpOnBreak = 20;
    [SerializeField] private int requiredSkillLevel = 1;

    //───────────────────────────────── UI
    public Slider         hpSlider;
    public TextMeshProUGUI hpText;

    //───────────────────────────────── Runtime
    private int currentHP;

    // ───────────────────────────────────────────────────────────────
    private void Start()
    {
        currentHP = maxHP;

        if (hpSlider)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value    = currentHP;
        }

        GetComponent<Button>().onClick.AddListener(OnTapped);
    }

    private void OnTapped()
    {
        // 1) Skill-level gate
        if (TaskSkillManager.Instance.GetLevel(skillID) < requiredSkillLevel)
        {
            Debug.Log($"Need {requiredSkillLevel}+ {skillID} to tap this node.");
            return;
        }

        // 2) Pay step cost (if any)
        if (stepCost > 0 && !PlayerData.Instance.UseSteps(stepCost))
            return;

        // 3) Deal damage & grant per-tap XP
        int dmg = PlayerHarvestStats.Instance.GetTapDamage(type);
        TaskSkillManager.Instance.AddXP(skillID, xpPerTap);
        Debug.Log($"+{xpPerTap} {skillID} XP (tap)");

        ApplyDamage(dmg);
    }

    private void ApplyDamage(int dmg)
    {
        currentHP = Mathf.Max(0, currentHP - dmg);

        if (hpSlider) hpSlider.value = currentHP;
        if (hpText)   hpText.text   = $"{currentHP}/{maxHP}";

        // Break?
        if (currentHP == 0)
        {
            TaskSkillManager.Instance.AddXP(skillID, xpOnBreak);
            Debug.Log($"+{xpOnBreak} {skillID} XP (break)");
            GiveLoot();
            Respawn();
        }
    }

    private void GiveLoot()
    {
        int qty = Random.Range(payoutMin, payoutMax + 1);
        for (int i = 0; i < qty; i++)
            GameManager.Instance.AddItem(payoutItem);
    }

    private void Respawn()
    {
        currentHP = maxHP;
        if (hpSlider) hpSlider.value = currentHP;
        if (hpText)   hpText.text   = $"{currentHP}/{maxHP}";
    }
}
