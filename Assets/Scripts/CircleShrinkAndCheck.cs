using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShrinkAndCheck : MonoBehaviour
{
    public RectTransform circleTransform; // Assign your circle in the inspector
    public RectTransform bubbleTransform; // Assign your bubble in the inspector
    public float shrinkSpeed = 50f; // Adjust the speed at which the circle shrinks
    public float successCheckDelay = 1f; // Time in seconds to wait before checking success
    private Vector2 initialCircleSize; // Variable to store the initial size of the circle
    public bool isShrinking = false;

    void Start()
    {
        // Store the initial size of the circle at the start
        initialCircleSize = circleTransform.sizeDelta;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Ensure the circle is reset to its initial size every time shrinking starts
            circleTransform.sizeDelta = initialCircleSize;
            isShrinking = true;
        }

        if (isShrinking)
        {
            circleTransform.sizeDelta -= new Vector2(shrinkSpeed, shrinkSpeed) * Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) && isShrinking)
        {
            isShrinking = false;
            StartCoroutine(CheckSuccessAfterDelay());
        }
    }

    IEnumerator CheckSuccessAfterDelay()
{
    yield return new WaitForSeconds(successCheckDelay); // Wait for the specified delay

    // Instead of a fixed tolerance, consider the bubble size as a percentage of the initial circle size to determine a dynamic tolerance
    float bubbleSizeAsPercentageOfInitialCircleX = bubbleTransform.sizeDelta.x / initialCircleSize.x;
    float bubbleSizeAsPercentageOfInitialCircleY = bubbleTransform.sizeDelta.y / initialCircleSize.y;

    // Dynamic tolerance could be a fraction of the bubble's size percentage, ensuring flexibility based on bubble's current size
    // This could be fine-tuned based on the desired difficulty
    float dynamicToleranceX = bubbleSizeAsPercentageOfInitialCircleX / 2; // Example: half of the bubble's size percentage
    float dynamicToleranceY = bubbleSizeAsPercentageOfInitialCircleY / 2;

    // Calculate the size difference in terms of percentage of the initial circle size
    float sizeDifferenceX = Mathf.Abs(circleTransform.sizeDelta.x - bubbleTransform.sizeDelta.x) / initialCircleSize.x;
    float sizeDifferenceY = Mathf.Abs(circleTransform.sizeDelta.y - bubbleTransform.sizeDelta.y) / initialCircleSize.y;

    // Check if the circle's size difference falls within the dynamic tolerance range
    if (sizeDifferenceX <= dynamicToleranceX && sizeDifferenceY <= dynamicToleranceY)
    {
        Debug.Log("Success: Circle is correctly sized for the bubble.");
    }
    else
    {
        Debug.Log("Failed: Circle size does not match the bubble adequately.");
    }

    // Reset the circle size after checking for success/failure
    circleTransform.sizeDelta = initialCircleSize;
}


}
