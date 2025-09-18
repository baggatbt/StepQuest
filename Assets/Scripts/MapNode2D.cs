using UnityEngine;

public enum NodeKind { Battle, Resource, Shop, Event }

public class MapNode2D : MonoBehaviour
{
    [Header("Node Data")]
    public string nodeID = "Node_001";
    public NodeKind kind = NodeKind.Battle;
    [Tooltip("Meters radius the player must be within to Interact (E).")]
    public float interactRadiusMeters = 25f;

    [Header("Geo")]
    public double latitude;
    public double longitude;
}
