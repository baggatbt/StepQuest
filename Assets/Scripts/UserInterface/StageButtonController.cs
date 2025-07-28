using UnityEngine;
using UnityEngine.UI;

public class StageButtonController : MonoBehaviour
{
    public Button startButton;

    private void OnEnable()
    {
        CheckPartyStamina();
    }

    public void CheckPartyStamina()
    {
        if (startButton == null)
        {
            Debug.LogError("Start button not assigned.");
            return;
        }

        bool allHaveStamina = GameManager.Instance.currentParty.TrueForAll(c => c.stamina > 0);
        startButton.interactable = allHaveStamina;
    }
}
