using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBar : MonoBehaviour
{
    public GameObject emptySegmentPrefab; // Assign the EmptyUpgradeSegment prefab in the inspector
    public GameObject filledSegmentPrefab; // Assign the FilledUpgradeSegment prefab in the inspector
    public Button upgradeButton; // Assign in the inspector
    public int maxLevel;
    private int currentLevel;
    private GameObject[] segments;

    private void Awake()
    {
        segments = new GameObject[maxLevel];
        InitializeSegments();
        upgradeButton.onClick.AddListener(UpgradeStat);
    }

    private void InitializeSegments()
    {
        // Instantiate the empty segment prefabs
        for (int i = 0; i < maxLevel; i++)
        {
            segments[i] = Instantiate(emptySegmentPrefab, transform);
        }
    }

    private void UpgradeStat()
{
    if (currentLevel < maxLevel)
    {
        // Get the position, rotation, and parent from the current segment to be upgraded
        Vector3 position = segments[currentLevel].transform.localPosition; // Use localPosition for UI elements
        Quaternion rotation = segments[currentLevel].transform.localRotation;
        Transform parent = segments[currentLevel].transform.parent;

        // Destroy the current empty segment
        Destroy(segments[currentLevel]);

        // Instantiate a new filled segment at the correct position
        segments[currentLevel] = Instantiate(filledSegmentPrefab, parent);
        segments[currentLevel].transform.localPosition = position;
        segments[currentLevel].transform.localRotation = rotation;
        
        // Ensure the new segment has the same sibling index so it appears in the correct order in the UI hierarchy
        segments[currentLevel].transform.SetSiblingIndex(currentLevel);

        // Increment the current level
        currentLevel++;
        Debug.Log("Upgraded");

        // Disable the button if max level is reached
        if (currentLevel >= maxLevel)
        {
            upgradeButton.interactable = false;
        }
    }
}

}
