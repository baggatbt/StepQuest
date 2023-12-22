using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayValues : MonoBehaviour
{
    public TextMeshProUGUI stepsText;

    private void Update()
    {
        UpdateUI();
    }

     private void UpdateUI()
    {  
        if (stepsText != null) stepsText.text =  PlayerData.Instance.inGameSteps.ToString();   
    }

}
