using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeBar : MonoBehaviour
{
    public GameObject emptySegmentPrefab; // Assign in the Inspector
    public GameObject filledSegmentPrefab; // Assign in the Inspector
    public Button upgradeButton; // Assign in the Inspector
    public int maxLevel = 5; // Always set to 5 as per requirements
    private int currentLevel; // Tracks the current progress

    private GameObject[] segments; // Holds the segments (empty or filled)

    public event Action OnMaxLevelReached;

    private void Awake()
    {
        segments = new GameObject[maxLevel]; // Initialize based on maxLevel
        InitializeSegments();
    }

    private void Start()
    {
        upgradeButton.onClick.AddListener(UpgradeStat);
    }

    private void InitializeSegments()
    {
        for (int i = 0; i < maxLevel; i++)
        {
            segments[i] = Instantiate(emptySegmentPrefab, transform);
        }
    }

    public void SetInitialProgress(int progress)
    {
        // Resetting currentLevel to 0 to safely re-assign it based on progress
        currentLevel = 0; 
        // Ensure progress doesn't exceed maxLevel
        progress = Mathf.Min(progress, maxLevel);
        for (int i = 0; i < progress; i++)
        {
            UpgradeSegment(i, true);
        }
        currentLevel = progress; // Update currentLevel after setting progress
    }

    private void UpgradeSegment(int index, bool immediate = false)
    {
        if (index < maxLevel)
        {
            if (!immediate)
            {
                Destroy(segments[index]);
            }
            segments[index] = Instantiate(filledSegmentPrefab, transform);
            segments[index].transform.SetSiblingIndex(index);
        }
    }

    public void UpgradeStat()
    {
        // Check if currentLevel is about to exceed maxLevel
        if (GameManager.Instance.currentCompanion.heroStatPoints > 0 && currentLevel < maxLevel)
        {
            UpgradeSegment(currentLevel);
            currentLevel++; // Safely increment currentLevel
            GameManager.Instance.currentCompanion.heroStatPoints--; // Deduct a stat point

            if (currentLevel == maxLevel)
            {
                OnMaxLevelReached?.Invoke();
            }
        }
        else
        {
            Debug.Log("No stat points available or max level reached.");
        }
    }

    public void ResetSegments()
    {
        foreach (var segment in segments)
        {
            Destroy(segment);
        }
        currentLevel = 0;
        InitializeSegments(); // Reinitialize the segments as empty
    }
}
