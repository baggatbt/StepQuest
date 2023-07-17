using UnityEngine;
using UnityEngine.SceneManagement;

public class EndBattleScene : MonoBehaviour
{

    public void ExitBattleScene()
    {
        // Replace "BattleScene" with the name of your scene.
        SceneManager.LoadScene("CharacterInfoPage");
    }


}
