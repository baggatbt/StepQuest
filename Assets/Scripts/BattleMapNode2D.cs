using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMapNode2D : MapNode2D
{
    [Header("Battle")]
    public BattleConfig battleConfig;     // assign in Inspector
    public string battleSceneName;        // optional override

    public void EnterBattle()
    {
        if (battleConfig == null) { Debug.LogError("No BattleConfig set on node " + nodeID); return; }

        GameManager.Instance.CurrentBattleConfig = battleConfig;
        var sceneToLoad = string.IsNullOrEmpty(battleSceneName)
            ? battleConfig.stage?.battleSceneName
            : battleSceneName;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("No battle scene specified for node " + nodeID);
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
