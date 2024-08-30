using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string buildingName;
    public int level;
    public bool isProducing;
    public int stepsRequiredForProduction;
    public int totalResourcesProduced; // Track the total resources produced

    public void Produce(int steps)
    {
        int resourcesProduced = steps / stepsRequiredForProduction;
        totalResourcesProduced += resourcesProduced;

        Debug.Log($"{buildingName} produced {resourcesProduced} resources. Total: {totalResourcesProduced}");
    }

    public int CalculatePotentialProduction(int steps)
    {
        return steps / stepsRequiredForProduction;
    }

    public bool IsProducing()
    {
        return isProducing;
    }
}
