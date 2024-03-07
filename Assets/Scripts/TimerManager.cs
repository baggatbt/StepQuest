using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    private Dictionary<string, DateTime> timers = new Dictionary<string, DateTime>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadTimers(); // Load saved timers when the game starts
    }

    // Start or reset a timer with a given ID and duration in seconds
    public void SetTimer(string timerId, float durationInSeconds)
    {
        DateTime endTime = DateTime.UtcNow.AddSeconds(durationInSeconds);
        if (timers.ContainsKey(timerId))
        {
            timers[timerId] = endTime;
        }
        else
        {
            timers.Add(timerId, endTime);
        }
        SaveTimers();
    }

    // Check how much time is remaining for a specific timer
    public TimeSpan GetRemainingTime(string timerId)
    {
        if (!timers.ContainsKey(timerId)) return TimeSpan.Zero;

        TimeSpan timeLeft = timers[timerId].Subtract(DateTime.UtcNow);
        return timeLeft > TimeSpan.Zero ? timeLeft : TimeSpan.Zero;
    }

    // a method to remove a timer, call after a timer ends
    public void RemoveTimer(string timerId)
    {
        if (timers.ContainsKey(timerId))
        {
            timers.Remove(timerId);
            SaveTimers();
        }
    }

    // Save timers to PlayerPrefs or another persistent storage
    private void SaveTimers()
    {
        foreach (var timer in timers)
        {
            PlayerPrefs.SetString("Timer_" + timer.Key, timer.Value.ToBinary().ToString());
        }
        PlayerPrefs.Save();
    }

    // Load timers from PlayerPrefs or another persistent storage
    private void LoadTimers()
    {
        foreach (var timerKey in timers.Keys)
        {
            if (PlayerPrefs.HasKey("Timer_" + timerKey))
            {
                long temp = Convert.ToInt64(PlayerPrefs.GetString("Timer_" + timerKey));
                timers[timerKey] = DateTime.FromBinary(temp);
            }
        }
    }

    private void Update()
    {
        // Example usage in Update for demonstration. Consider optimizing for your needs.
        foreach (var timer in timers)
        {
            if (GetRemainingTime(timer.Key) <= TimeSpan.Zero)
            {
                Debug.Log($"Timer {timer.Key} has finished.");
                // Perform action for timer completion
                // Consider removing or resetting the timer here if appropriate
            }
        }
    }
}
