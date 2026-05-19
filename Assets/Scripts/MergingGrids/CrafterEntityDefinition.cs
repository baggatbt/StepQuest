using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Entity Definition")]
public class CrafterEntityDefinition : ScriptableObject
{
    [Header("Identity")]
    public CrafterEntityType entityType;
    public string displayName;
    public int inventoryItemID = -1;

    [Header("Inventory Export")]
    public Item inventoryItem;
    public int inventoryAmount = 1;
    public bool canExportToInventory = true;

    [Header("Visuals")]
    public Sprite iconSprite;

    [Header("Classification")]
    public bool isGenerator;
    public bool isEnemy;
    public bool isMovable = true;

    [Header("Generator Data")]
    public int stepCost;
    public CrafterEntityType generatedEntityType;

    [Header("Economy")]
    public int sellValueCopper;
    public int destroyXp;
}