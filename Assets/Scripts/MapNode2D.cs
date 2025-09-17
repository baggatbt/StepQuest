using UnityEngine;

public enum NodeKind { Battle, Resource, Shop, Event }

public class MapNode2D : MonoBehaviour
{
    [Header("Node Data")]
    public string nodeID = "Node_001";
    public NodeKind kind = NodeKind.Battle;

    [Header("Geo")]
    public double latitude;
    public double longitude;

    [Header("Interaction")]
    public float interactRadiusMeters = 20f;

    // Visual testing aid
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // approximate ring in world units is drawn by a helper if needed (requires calibration)
    }
}
