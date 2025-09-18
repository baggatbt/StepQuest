using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeSet", menuName = "World/Node Set")]
public class NodeSet : ScriptableObject
{
    public List<NodeData> nodes = new();
}
