using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    NormalBattle,
    EliteBattle,
    RandomEvent
}


public class MapNode
{
    public NodeType NodeType { get; set; }
    public Vector2 Position { get; set; }

    public MapNode(Vector2 position)
    {
        Position = position;
        AssignRandomNodeType();
    }

    private void AssignRandomNodeType()
    {
        // Simple random assignment, can be replaced with weighted randomness
        int rand = Random.Range(0, 3); // Assuming three node types
        NodeType = (NodeType)rand;
    }
}




