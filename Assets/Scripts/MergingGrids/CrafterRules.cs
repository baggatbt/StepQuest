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
        if (IsGenerator(type)) return false;
        if (IsEnemy(type)) return false;
        return true;
    }

    public static int GetStepCost(CrafterEntityType generatorType)
    {
        switch (generatorType)
        {
            case CrafterEntityType.WoodGenerator: return 50;
            case CrafterEntityType.OreGenerator: return 100;
            default: return 0;
        }
    }

    public static CrafterEntityType GetGeneratedItem(CrafterEntityType generatorType)
    {
        switch (generatorType)
        {
            case CrafterEntityType.WoodGenerator: return CrafterEntityType.Wood;
            case CrafterEntityType.OreGenerator: return CrafterEntityType.Ore;
            default: return CrafterEntityType.None;
        }
    }

     public static CrafterEntityType GetMergeResult(CrafterEntityType a, CrafterEntityType b)
    {
        if (a == CrafterEntityType.Wood && b == CrafterEntityType.Wood)
            return CrafterEntityType.Plank;

        if (a == CrafterEntityType.Plank && b == CrafterEntityType.Plank)
            return CrafterEntityType.Handle;

        if (a == CrafterEntityType.Ore && b == CrafterEntityType.Ore)
            return CrafterEntityType.Ingot;

        if (a == CrafterEntityType.Ingot && b == CrafterEntityType.Ingot)
            return CrafterEntityType.Blade;

        bool swordRecipe =
            (a == CrafterEntityType.Handle && b == CrafterEntityType.Blade) ||
            (a == CrafterEntityType.Blade && b == CrafterEntityType.Handle);

        if (swordRecipe)
            return CrafterEntityType.IronSword;

        return CrafterEntityType.None;
    }

    public static string GetLabel(CrafterEntityType type)
    {
        switch (type)
        {
            case CrafterEntityType.WoodGenerator: return "Wood\nGen";
            case CrafterEntityType.OreGenerator: return "Ore\nGen";

            case CrafterEntityType.Wood: return "Wood";
            case CrafterEntityType.Plank: return "Plank";
            case CrafterEntityType.Handle: return "Handle";

            case CrafterEntityType.Ore: return "Ore";
            case CrafterEntityType.Ingot: return "Ingot";
            case CrafterEntityType.Blade: return "Blade";

            case CrafterEntityType.IronSword: return "Iron\nSword";
            case CrafterEntityType.RogueEnemy: return "Enemy";
            default: return "";
        }
    }

    public static Color GetColor(CrafterEntityType type)
    {
        switch (type)
        {
            case CrafterEntityType.WoodGenerator: return new Color(0.42f, 0.26f, 0.14f);
            case CrafterEntityType.OreGenerator: return new Color(0.32f, 0.34f, 0.40f);

            case CrafterEntityType.Wood: return new Color(0.72f, 0.52f, 0.30f);
            case CrafterEntityType.Plank: return new Color(0.78f, 0.58f, 0.34f);
            case CrafterEntityType.Handle: return new Color(0.85f, 0.67f, 0.38f);

            case CrafterEntityType.Ore: return new Color(0.48f, 0.50f, 0.56f);
            case CrafterEntityType.Ingot: return new Color(0.68f, 0.70f, 0.78f);
            case CrafterEntityType.Blade: return new Color(0.82f, 0.84f, 0.90f);

            case CrafterEntityType.IronSword: return new Color(0.90f, 0.88f, 0.58f);

            case CrafterEntityType.RogueEnemy: return new Color(0.78f, 0.20f, 0.20f);

            default: return new Color(0f, 0f, 0f, 0f);
        }
    }

    public static int GetSellValueCopper(CrafterEntityType type)
    {
        switch (type)
        {
            case CrafterEntityType.Wood: return 1;
            case CrafterEntityType.Plank: return 2;
            case CrafterEntityType.Handle: return 4;

            case CrafterEntityType.Ore: return 2;
            case CrafterEntityType.Ingot: return 4;
            case CrafterEntityType.Blade: return 8;

            case CrafterEntityType.IronSword: return 20;
            default: return 0;
        }
    }

    public static int GetDestroyXp(CrafterEntityType type)
    {
        switch (type)
        {
            case CrafterEntityType.Wood: return 1;
            case CrafterEntityType.Plank: return 2;
            case CrafterEntityType.Handle: return 3;

            case CrafterEntityType.Ore: return 1;
            case CrafterEntityType.Ingot: return 2;
            case CrafterEntityType.Blade: return 3;

            case CrafterEntityType.IronSword: return 8;
            default: return 0;
        }
    }
}