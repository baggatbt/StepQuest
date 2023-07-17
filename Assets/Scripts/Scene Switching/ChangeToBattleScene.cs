using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToBattleScene : MonoBehaviour
{
    public void LoadBattleScene()
    {
        // Replace "BattleScene" with the name of your scene.
        SceneManager.LoadScene("SampleBattleScene");
    }
}
