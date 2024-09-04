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
    public int resourcesProduced; //Claimable items
    public int stepCostToProduce; //Cost in steps
    public Item itemToProduce;

    public void Produce()
{
    if (PlayerData.Instance.inGameSteps >= stepCostToProduce && AreMaterialRequirementsMet())
    {
        // Deduct steps
        PlayerData.Instance.inGameSteps -= stepCostToProduce;

        // Deduct required materials
        DeductMaterialRequirements();

        // Add the produced item to inventory
        GameManager.Instance.AddItem(itemToProduce);
        Debug.Log("Produced 1: " + itemToProduce);
    }
    else
    {
        Debug.Log("Not enough steps or materials to produce the item.");
    }
}

   public void SetItemToProduce(Item itemCurrentlySelected)
   {
    itemToProduce = itemCurrentlySelected;
    stepCostToProduce = itemToProduce.stepCostToProduce;
   }

   private bool AreMaterialRequirementsMet()
{
   
        foreach (var requirement in itemToProduce.materialRequirements)
        {
            // Check if the GameManager's inventory has the required material and quantity
            if (!GameManager.Instance.HasItem(requirement.material, requirement.quantity))
            {
                return false; // Requirement not met
            }
        }
    
    return true; // All requirements are met
}

private void DeductMaterialRequirements()
{  
    
        foreach (var requirement in itemToProduce.materialRequirements)
        {
            // Deduct the required materials using the GameManager's RemoveItem function
            GameManager.Instance.RemoveItem(requirement.material, requirement.quantity);
        }
    
}



    public bool IsProducing()
    {
        return isProducing;
    }

    public void ClaimItems()
    {
        resourcesProduced = 0; //Set resources back to 0
       
    }
}
