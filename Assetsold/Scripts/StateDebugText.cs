using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateDebugText : MonoBehaviour
{
    public BattleManager battleManager;
    private TextMeshProUGUI debugText;

    private void Start()
    {
        debugText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // Update the text based on the current state
        debugText.text = "State: " + battleManager.GetState().ToString();
    }
}
