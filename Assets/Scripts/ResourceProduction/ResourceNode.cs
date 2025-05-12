using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ResourceNode : MonoBehaviour
{
    [Header("Config")]
    public ResourceType   type          = ResourceType.Wood;
    public int            maxHP         = 20;
    public Item           payoutItem;              // wood, ore, etc.
    public int            payoutMin     = 3;       // items on break
    public int            payoutMax     = 6;

    [Header("UI")]
    public Slider         hpSlider;
    public TextMeshProUGUI hpText;                 // optional

    // ───────── runtime
    private int currentHP;

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
        int dmg = PlayerHarvestStats.Instance.GetTapDamage(type);
        ApplyDamage(dmg);
    }

    private void ApplyDamage(int dmg)
    {
        currentHP = Mathf.Max(0, currentHP - dmg);
        if (hpSlider) hpSlider.value = currentHP;
        if (hpText)   hpText.text   = $"{currentHP}/{maxHP}";

        if (currentHP == 0)
        {
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
