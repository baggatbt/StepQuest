using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchNodeUI : MonoBehaviour
{
    public ResearchNode node;
    public TextMeshProUGUI nameText;
    public Button researchButton;
    public Image progressFill;

    private void Start()
    {
        nameText.text = node.displayName;
        researchButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (ResearchTreeManager.Instance.CanResearch(node))
        {
            ResearchTreeManager.Instance.StartResearch(node);
        }
    }

    void Update()
    {
        if (ResearchTreeManager.Instance.currentResearch == node)
        {
            progressFill.fillAmount = (float)ResearchTreeManager.Instance.currentProgress / node.stepCost;
        }

        if (ResearchTreeManager.Instance.unlockedNodes.Contains(node))
        {
            researchButton.interactable = false;
            nameText.color = Color.green;
        }
    }
}
