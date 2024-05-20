using UnityEngine;
using UnityEngine.UI;
using System;

public class EnergyManager : MonoBehaviour
{
    public int maxEnergy = 10;
    public float regenerationTime = 300; // 5 minutes for one energy
    public Text energyText;
    public Text timerText;

    private int currentEnergy;
    private float timer;

    void Start()
    {
        LoadEnergy(); // Load saved energy and timer from PlayerPrefs or other persistent storage
    }

    void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            timer += Time.deltaTime;
            if (timer >= regenerationTime)
            {
                currentEnergy++;
                SaveEnergy(currentEnergy, regenerationTime); // Reset timer and save new energy value
                timer = 0;
            }
            UpdateTimerDisplay(timer);
        }
        energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";
    }

    private void UpdateTimerDisplay(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(regenerationTime - time);
        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void SaveEnergy(int energy, float time)
    {
        // Save current energy and the time until the next regeneration
        PlayerPrefs.SetInt("Energy", energy);
        PlayerPrefs.SetFloat("Timer", time);
        PlayerPrefs.Save();
    }

    private void LoadEnergy()
    {
        // Load energy and timer. If not set, initialize with max energy.
        currentEnergy = PlayerPrefs.GetInt("Energy", maxEnergy);
        timer = PlayerPrefs.GetFloat("Timer", 0);
    }

    public void UseEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            SaveEnergy(currentEnergy, timer); // Save the updated energy value
        }
        else
        {
            Debug.Log("Not enough energy!");
        }
    }

    // Call this method to add energy (e.g., from in-app purchases or rewards)
    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
        SaveEnergy(currentEnergy, timer); // Save the updated energy value
    }
}
