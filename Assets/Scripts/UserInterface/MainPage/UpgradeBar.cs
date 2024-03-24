using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class UpgradeBar : MonoBehaviour
{
    public GameObject emptySegmentPrefab; // Assign the EmptyUpgradeSegment prefab in the inspector
    public GameObject filledSegmentPrefab; // Assign the FilledUpgradeSegment prefab in the inspector
    public Button upgradeButton; // Assign in the inspector
    public int maxLevel;
    public int currentLevel;
    public int associatedStat;
    private GameObject[] segments;
    public event Action OnMaxLevelReached;

    private void Awake()
    {
        maxLevel = 3;
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

    public void UpgradeStat()
    {
        if (GameManager.Instance.currentCompanion.heroStatPoints > 0)
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
            GameManager.Instance.currentCompanion.heroStatPoints--;
            Debug.Log("Upgraded");

            // Reset the button if max level is reached
            if (currentLevel == maxLevel)
            {
                ResetSegments();
            }
        }
        }
    }
    private void ResetSegments()
    {
        foreach (var segment in segments)
        {
            currentLevel -= 1;
            // Get the position, rotation, and parent from the current segment to be upgraded
            Vector3 position = segments[currentLevel].transform.localPosition; // Use localPosition for UI elements
            Quaternion rotation = segments[currentLevel].transform.localRotation;
            Transform parent = segments[currentLevel].transform.parent;

            // Destroy the current segment
            Destroy(segments[currentLevel]);

            // Instantiate a new empty segment at the correct position
            segments[currentLevel] = Instantiate(emptySegmentPrefab, parent);
            segments[currentLevel].transform.localPosition = position;
            segments[currentLevel].transform.localRotation = rotation;
            
            // Ensure the new segment has the same sibling index so it appears in the correct order in the UI hierarchy
            segments[currentLevel].transform.SetSiblingIndex(currentLevel);


            
        }
        currentLevel = 0;
        // Trigger the event to notify subscribers that the max level was reached
        OnMaxLevelReached?.Invoke();
    }

}
