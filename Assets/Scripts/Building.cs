using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Building : MonoBehaviour
{
    public string buildingName;
    public int level;
    public bool isProducing;
    public int stepsRequiredForProduction;

    public bool IsProducing()
    {
        return isProducing;
    }

    public int StepsRequiredForProduction()
    {
        return stepsRequiredForProduction;
    }

    public void Produce()
    {
        // Logic for producing resources
        Debug.Log(buildingName + " produced resources.");
    }

    public void PartialProduce(int stepsAvailable)
    {
        // Logic for partially producing resources with available steps
        Debug.Log(buildingName + " partially produced resources with " + stepsAvailable + " steps.");
    }
}
