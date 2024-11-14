using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GoldGenerator : MonoBehaviour
{
    public string timerId = "GoldGenerator";
    public float goldGenerationInterval = 1800f; // Interval in seconds (30 mins base)
    public int baseGoldAmount = 50; // Base amount of gold
    public int maxGoldUncollected = 300; //Temp placeholder value
    private int collectedGold = 0;
    private bool goldReadyToCollect = false; // Flag to track if gold is ready
    

    public TextMeshProUGUI timerText; // Assign your TMP text element in the Inspector

    // Player attributes and other modifiers
    public int playerLevel = 1;
    public float levelMultiplier = 1.0f; // Example: each level increases gold by X
    public float otherModifiers = 1.0f;  // Additional multiplier (e.g., from skills, items)

    private void Start()
    {
        TimerManager.Instance.SetPeriodicTimer(timerId, goldGenerationInterval);
        TimerManager.Instance.OnTimerCompleted += OnGoldGenerationComplete;
    }

    private void OnDestroy()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerCompleted -= OnGoldGenerationComplete;
        }
    }

    private void OnEnable()
    {
        // Check if the timer has completed while the object was inactive
        if (TimerManager.Instance.GetRemainingTime(timerId) <= TimeSpan.Zero)
        {
            goldReadyToCollect = true;
            if (timerText != null) timerText.text = "Gold ready to collect!";
        }
    }

    private void Update()
    {
        if (!goldReadyToCollect && timerText != null)
        {
            TimeSpan timeRemaining = TimerManager.Instance.GetRemainingTime(timerId);
            timerText.text = FormatTime(timeRemaining);
        }
        else if (goldReadyToCollect && timerText != null)
        {
            timerText.text = "Gold ready to collect!";
        }
    }

    private void OnGoldGenerationComplete(string completedTimerId)
    {
        if (completedTimerId == timerId)
        {
            goldReadyToCollect = true; // Set flag to indicate gold is ready
            TimerManager.Instance.SetPeriodicTimer(timerId, goldGenerationInterval); // Reset timer
            Debug.Log("Gold is ready to collect!"); // Optional notification
        }
    }

    private int CalculateGoldAmount()
    {
        // Calculate gold based on level and modifiers
        float goldAmount = baseGoldAmount * Mathf.Pow(levelMultiplier, playerLevel - 1) * otherModifiers;
        return Mathf.RoundToInt(goldAmount);
    }

    public void CollectGold()
    {
        if (goldReadyToCollect)
        {
            int goldToCollect = CalculateGoldAmount();
            collectedGold += goldToCollect;
            goldReadyToCollect = false; // Reset flag after collecting
            Debug.Log($"Gold collected: {collectedGold} (Generated: {goldToCollect})");

            // Optionally, update the UI with the collected gold amount here.
        }
        else
        {
            Debug.Log("Gold is not ready to collect yet.");
        }
    }

    private string FormatTime(TimeSpan time)
    {
        return $"{time.Minutes:D2}:{time.Seconds:D2}";
    }
}
