using UnityEngine;

public static class CrafterRules
{
    public static bool IsGenerator(CrafterEntityType type)
    {
        return type == CrafterEntityType.WoodGenerator ||
               type == CrafterEntityType.OreGenerator;
    }

    public static bool IsEnemy(CrafterEntityType type)
    {
        return type == CrafterEntityType.RogueEnemy;
    }

    public static bool IsMovable(CrafterEntityType type)
    {
        if (type == CrafterEntityType.None) return false;
        if (type == CrafterEntityType.Chest) return false;
        if (IsGenerator(type)) return false;
        if (IsEnemy(type)) return false;
        return true;
    }
}