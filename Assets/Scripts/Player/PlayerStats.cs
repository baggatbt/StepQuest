using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Namespace for TextMeshPro

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public int exp = 0;
    public int gold = 0;

    // The TextMeshProUGUI references.
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;

    void Update()
    {
        // Convert the level, exp, and gold int values to strings and update the text fields.
        levelText.text = "Level: " + level.ToString();
        expText.text = "EXP: " + exp.ToString();
        goldText.text = "Gold: " + gold.ToString();
    }
}
