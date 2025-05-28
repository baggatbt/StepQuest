using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [Header("Which skill to display")]
    public string          skillID;        // e.g. "Woodcutting"

    [Header("UI References")]
    public Slider          xpSlider;       // set Min=0, Max=1
    public TextMeshProUGUI xpText;         // e.g. "23 / 150 XP"
    public TextMeshProUGUI levelText;      // e.g. "Woodcutting Lv. 3"

    void Update()
    {
        var mgr     = TaskSkillManager.Instance;
        int current = mgr.GetCurrentXP(skillID);
        int needed  = mgr.GetXPToNextLevel(skillID);
        int level   = mgr.GetLevel(skillID);

        xpSlider.value = mgr.GetProgress01(skillID);
        xpText.text    = $"{current} / {needed} XP";
        levelText.text = $"{skillID} Lv. {level}";
    }
}
