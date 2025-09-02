using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveRowUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Slider bar;
    public TextMeshProUGUI progressText;

    Objective data;

    public void Bind(Objective o)
    {
        data = o;

        // Wire child refs if not assigned in prefab
        if (!titleText)    titleText    = transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        if (!bar)          bar          = transform.Find("Bar")?.GetComponent<Slider>();
        if (!progressText) progressText = transform.Find("ProgressText")?.GetComponent<TextMeshProUGUI>();

        if (titleText)    titleText.text = BuildTitle(o);
        if (bar)
        {
            bar.minValue = 0;
            bar.maxValue = Mathf.Max(1, o.target);
            bar.value    = Mathf.Clamp(o.progress, 0, o.target);
        }
        if (progressText) progressText.text = $"{o.progress} / {o.target}";
    }

    string BuildTitle(Objective o)
    {
        switch (o.type)
        {
            case ObjectiveType.KillAny:
                return $"Defeat {o.target} enemies";
            case ObjectiveType.SpendSteps:
                return $"Spend {o.target} steps";
            case ObjectiveType.ClearStage:
                // Optional: resolve stage name nicely
                var stage = GameManager.Instance.allStagesData.Find(s => s.stageID == o.arg);
                var name  = stage != null ? stage.stageID : o.arg;
                return $"Clear: {name}";
            default:
                return o.id;
        }
    }
}
