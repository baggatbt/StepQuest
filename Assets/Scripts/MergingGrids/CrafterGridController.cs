using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrafterGridController : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 4;
    [SerializeField] private Transform gridRoot;
    [SerializeField] private CrafterGridSlotUI slotPrefab;
    [SerializeField] private CrafterGridItemUI itemPrefab;

    [Header("UI")]
    [SerializeField] private TMP_Text stepText;
    [SerializeField] private Canvas rootCanvas;

    [Header("Chest")]
    [SerializeField] private CrafterChestPanelUI chestPanelUI;
    [SerializeField] private int chestSlotCount = 4;

    [Header("Start State")]
    [SerializeField] private bool startWithOreGenerator = false;
    [SerializeField] private int mergesPerEnemySpawn = 6;

    [Header("Data")]
    [SerializeField] private CrafterEntityDatabase entityDatabase;
    [SerializeField] private CrafterMergeRecipeDatabase mergeRecipeDatabase;
    [SerializeField] private CrafterGeneratorUpgradeDatabase generatorUpgradeDatabase;
    [SerializeField] private CrafterGeneratorUpgradeCostDatabase generatorUpgradeCostDatabase;

    [Header("Debug")]
    [SerializeField] private bool autoDefeatEnemyForNow = true;

    private const string SaveKey = "CrafterBuilding_GridSave";

    private readonly List<CrafterGridSlotUI> slots = new();
    private readonly List<CrafterGridItemUI> spawnedVisuals = new();

    private CrafterEntityType[] gridState;
    private int mergeCounter;
    private List<CrafterChestSlotData> chestSlots = new();

    private int woodGeneratorLevel = 1;
    private int oreGeneratorLevel = 1;

    [Serializable]
    private class CrafterGridSaveData
    {
        public int columns;
        public int rows;
        public int mergeCounter;
        public int woodGeneratorLevel = 1;
        public int oreGeneratorLevel = 1;
        public int[] occupants;
        public int[] chestOccupants;
    }

    private void OnEnable()
    {
        PlayerData.OnStepsAdded += HandleStepsChanged;
    }

    private void OnDisable()
    {
        PlayerData.OnStepsAdded -= HandleStepsChanged;
    }

    private void Start()
    {
        if (entityDatabase != null)
            entityDatabase.BuildLookup();

        if (generatorUpgradeDatabase != null)
            generatorUpgradeDatabase.BuildLookup();

        if (generatorUpgradeCostDatabase != null)
            generatorUpgradeCostDatabase.BuildLookup();

        EnsureChestSlots();
        BuildSlotObjects();
        LoadOrCreate();

        if (chestPanelUI != null)
            chestPanelUI.Initialize(this);

        RefreshVisuals();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void HandleStepsChanged(int amount)
    {
        RefreshStepText();
    }

    private void EnsureChestSlots()
    {
        if (chestSlots == null)
            chestSlots = new List<CrafterChestSlotData>();

        while (chestSlots.Count < chestSlotCount)
            chestSlots.Add(new CrafterChestSlotData());

        if (chestSlots.Count > chestSlotCount)
            chestSlots.RemoveRange(chestSlotCount, chestSlots.Count - chestSlotCount);
    }

    private void BuildSlotObjects()
    {
        foreach (Transform child in gridRoot)
            Destroy(child.gameObject);

        slots.Clear();

        int total = columns * rows;
        for (int i = 0; i < total; i++)
        {
            CrafterGridSlotUI slot = Instantiate(slotPrefab, gridRoot);
            slot.Setup(this, i);
            slots.Add(slot);
        }
    }

    private void LoadOrCreate()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            CrafterGridSaveData data = JsonUtility.FromJson<CrafterGridSaveData>(json);

            if (data != null && data.occupants != null && data.occupants.Length == columns * rows)
            {
                mergeCounter = data.mergeCounter;
                woodGeneratorLevel = Mathf.Max(1, data.woodGeneratorLevel);
                oreGeneratorLevel = Mathf.Max(1, data.oreGeneratorLevel);

                gridState = new CrafterEntityType[data.occupants.Length];

                for (int i = 0; i < data.occupants.Length; i++)
                    gridState[i] = (CrafterEntityType)data.occupants[i];

                EnsureChestSlots();

                if (data.chestOccupants != null)
                {
                    for (int i = 0; i < Mathf.Min(data.chestOccupants.Length, chestSlots.Count); i++)
                        chestSlots[i].storedType = (CrafterEntityType)data.chestOccupants[i];
                }

                return;
            }
        }

        CreateFreshGrid();
    }

    private void CreateFreshGrid()
    {
        int total = columns * rows;
        gridState = new CrafterEntityType[total];
        mergeCounter = 0;
        woodGeneratorLevel = 1;
        oreGeneratorLevel = 1;

        gridState[0] = CrafterEntityType.WoodGenerator;

        if (total > 1)
            gridState[1] = CrafterEntityType.Chest;

        if (startWithOreGenerator && total > 2)
            gridState[2] = CrafterEntityType.OreGenerator;

        Debug.Log("Fresh grid created with chest at index 1");
        EnsureChestSlots();
        Save();
    }

    public bool TryMoveChestItemToFirstEmptyGrid(int chestSlotIndex)
    {
        if (chestSlotIndex < 0 || chestSlotIndex >= chestSlots.Count)
            return false;

        int emptyIndex = GetFirstEmptyIndex();
        if (emptyIndex < 0)
            return false;

        CrafterEntityType storedType = chestSlots[chestSlotIndex].storedType;
        if (storedType == CrafterEntityType.None)
            return false;

        chestSlots[chestSlotIndex].Clear();
        gridState[emptyIndex] = storedType;

        Save();
        RefreshVisuals();
        return true;
    }

    public void Save()
    {
        CrafterGridSaveData data = new CrafterGridSaveData
        {
            columns = columns,
            rows = rows,
            mergeCounter = mergeCounter,
            woodGeneratorLevel = woodGeneratorLevel,
            oreGeneratorLevel = oreGeneratorLevel,
            occupants = new int[gridState.Length],
            chestOccupants = new int[chestSlots.Count]
        };

        for (int i = 0; i < gridState.Length; i++)
            data.occupants[i] = (int)gridState[i];

        for (int i = 0; i < chestSlots.Count; i++)
            data.chestOccupants[i] = (int)chestSlots[i].storedType;

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public void RefreshVisuals()
    {
        for (int i = 0; i < spawnedVisuals.Count; i++)
        {
            if (spawnedVisuals[i] != null)
                Destroy(spawnedVisuals[i].gameObject);
        }
        spawnedVisuals.Clear();

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridRoot as RectTransform);
        Canvas.ForceUpdateCanvases();

        for (int i = 0; i < gridState.Length; i++)
        {
            CrafterEntityType type = gridState[i];
            if (type == CrafterEntityType.None)
                continue;

            CrafterEntityDefinition def = entityDatabase.Get(type);
            if (def == null)
            {
                Debug.LogWarning($"Missing CrafterEntityDefinition for {type}");
                continue;
            }

            CrafterGridItemUI item = Instantiate(itemPrefab, slots[i].transform);

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;

            item.Setup(this, i, def, rootCanvas);
            spawnedVisuals.Add(item);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridRoot as RectTransform);
        Canvas.ForceUpdateCanvases();

        if (chestPanelUI != null)
            chestPanelUI.RefreshUI();

        RefreshStepText();
    }

    private void RefreshStepText()
    {
        if (stepText == null)
            return;

        int currentSteps = 0;
        if (PlayerData.Instance != null)
            currentSteps = PlayerData.Instance.inGameSteps;

        stepText.text = $"Steps: {currentSteps}";
    }

    public void OnEntityClicked(int index)
    {
        if (!IsValidIndex(index))
            return;

        CrafterEntityType type = gridState[index];
        CrafterEntityDefinition def = entityDatabase.Get(type);

        if (def == null)
            return;

        if (type == CrafterEntityType.Chest)
        {
            chestPanelUI?.Toggle();
            return;
        }

        if (def.isGenerator)
        {
            TryGenerateFrom(index);
            return;
        }

        if (def.isEnemy)
        {
            Debug.Log("Open battle confirmation panel here.");

            if (autoDefeatEnemyForNow)
                DefeatEnemyAt(index);

            return;
        }

        if (def.isMovable)
            Debug.Log($"Clicked item: {def.displayName}");
    }

    private void TryGenerateFrom(int generatorIndex)
    {
        if (PlayerData.Instance == null)
        {
            Debug.LogWarning("PlayerData.Instance missing.");
            return;
        }

        CrafterEntityType generatorType = gridState[generatorIndex];
        CrafterEntityDefinition generatorDef = entityDatabase.Get(generatorType);

        if (generatorDef == null || !generatorDef.isGenerator)
        {
            Debug.LogWarning($"No generator definition found for {generatorType}");
            return;
        }

        int cost = generatorDef.stepCost;

        int emptyIndex = GetFirstEmptyIndex();
        if (emptyIndex < 0)
        {
            Debug.Log("No empty tile available.");
            return;
        }

        if (!PlayerData.Instance.UseSteps(cost))
        {
            Debug.Log("Not enough steps.");
            RefreshStepText();
            return;
        }

        int generatorLevel = GetGeneratorLevel(generatorType);

        CrafterEntityType spawnType = generatorUpgradeDatabase != null
            ? generatorUpgradeDatabase.GetOutputForLevel(
                generatorType,
                generatorLevel,
                generatorDef.generatedEntityType)
            : generatorDef.generatedEntityType;

        if (spawnType == CrafterEntityType.None)
        {
            RefreshStepText();
            return;
        }

        gridState[emptyIndex] = spawnType;

        Save();
        RefreshVisuals();
    }

    public void TryMoveOrMerge(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
            return;

        if (fromIndex == toIndex)
            return;

        CrafterEntityType from = gridState[fromIndex];
        CrafterEntityType to = gridState[toIndex];

        CrafterEntityDefinition fromDef = entityDatabase.Get(from);
        CrafterEntityDefinition toDef = entityDatabase.Get(to);

        if (fromDef == null || !fromDef.isMovable || fromDef.isGenerator || fromDef.isEnemy)
            return;

        if (to == CrafterEntityType.None)
        {
            gridState[toIndex] = from;
            gridState[fromIndex] = CrafterEntityType.None;

            Save();
            RefreshVisuals();
            return;
        }

        CrafterEntityType result = GetMergeResult(from, to);
        if (result != CrafterEntityType.None)
        {
            gridState[toIndex] = result;
            gridState[fromIndex] = CrafterEntityType.None;

            mergeCounter++;
            TrySpawnEnemy();

            Save();
            RefreshVisuals();
            return;
        }

        if (toDef != null && toDef.isMovable && !toDef.isGenerator && !toDef.isEnemy)
        {
            gridState[toIndex] = from;
            gridState[fromIndex] = to;

            Save();
            RefreshVisuals();
        }
    }

    private CrafterEntityType GetMergeResult(CrafterEntityType a, CrafterEntityType b)
    {
        if (mergeRecipeDatabase == null)
        {
            Debug.LogWarning("CrafterMergeRecipeDatabase is missing on CrafterGridController.");
            return CrafterEntityType.None;
        }

        return mergeRecipeDatabase.GetMergeResult(a, b);
    }

    private void TrySpawnEnemy()
    {
        if (mergeCounter < mergesPerEnemySpawn)
            return;

        mergeCounter = 0;

        if (HasEnemyOnBoard())
            return;

        int emptyIndex = GetFirstEmptyIndex();
        if (emptyIndex < 0)
            return;

        gridState[emptyIndex] = CrafterEntityType.RogueEnemy;
    }

    private bool HasEnemyOnBoard()
    {
        for (int i = 0; i < gridState.Length; i++)
        {
            if (gridState[i] == CrafterEntityType.RogueEnemy)
                return true;
        }
        return false;
    }

    public void DefeatEnemyAt(int index)
    {
        if (!IsValidIndex(index))
            return;

        if (gridState[index] != CrafterEntityType.RogueEnemy)
            return;

        gridState[index] = CrafterEntityType.None;
        Save();
        RefreshVisuals();
    }

    public void RecycleItemAt(int index)
    {
        if (!IsValidIndex(index))
            return;

        CrafterEntityType type = gridState[index];
        if (type == CrafterEntityType.None)
            return;

        CrafterEntityDefinition def = entityDatabase.Get(type);
        if (def == null)
            return;

        if (!def.isMovable || def.isGenerator || def.isEnemy)
            return;

        if (PlayerData.Instance != null)
            PlayerData.Instance.AddCopper(def.sellValueCopper);

        gridState[index] = CrafterEntityType.None;
        Save();
        RefreshVisuals();
    }

    public bool TryStoreGridItemInChest(int gridIndex, int chestSlotIndex)
    {
        if (!IsValidIndex(gridIndex))
            return false;

        if (chestSlotIndex < 0 || chestSlotIndex >= chestSlots.Count)
            return false;

        CrafterEntityType type = gridState[gridIndex];
        if (type == CrafterEntityType.None)
            return false;

        CrafterEntityDefinition def = entityDatabase.Get(type);
        if (def == null || !def.isMovable || def.isGenerator || def.isEnemy)
            return false;

        if (!chestSlots[chestSlotIndex].IsEmpty)
            return false;

        chestSlots[chestSlotIndex].Set(type);
        gridState[gridIndex] = CrafterEntityType.None;

        Save();
        RefreshVisuals();
        return true;
    }

    public int GetChestSlotCount() => chestSlots.Count;

    public CrafterEntityType GetChestStoredType(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= chestSlots.Count)
            return CrafterEntityType.None;

        return chestSlots[slotIndex].storedType;
    }

    public bool IsChestAt(int gridIndex)
    {
        if (!IsValidIndex(gridIndex))
            return false;

        return gridState[gridIndex] == CrafterEntityType.Chest;
    }

    public bool TryStoreGridItemInChestFirstOpen(int gridIndex)
    {
        if (!IsValidIndex(gridIndex))
            return false;

        CrafterEntityType type = gridState[gridIndex];
        if (type == CrafterEntityType.None)
            return false;

        CrafterEntityDefinition def = entityDatabase.Get(type);
        if (def == null || !def.isMovable || def.isGenerator || def.isEnemy)
            return false;

        for (int i = 0; i < chestSlots.Count; i++)
        {
            if (chestSlots[i].IsEmpty)
            {
                chestSlots[i].Set(type);
                gridState[gridIndex] = CrafterEntityType.None;

                Save();
                RefreshVisuals();
                return true;
            }
        }

        return false;
    }

    public CrafterEntityDefinition GetDefinition(CrafterEntityType type)
    {
        return entityDatabase != null ? entityDatabase.Get(type) : null;
    }

    public int CountEntityOnBoardAndChest(CrafterEntityType type)
    {
        int count = 0;

        for (int i = 0; i < gridState.Length; i++)
        {
            if (gridState[i] == type)
                count++;
        }

        for (int i = 0; i < chestSlots.Count; i++)
        {
            if (chestSlots[i].storedType == type)
                count++;
        }

        return count;
    }

    public bool TryConsumeEntityFromBoardAndChest(CrafterEntityType type, int amount)
    {
        if (amount <= 0)
            return true;

        int total = CountEntityOnBoardAndChest(type);
        if (total < amount)
            return false;

        int remaining = amount;

        for (int i = 0; i < gridState.Length && remaining > 0; i++)
        {
            if (gridState[i] == type)
            {
                gridState[i] = CrafterEntityType.None;
                remaining--;
            }
        }

        for (int i = 0; i < chestSlots.Count && remaining > 0; i++)
        {
            if (chestSlots[i].storedType == type)
            {
                chestSlots[i].Clear();
                remaining--;
            }
        }

        Save();
        return true;
    }

    private int GetFirstEmptyIndex()
    {
        for (int i = 0; i < gridState.Length; i++)
        {
            if (gridState[i] == CrafterEntityType.None)
                return i;
        }
        return -1;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < gridState.Length;
    }

    public bool HasGridItems(CrafterEntityType type, int amount)
    {
        if (amount <= 0)
            return true;

        int count = 0;

        for (int i = 0; i < gridState.Length; i++)
        {
            if (gridState[i] == type)
                count++;
        }

        return count >= amount;
    }

    public bool RemoveGridItems(CrafterEntityType type, int amount)
    {
        if (amount <= 0)
            return true;

        if (!HasGridItems(type, amount))
            return false;

        int remaining = amount;

        for (int i = 0; i < gridState.Length; i++)
        {
            if (gridState[i] != type)
                continue;

            gridState[i] = CrafterEntityType.None;
            remaining--;

            if (remaining <= 0)
                break;
        }

        Save();
        RefreshVisuals();
        return true;
    }

    public bool CanAffordUpgrade(BuildingUpgradeCost cost)
    {
        if (cost == null || GameManager.Instance == null)
        {
            Debug.LogWarning("CanAffordUpgrade failed: cost or GameManager missing.");
            return false;
        }

        foreach (var invReq in cost.inventoryCosts)
        {
            if (invReq == null || invReq.item == null)
                continue;

            int have = GameManager.Instance.GetItemCount(invReq.item.itemID);
            if (have < invReq.quantity)
                return false;
        }

        foreach (var gridReq in cost.gridCosts)
        {
            if (gridReq == null)
                continue;

            int have = CountEntityOnBoardAndChest(gridReq.entityType);
            if (have < gridReq.quantity)
                return false;
        }

        return true;
    }

    public bool TryPayUpgradeCost(BuildingUpgradeCost cost)
    {
        if (!CanAffordUpgrade(cost))
            return false;

        foreach (var invReq in cost.inventoryCosts)
        {
            if (invReq == null || invReq.item == null)
                continue;

            bool removed = GameManager.Instance.RemoveItem(invReq.item.itemID, invReq.quantity);
            if (!removed)
            {
                Debug.LogWarning($"Failed removing inventory item {invReq.item.itemName}");
                return false;
            }
        }

        foreach (var gridReq in cost.gridCosts)
        {
            if (gridReq == null)
                continue;

            bool removed = TryConsumeEntityFromBoardAndChest(gridReq.entityType, gridReq.quantity);
            if (!removed)
            {
                Debug.LogWarning($"Failed removing grid/chest item {gridReq.entityType}");
                return false;
            }
        }

        Save();
        RefreshVisuals();
        return true;
    }

    public bool CanAffordUpgrade(CrafterGeneratorUpgradeCostEntry costEntry)
    {
        if (costEntry == null || GameManager.Instance == null)
            return false;

        if (costEntry.inventoryCosts != null)
        {
            for (int i = 0; i < costEntry.inventoryCosts.Length; i++)
            {
                InventoryRequirement invReq = costEntry.inventoryCosts[i];
                if (invReq == null || invReq.item == null)
                    continue;

                int have = GameManager.Instance.GetItemCount(invReq.item.itemID);
                if (have < invReq.quantity)
                    return false;
            }
        }

        if (costEntry.gridCosts != null)
        {
            for (int i = 0; i < costEntry.gridCosts.Length; i++)
            {
                GridRequirement gridReq = costEntry.gridCosts[i];
                if (gridReq == null)
                    continue;

                int have = CountEntityOnBoardAndChest(gridReq.entityType);
                if (have < gridReq.quantity)
                    return false;
            }
        }

        return true;
    }

    public bool TryPayUpgradeCost(CrafterGeneratorUpgradeCostEntry costEntry)
    {
        if (!CanAffordUpgrade(costEntry))
            return false;

        if (costEntry.inventoryCosts != null)
        {
            for (int i = 0; i < costEntry.inventoryCosts.Length; i++)
            {
                InventoryRequirement invReq = costEntry.inventoryCosts[i];
                if (invReq == null || invReq.item == null)
                    continue;

                bool removed = GameManager.Instance.RemoveItem(invReq.item.itemID, invReq.quantity);
                if (!removed)
                {
                    Debug.LogWarning($"Failed removing inventory item {invReq.item.itemName}");
                    return false;
                }
            }
        }

        if (costEntry.gridCosts != null)
        {
            for (int i = 0; i < costEntry.gridCosts.Length; i++)
            {
                GridRequirement gridReq = costEntry.gridCosts[i];
                if (gridReq == null)
                    continue;

                bool removed = TryConsumeEntityFromBoardAndChest(gridReq.entityType, gridReq.quantity);
                if (!removed)
                {
                    Debug.LogWarning($"Failed removing grid/chest item {gridReq.entityType}");
                    return false;
                }
            }
        }

        Save();
        RefreshVisuals();
        return true;
    }

    public int GetGeneratorLevel(CrafterEntityType generatorType)
    {
        switch (generatorType)
        {
            case CrafterEntityType.WoodGenerator:
                return woodGeneratorLevel;
            case CrafterEntityType.OreGenerator:
                return oreGeneratorLevel;
            default:
                return 1;
        }
    }

    public void SetGeneratorLevel(CrafterEntityType generatorType, int newLevel)
    {
        newLevel = Mathf.Max(1, newLevel);
        int maxLevel = GetGeneratorMaxLevel(generatorType);
        newLevel = Mathf.Min(newLevel, maxLevel);

        switch (generatorType)
        {
            case CrafterEntityType.WoodGenerator:
                woodGeneratorLevel = newLevel;
                break;
            case CrafterEntityType.OreGenerator:
                oreGeneratorLevel = newLevel;
                break;
            default:
                return;
        }

        Save();
        RefreshVisuals();
    }

    public int GetGeneratorMaxLevel(CrafterEntityType generatorType)
    {
        if (generatorUpgradeDatabase == null)
            return 1;

        return Mathf.Max(1, generatorUpgradeDatabase.GetMaxLevel(generatorType));
    }

    public CrafterGeneratorUpgradeCostEntry GetNextGeneratorUpgradeCost(CrafterEntityType generatorType)
    {
        if (generatorUpgradeCostDatabase == null)
            return null;

        int currentLevel = GetGeneratorLevel(generatorType);
        return generatorUpgradeCostDatabase.GetNextCost(generatorType, currentLevel);
    }
}