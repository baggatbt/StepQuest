using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBar : MonoBehaviour
{
    public GameObject segmentPrefab; // This will use EmptyUpgradeSegment prefab
    public Button upgradeButton; // Assign in the inspector
    public int maxLevel;
    private int currentLevel = 0;
    private Image[] segments;

    // Add references to your sprites here
    private Sprite emptySprite;
    private Sprite filledSprite;

    private void Awake()
    {
        // Load sprites from the Resources folder
        emptySprite = Resources.Load<Sprite>("PreFab/UI Elements/EmptyUpgradeSegment");
        filledSprite = Resources.Load<Sprite>("PreFab/UI Elements/FilledUpgradeSegment");

        // Now create an array to hold the segment images
        segments = new Image[maxLevel];
        InitializeSegments();
        upgradeButton.onClick.AddListener(UpgradeStat);
    }

    private void InitializeSegments()
    {
        for (int i = 0; i < maxLevel; i++)
        {
            // Instantiate the segmentPrefab which should be the empty segment
            GameObject newSegment = Instantiate(segmentPrefab, transform);
            segments[i] = newSegment.GetComponent<Image>();
            // Initially, all segments display the empty sprite
            segments[i].sprite = emptySprite;
        }
    }

    private void UpgradeStat()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            UpdateBarDisplay();
            // Increase the actual stat here
        }

        if (currentLevel >= maxLevel)
        {
            upgradeButton.interactable = false; // Disables the button when max level is reached
        }
    }

    private void UpdateBarDisplay()
    {
        for (int i = 0; i < currentLevel; i++)
        {
            // Update only the segment that corresponds to the current level
            segments[i].sprite = filledSprite; // Change to the filled sprite
        }
    }

    // No need for variables or methods for sprite management
    // since they're loaded from the Resources folder at Awake
}
