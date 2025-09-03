using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasteryBarUI : MonoBehaviour
{
    [Header("Wiring")]
    public Slider masterySlider;
    public TextMeshProUGUI masteryText;

    CharacterData cd;

    // Call this once whenever the current hero/class changes (or on panel open)
    public void SetData(CharacterData characterData)
    {
        cd = characterData;
        Refresh();
    }

    public void Refresh()
    {
        if (cd == null || cd.classMastery == null) return;

        var m = cd.classMastery;
        int level = Mathf.Max(1, m.knightLevel);

        // If you used the same KnightLevelXP array as earlier:
        float current = m.knightXP;
        float prevReq = KnightXP(level - 1);
        float nextReq = KnightXP(level);

        float inLevel = Mathf.Clamp(current - prevReq, 0f, Mathf.Max(1f, nextReq - prevReq));
        masterySlider.minValue = 0f;
        masterySlider.maxValue = Mathf.Max(1f, nextReq - prevReq);
        masterySlider.value = inLevel;

        masteryText.text = $"Knight Mastery Lv {level}  ({Mathf.FloorToInt(inLevel)} / {Mathf.FloorToInt(nextReq - prevReq)})";
    }

    // Keep in sync with ClassMastery.KnightLevelXP values
    float KnightXP(int lvl)
    {
        // lvl is 0..10, use the same thresholds you defined in ClassMastery
        switch (Mathf.Clamp(lvl, 0, 10))
        {
            case 0:  return 0;
            case 1:  return 120;
            case 2:  return 240;
            case 3:  return 420;
            case 4:  return 650;
            case 5:  return 950;
            case 6:  return 1350;
            case 7:  return 1850;
            case 8:  return 2450;
            case 9:  return 3150;
            case 10: return 4000;
        }
        return 4000;
    }
}
