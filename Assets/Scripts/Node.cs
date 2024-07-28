using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string nodeName;
    public Vector2 position; // Position on the map
    public List<Node> neighbors = new List<Node>(); // Adjacent nodes

     public static int CalculateCost(Node fromNode, Node toNode)
    {
        // Example: base cost on distance
        float distance = Vector2.Distance(fromNode.position, toNode.position);

        // Calculate the cost (e.g., 1 unit per unit distance)
        int cost = Mathf.RoundToInt(distance); // Adjust this calculation as needed

        return cost;
    }
}
