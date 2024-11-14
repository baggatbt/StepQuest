using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject nodePrefab; // The prefab for the nodes
    public Transform stageNodes; // The parent object for all nodes
    public List<StageConfig> stageConfigs; // List of all stage configurations

    void Start()
    {
        GenerateNodes();
    }

    void GenerateNodes()
    {
        foreach (StageConfig config in stageConfigs)
        {
            CreateNode(config);
        }
    }

    void CreateNode(StageConfig config)
{
    // Corrected instantiation to include the parent properly
    GameObject nodeObject = Instantiate(nodePrefab, config.position, Quaternion.identity, stageNodes);
    nodeObject.transform.localPosition = config.position;

    Stage stageComponent = nodeObject.GetComponent<Stage>();
    if (stageComponent != null)
    {
        stageComponent.Configure(config);
    }
    else
    {
        Debug.LogError("Stage component not found on node prefab!");
    }
}

}
