#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TimerManagerEditor : EditorWindow
{
    [MenuItem("Tools/Timer Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TimerManagerEditor), false, "Timer Manager");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
        {
            // Trigger refresh or update of the displayed data
        }

        if (Application.isPlaying && TimerManager.Instance != null)
        {
            foreach (var timer in TimerManager.Instance.timers)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Timer ID: {timer.Key}, Ends At: {timer.Value}");
                if (GUILayout.Button("Reset Timer"))
                {
                    // Add logic to reset this specific timer
                    TimerManager.Instance.SetTimer(timer.Key, 1800);  // Reset to 30 minutes for example
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("TimerManager is not available. Start the game.");
        }
    }
}
#endif