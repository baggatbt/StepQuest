using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimingVisualAid : MonoBehaviour
{
    public GameObject timingBarObject; // The GameObject of the Image to toggle visibility
    private bool isTiming = false;

    void Start()
    {
        // Make sure the timing bar is invisible at the start
        if (timingBarObject != null)
        {
            timingBarObject.SetActive(false);
        }
    }

    // Method to start the timing window
    public void StartTimingWindow()
{
    isTiming = true;
    if (timingBarObject != null)
    {
        timingBarObject.SetActive(true); // Show the timing bar when the window starts
        Debug.Log("Timing window started and bar is now visible.");
    }
}

public void EndTimingWindow()
{
    isTiming = false;
    if (timingBarObject != null)
    {
        timingBarObject.SetActive(false); // Hide the timing bar when the window ends
        Debug.Log("Timing window ended and bar is now hidden.");
    }
}

}
