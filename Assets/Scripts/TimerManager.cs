using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour //USES UTC, Convert to correct timezone in UI if necessary
{
    public static TimerManager Instance { get; private set; }

    public Dictionary<string, DateTime> timers = new Dictionary<string, DateTime>();
    public delegate void TimerCompleted(string timerId);
    public event TimerCompleted OnTimerCompleted;

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
    

    private void Update()
    {
        List<string> completedTimers = new List<string>();
        foreach (var timer in timers)
        {
            if (GetRemainingTime(timer.Key) <= TimeSpan.Zero)
            {
                completedTimers.Add(timer.Key);
                Debug.Log($"Timer {timer.Key} has finished.");
            }
        }

        foreach (var timerId in completedTimers)
        {
            timers.Remove(timerId);
            OnTimerCompleted?.Invoke(timerId);
        }
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

    public void SetPeriodicTimer(string timerId, float durationInSeconds)
{
    DateTime endTime = DateTime.UtcNow.AddSeconds(durationInSeconds);
    if (timers.ContainsKey(timerId))
    {
        DateTime lastEndTime = timers[timerId];
        if (lastEndTime < DateTime.UtcNow)
        {
            // Calculate missed intervals
            double totalSecondsMissed = (DateTime.UtcNow - lastEndTime).TotalSeconds;
            int missedIntervals = (int)(totalSecondsMissed / durationInSeconds);
            for (int i = 0; i <= missedIntervals; i++)
            {
                OnTimerCompleted?.Invoke(timerId);
            }
            // Set next due time after handling all missed intervals
            endTime = DateTime.UtcNow.AddSeconds(durationInSeconds * (missedIntervals + 1));
        }
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
    SaveTimerKeys(); // Save keys first to ensure they are up-to-date
    foreach (var timer in timers)
    {
        PlayerPrefs.SetString("Timer_" + timer.Key, timer.Value.ToBinary().ToString());
    }
    PlayerPrefs.Save();
}


    // Load timers from PlayerPrefs or another persistent storage
    private void LoadTimers()
    {
        timers.Clear(); // Clear existing timers before loading new ones
        string[] keys = PlayerPrefs.GetString("TimerKeys", "").Split(',');
        foreach (string key in keys)
        {
            if (PlayerPrefs.HasKey("Timer_" + key))
            {
                long temp = Convert.ToInt64(PlayerPrefs.GetString("Timer_" + key));
                DateTime endTime = DateTime.FromBinary(temp);
                timers[key] = endTime;

                // Immediately handle the timer if it's overdue
                if (endTime <= DateTime.UtcNow)
                {
                    SetPeriodicTimer(key, 1800);  // Assuming 1800 is the period in seconds
                }
            }
        }
    }

    private void SaveTimerKeys()
    {
        string keys = string.Join(",", timers.Keys);
        PlayerPrefs.SetString("TimerKeys", keys);
    }

    private void CheckAndHandleExpiredTimers()
    {
        List<string> expiredTimers = new List<string>();
        foreach (var timer in timers)
        {
            if (GetRemainingTime(timer.Key) <= TimeSpan.Zero)
            {
                expiredTimers.Add(timer.Key);
            }
        }

        foreach (var timerId in expiredTimers)
        {
            timers.Remove(timerId);
            OnTimerCompleted?.Invoke(timerId);
        }
    }


    
}
