using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMapNode2D : MapNode2D
{
    [Header("Quick Battle Setup")]
    public string battleSceneName = "BattleScene"; // <-- set this to your battle scene
    public string poolName = "Forest";             // <-- enemy pool used by your spawner
    public int enemiesToSpawn = 2;
    public int enemyLevel = 1;

    // call when within range or after step-travel
    public void EnterBattle()
    {
        // Build a minimal BattleConfig at runtime
        var cfg = new BattleConfig
        {
            poolName = poolName,
            maxEnemiesToSpawn = Mathf.Max(1, enemiesToSpawn),
            levelOfEnemies = Mathf.Max(1, enemyLevel),
            currentParty = GameManager.Instance.currentParty // use your current party
        };

        GameManager.Instance.CurrentBattleConfig = cfg;

        if (string.IsNullOrWhiteSpace(battleSceneName))
        {
            Debug.LogError($"[{nodeID}] No battleSceneName set.");
            return;
        }

        Debug.Log($"[{nodeID}] Loading battle: scene={battleSceneName}, pool={poolName}, count={enemiesToSpawn}, level={enemyLevel}");
        SceneManager.LoadScene(battleSceneName);
    }
}
