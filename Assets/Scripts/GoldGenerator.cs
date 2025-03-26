using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/*
public class GoldGenerator : MonoBehaviour
{
    public string timerId = "GoldGenerator";
    public float goldGenerationInterval = 1800f; // Interval in seconds (30 mins base)
    public int baseGoldAmount = 50; // Base amount of gold
    public int maxGoldUncollected = 300; // Maximum gold that can be uncollected
    private int uncollectedGold = 0; // Tracks how much gold is uncollected
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
    // Check if the timer has completed while the object was inactive or in another scene
    TimeSpan remainingTime = TimerManager.Instance.GetRemainingTime(timerId);

    if (remainingTime <= TimeSpan.Zero)
    {
        int goldToGenerate = CalculateGoldAmount();
        if (uncollectedGold + goldToGenerate <= maxGoldUncollected)
        {
            uncollectedGold += goldToGenerate;
            goldReadyToCollect = true;
            TimerManager.Instance.SetPeriodicTimer(timerId, goldGenerationInterval); // Restart timer
        }
        else
        {
            uncollectedGold = maxGoldUncollected;
            goldReadyToCollect = true;
        }

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
            int goldToGenerate = CalculateGoldAmount();

            // Check if adding the generated gold exceeds the cap
            if (uncollectedGold + goldToGenerate <= maxGoldUncollected)
            {
                uncollectedGold += goldToGenerate;
                goldReadyToCollect = true; // Gold is ready to collect
                TimerManager.Instance.SetPeriodicTimer(timerId, goldGenerationInterval); // Restart timer
                Debug.Log($"Gold ready to collect! Uncollected Gold: {uncollectedGold}");
            }
            else
            {
                // Cap reached, stop generating
                uncollectedGold = maxGoldUncollected;
                goldReadyToCollect = true;
                Debug.Log($"Gold cap reached! Uncollected Gold: {uncollectedGold}");
            }
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
            PlayerData.Instance.gold += uncollectedGold; // Add uncollected gold to player's total
            Debug.Log($"Gold collected: {uncollectedGold}");
            uncollectedGold = 0; // Reset uncollected gold
            goldReadyToCollect = false; // Reset flag
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
*/
