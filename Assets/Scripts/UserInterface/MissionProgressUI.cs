using UnityEngine;
using UnityEngine.UI;

public class MissionProgressUI : MonoBehaviour
{
    [Tooltip("Reference to the mission we want to visualise.")]
    public ResourceMission mission;

    [Tooltip("Slider whose value will track mission progress.")]
    public Slider progressSlider;

    private void Awake()
    {
        if (mission == null)         Debug.LogError("MissionProgressUI: mission reference missing!");
        if (progressSlider == null)  Debug.LogError("MissionProgressUI: slider reference missing!");

        // Ensure slider is configured (handy when you duplicate prefab)
        progressSlider.minValue    = 0f;
        progressSlider.maxValue    = 1f;
        progressSlider.wholeNumbers = false;
        progressSlider.interactable = false;
    }

    private void Update()
    {
        // Show real‑time status every frame
        progressSlider.value = mission.GetProgressNormalized();
    }
}
