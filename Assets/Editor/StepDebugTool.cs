#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds a menu command & hot‑key so designers can inject test steps at runtime.
/// </summary>
public static class StepDebugTool
{
    //  Menu: Tools ▸ Step Debug ▸ Add 100 Steps   (hot‑key: Ctrl + Shift + K)
    [MenuItem("Tools/Step Debug/Add 100 Steps %#k")]
    private static void AddHundredSteps()
    {
        var player = PlayerData.Instance;
        if (player == null)
        {
            Debug.LogWarning("StepDebugTool: no PlayerData in the scene.");
            return;
        }

        player.DebugAddSteps(100);
    }

    // Validate so the menu item greys‑out when not in Play Mode
    [MenuItem("Tools/Step Debug/Add 100 Steps %#k", validate = true)]
    private static bool AddHundredStepsValidate()
    {
        return Application.isPlaying && PlayerData.Instance != null;
    }
}
#endif
