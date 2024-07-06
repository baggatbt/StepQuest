using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleConfig
{
    public string poolName;
    public int maxEnemiesToSpawn;
    public int levelOfEnemies;
    public Stage stage;
    public List<Companion> currentParty = new List<Companion>();

    public static BattleConfig ForestConfig = new BattleConfig
    {
        poolName = "Forest",
        maxEnemiesToSpawn = 0,
        levelOfEnemies = 1,
        currentParty = new List<Companion>()
    };

    public static BattleConfig ForestEliteConfig = new BattleConfig
    {
        poolName = "ForestElite",
        maxEnemiesToSpawn = 0,
        levelOfEnemies = 1,
        currentParty = new List<Companion>()
    };

    public static BattleConfig MountainConfig = new BattleConfig
    {
        poolName = "Mountains",
        maxEnemiesToSpawn = 0,
        levelOfEnemies = 1,
        currentParty = new List<Companion>()
    };
}
