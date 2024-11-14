using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInteraction : MonoBehaviour
{
    public MapNode NodeData;

    void OnMouseDown() // This method is called when the node is clicked
    {
        switch (NodeData.NodeType)
        {
            case NodeType.NormalBattle:
                StartBattle("Normal");
                break;
            case NodeType.EliteBattle:
                StartBattle("Elite");
                break;
            case NodeType.RandomEvent:
                TriggerEvent();
                break;
        }
    }

    void StartBattle(string type)
    {
        Debug.Log($"Starting {type} battle.");
        // Add battle logic here
    }

    void TriggerEvent()
    {
        Debug.Log("Triggering random event.");
        // Add event logic here
    }
}