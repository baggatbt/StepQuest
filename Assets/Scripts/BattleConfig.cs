[System.Serializable]
public class BattleConfig //Responsible for passing information to the battle scene.
{
    public string poolName;  // e.g. "Slimes" for test later will be "Forest", "Mountains" etc etc
    public int maxEnemiesToSpawn;
    public int stageID;
    // Other properties like background music, stage visuals, etc. Will be added here

    // Static instances of BattleConfig for easy access
    public static BattleConfig ForestConfig = new BattleConfig
    {
        poolName = "Forest",
        maxEnemiesToSpawn = 0
    };

    public static BattleConfig MountainConfig = new BattleConfig
    {
        poolName = "Mountains",
        maxEnemiesToSpawn = 0
    };
}