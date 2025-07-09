using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Displays live level + XP progress for a set of gathering / life‑skills.
/// Attach this to a UI panel and assign the widgets for each skill
/// in the Inspector. Works out‑of‑the‑box for Woodcutting and Mining
/// but you can add more rows at any time.
/// </summary>
public class SkillsPanelUI : MonoBehaviour
{
    [System.Serializable]
    private class SkillWidgets
    {
        [Tooltip("Exact skillID string used in TaskSkillManager")] public string   skillID = "Woodcutting";
        [Tooltip("Label that shows the skill name")]               public TMP_Text nameLabel;
        [Tooltip("Label that shows the level e.g. 'Lv 3'")]        public TMP_Text levelLabel;
        [Tooltip("Label that shows XP, e.g. '45 / 60 XP'")]        public TMP_Text xpLabel;
        [Tooltip("Slider with Min 0, Max 1 (not whole numbers)")]  public Slider   xpBar;
    }

    [Header("Assign the two rows from the hierarchy")]
    [SerializeField] private SkillWidgets[] skills;

    [Tooltip("How often (seconds) the panel refreshes")] 
    [SerializeField] private float refreshInterval = 0.25f;

    private TaskSkillManager mgr;

    private void Awake()
    {
        mgr = TaskSkillManager.Instance; // safe: panel lives in scenes after bootstrap
    }

    private void OnEnable() => StartCoroutine(RefreshLoop());
    private void OnDisable() => StopAllCoroutines();

    private IEnumerator RefreshLoop()
    {
        while (true)
        {
            if (mgr)
            {
                foreach (var w in skills)
                {
                    int lvl   = mgr.GetLevel   (w.skillID);
                    int curXP = mgr.GetCurrentXP(w.skillID);
                    int need  = mgr.GetXPToNextLevel(w.skillID);

                    if (w.nameLabel)  w.nameLabel.text  = w.skillID;
                    if (w.levelLabel) w.levelLabel.text = $"Lv {lvl}";
                    if (w.xpLabel)    w.xpLabel.text    = $"{curXP} / {need} XP";
                    if (w.xpBar)      w.xpBar.value     = need == 0 ? 1f : (float)curXP / need;
                }
            }
            yield return new WaitForSeconds(refreshInterval);
        }
    }
}
