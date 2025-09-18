using UnityEngine;

[System.Serializable]
public class NodeData
{
    public string nodeID;
    public NodeKind kind = NodeKind.Battle;
    public double latitude;
    public double longitude;
    public float interactRadiusMeters = 25f;

    // Battle-only quick fields
    public string battleSceneName = "TestPortraitBattle";
    public string poolName = "Forest";
    public int enemiesToSpawn = 1;
    public int enemyLevel = 1;
}
