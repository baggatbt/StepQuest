using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler
{
    public string nodeName;
    public Vector2 position; // Position on the map
    public List<Node> neighbors = new List<Node>(); // Adjacent nodes

    private MainMenuUIManager uiManager;

    private void Start()
    {
        // Find the MainMenuUIManager in the scene (make sure there's only one instance)
        uiManager = FindObjectOfType<MainMenuUIManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiManager != null)
        {
            uiManager.ShowNodeInfo(nodeName);
        }
    }

    public static int CalculateCost(Node fromNode, Node toNode)
    {
        float distance = Vector2.Distance(fromNode.position, toNode.position);
        int cost = Mathf.RoundToInt(distance) * 100; // Adjust this calculation as needed
        return cost;
    }
}
