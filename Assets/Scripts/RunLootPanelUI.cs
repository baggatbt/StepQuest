using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunLootPanelUI : MonoBehaviour
{
    [Header("Panel")]
    public CanvasGroup cg;

    [Header("Header")]
    public TMP_Text titleText;

    [Header("List")]
    public Transform contentRoot;          // Content (VerticalLayoutGroup)
    public RunLootRowUI rowPrefab;         // prefab

    [Header("Buttons")]
    public Button continueButton;
    public Button leaveButton;

    // Keep references so we can destroy/rebuild cleanly
    readonly List<GameObject> _spawned = new();

    void Awake()
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();
        HideImmediate();
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnRunLootChanged += Rebuild;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnRunLootChanged -= Rebuild;
    }

    public void Show()
    {
        Rebuild();

        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    void HideImmediate()
    {
        gameObject.SetActive(true); // keep active so references exist
        Hide();
    }

    void Rebuild()
    {
        // clear old
        for (int i = 0; i < _spawned.Count; i++)
            if (_spawned[i] != null) Destroy(_spawned[i]);
        _spawned.Clear();

        var gm = GameManager.Instance;
        if (gm == null || gm.currentRunLoot == null) return;

        // header summary (optional)
        int distinct = gm.currentRunLoot.items.Count;
        int totalCount = 0;
        foreach (var s in gm.currentRunLoot.items) totalCount += s.quantity;

        if (titleText)
            titleText.text = $"Unsecured Loot: {totalCount} items ({distinct} types)";

        // rows
        foreach (var stack in gm.currentRunLoot.items)
        {
            if (stack.item == null || stack.quantity <= 0) continue;

            var row = Instantiate(rowPrefab, contentRoot);
            row.Bind(stack.item, stack.quantity);
            _spawned.Add(row.gameObject);
        }

        // buttons
        if (continueButton)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                Hide(); // just close the panel and keep going
            });
        }

        if (leaveButton)
        {
            leaveButton.onClick.RemoveAllListeners();
            leaveButton.onClick.AddListener(() =>
            {
                // Bank loot & end run
                GameManager.Instance.EndDungeonRun(true);
                // If you also want to return to town here:
                GameManager.Instance.ReturnToTownFromBattle();
            });
        }
    }
}
