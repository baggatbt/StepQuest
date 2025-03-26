using UnityEngine;
using System.Collections.Generic;

public static class CurrencyManager
{
    // Denominations:
    // 1 Silver    = 100 Copper
    // 1 Gold      = 100 Silver    = 10,000 Copper
    // 1 Platinum  = 100 Gold      = 1,000,000 Copper
    // 1 Diamond   = 100 Platinum  = 100,000,000 Copper
    public const long COPPER_PER_SILVER  = 100;
    public const long COPPER_PER_GOLD    = 10000;
    public const long COPPER_PER_PLAT    = 1000000;
    public const long COPPER_PER_DIAMOND = 100000000;

    /// <summary>
    /// Splits totalCopper into whole-number counts for each coin type.
    /// Returns (diamond, platinum, gold, silver, copper).
    /// </summary>
    public static (long diamond, long platinum, long gold, long silver, long copper)
        GetDenominationCounts(long totalCopper)
    {
        long diamond = totalCopper / COPPER_PER_DIAMOND;
        long leftover = totalCopper % COPPER_PER_DIAMOND;

        long platinum = leftover / COPPER_PER_PLAT;
        leftover = leftover % COPPER_PER_PLAT;

        long gold = leftover / COPPER_PER_GOLD;
        leftover = leftover % COPPER_PER_GOLD;

        long silver = leftover / COPPER_PER_SILVER;
        leftover = leftover % COPPER_PER_SILVER;

        long copper = leftover;

        return (diamond, platinum, gold, silver, copper);
    }

    /// <summary>
    /// Returns a multi-line string showing each coin count.
    /// For example:
    /// "Diamond: 2
    /// Platinum: 5
    /// Gold: 47
    /// Silver: 33
    /// Copper: 12"
    /// </summary>
    public static string GetMultiCoinString(long totalCopper)
    {
        var counts = GetDenominationCounts(totalCopper);
        List<string> lines = new List<string>();

        if (counts.diamond > 0) lines.Add("Diamond: " + counts.diamond);
        if (counts.platinum > 0) lines.Add("Platinum: " + counts.platinum);
        if (counts.gold > 0) lines.Add("Gold: " + counts.gold);
        if (counts.silver > 0) lines.Add("Silver: " + counts.silver);
        if (counts.copper > 0) lines.Add("Copper: " + counts.copper);

        // If totalCopper is zero, show "Copper: 0"
        if (lines.Count == 0)
            lines.Add("Copper: 0");

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Adds the specified amount of copper.
    /// </summary>
    public static void AddCopper(ref long totalCopper, long amountToAdd)
    {
        totalCopper += amountToAdd;
        if (totalCopper < 0) totalCopper = 0;
    }

    /// <summary>
    /// Attempts to spend the specified amount of copper.
    /// Returns true if successful.
    /// </summary>
    public static bool SpendCopper(ref long totalCopper, long amountToSpend)
    {
        if (totalCopper >= amountToSpend)
        {
            totalCopper -= amountToSpend;
            return true;
        }
        return false;
    }
}
