using UnityEngine;

public class CrafterChestPanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform slotRoot;
    [SerializeField] private CrafterChestSlotUI slotPrefab;
    [SerializeField] private CrafterChestItemUI itemPrefab;
    [SerializeField] private GameObject panelRoot;

    private CrafterGridController gridController;
    private CrafterChestSlotUI[] spawnedSlots;
    private CrafterChestItemUI[] spawnedItems;

    public void Initialize(CrafterGridController controller)
    {
        gridController = controller;
        BuildSlots();
        Hide();
    }

    public void Show()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);
        else
            gameObject.SetActive(true);

        RefreshUI();
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void Toggle()
    {
        bool isActive = panelRoot != null ? panelRoot.activeSelf : gameObject.activeSelf;

        if (isActive) Hide();
        else Show();
    }

    public void RefreshUI()
    {
        if (gridController == null || spawnedSlots == null)
            return;

        if (spawnedItems != null)
        {
            for (int i = 0; i < spawnedItems.Length; i++)
            {
                if (spawnedItems[i] != null)
                    Destroy(spawnedItems[i].gameObject);
            }
        }

        int count = gridController.GetChestSlotCount();
        spawnedItems = new CrafterChestItemUI[count];

        for (int i = 0; i < count; i++)
        {
            CrafterEntityType type = gridController.GetChestStoredType(i);
            if (type == CrafterEntityType.None)
                continue;

            CrafterEntityDefinition def = gridController.GetDefinition(type);
            if (def == null)
                continue;

            CrafterChestItemUI item = Instantiate(itemPrefab, spawnedSlots[i].transform);

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;

            item.Setup(this, i, def);
            spawnedItems[i] = item;
        }
    }

    public void TryReturnChestItemToGrid(int chestSlotIndex)
    {
        if (gridController == null)
            return;

        bool success = gridController.TryMoveChestItemToFirstEmptyGrid(chestSlotIndex);
        if (success)
            RefreshUI();
    }

    private void BuildSlots()
    {
        int count = gridController.GetChestSlotCount();

        foreach (Transform child in slotRoot)
            Destroy(child.gameObject);

        spawnedSlots = new CrafterChestSlotUI[count];

        for (int i = 0; i < count; i++)
        {
            CrafterChestSlotUI slot = Instantiate(slotPrefab, slotRoot);
            slot.Setup(this, i);
            spawnedSlots[i] = slot;
        }
    }
}