using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadBattleScene()
    {
        
        SceneManager.LoadScene("TestPortraitBattle");
    }

     public void GoToMainMenuScene()
    {
        GameManager.Instance.RecoverCompanion();
        SceneManager.LoadScene("CharacterInfoPage");
    }


   
}
