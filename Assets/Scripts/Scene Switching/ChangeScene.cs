using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadBattleScene()
    {
        
        SceneManager.LoadScene("SampleBattleScene");
    }

     public void GoToMainMenuScene()
    {
        
        SceneManager.LoadScene("CharacterInfoPage");
    }


   
}
