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

    [Header("Start State")]
    [SerializeField] private bool startWithOreGenerator = false;
    [SerializeField] private int mergesPerEnemySpawn = 6;

    [Header("Debug")]
    [SerializeField] private bool autoDefeatEnemyForNow = true;
    [SerializeField] private CrafterEntityDatabase entityDatabase;

    private const string SaveKey = "CrafterBuilding_GridSave";

    private readonly List<CrafterGridSlotUI> slots = new();
    private readonly List<CrafterGridItemUI> spawnedVisuals = new();

    private CrafterEntityType[] gridState;
    private int mergeCounter;

    [Serializable]
    private class CrafterGridSaveData
    {
        public int columns;
        public int rows;
        public int mergeCounter;
        public int[] occupants;
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

    BuildSlotObjects();
    LoadOrCreate();
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
                gridState = new CrafterEntityType[data.occupants.Length];

                for (int i = 0; i < data.occupants.Length; i++)
                    gridState[i] = (CrafterEntityType)data.occupants[i];

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

        gridState[0] = CrafterEntityType.WoodGenerator;

        if (startWithOreGenerator && total > 1)
            gridState[1] = CrafterEntityType.OreGenerator;

        Save();
    }

    public void Save()
    {
        CrafterGridSaveData data = new CrafterGridSaveData
        {
            columns = columns,
            rows = rows,
            mergeCounter = mergeCounter,
            occupants = new int[gridState.Length]
        };

        for (int i = 0; i < gridState.Length; i++)
            data.occupants[i] = (int)gridState[i];

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
{
    Debug.Log($"Clicked item: {def.displayName}. Later open submit / destroy / sell menu here.");
}
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

        CrafterEntityType spawnType = generatorDef.generatedEntityType;
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

        // move into empty tile
        if (to == CrafterEntityType.None)
        {
            gridState[toIndex] = from;
            gridState[fromIndex] = CrafterEntityType.None;

            Save();
            RefreshVisuals();
            return;
        }

        // merge
        CrafterEntityType result = CrafterRules.GetMergeResult(from, to);
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

        // swap with another movable item
        if (toDef != null && toDef.isMovable && !toDef.isGenerator && !toDef.isEnemy)
        {
            gridState[toIndex] = from;
            gridState[fromIndex] = to;

            Save();
            RefreshVisuals();
        }
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

        Debug.Log("Enemy defeated. Later: give battle reward here.");

        Save();
        RefreshVisuals();
    }

    public void UnlockOreGeneratorForTesting()
    {
        if (ContainsEntity(CrafterEntityType.OreGenerator))
            return;

        int empty = GetFirstEmptyIndex();
        if (empty < 0)
            return;

        gridState[empty] = CrafterEntityType.OreGenerator;
        Save();
        RefreshVisuals();
    }

    public void ResetGrid()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        CreateFreshGrid();
        RefreshVisuals();
    }

    private bool ContainsEntity(CrafterEntityType type)
    {
        for (int i = 0; i < gridState.Length; i++)
        {
            if (gridState[i] == type)
                return true;
        }
        return false;
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

    // Only recycle movable crafted items, not generators/enemies.
    if (!def.isMovable || def.isGenerator || def.isEnemy)
        return;

    // Award gold based on the item's base value.
    if (PlayerData.Instance != null)
        PlayerData.Instance.AddCopper(def.sellValueCopper);

    Debug.Log($"Recycled: {def.displayName} for {def.sellValueCopper} gold");

    gridState[index] = CrafterEntityType.None;

    Save();
    RefreshVisuals();
}
}