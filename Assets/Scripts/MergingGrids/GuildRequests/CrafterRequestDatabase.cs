using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Request Database")]
public class CrafterRequestDatabase : ScriptableObject
{
    public List<CrafterRequestDefinition> requests = new();

    public CrafterRequestDefinition GetRandom()
    {
        if (requests == null || requests.Count == 0)
            return null;

        return requests[Random.Range(0, requests.Count)];
    }
}