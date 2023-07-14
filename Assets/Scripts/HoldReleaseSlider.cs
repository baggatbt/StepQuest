using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldReleaseSlider : MonoBehaviour
{
    public Slider holdSlider;
    private float maxValue = 1.0f;

    private void Awake()
    {
        holdSlider.maxValue = maxValue;
        holdSlider.value = 0;
    }

    public void UpdateSlider(float value)
    {
        holdSlider.value = value;
    }

    public void ResetSlider()
    {
        holdSlider.value = 0;
    }
}