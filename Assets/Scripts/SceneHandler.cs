using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void OpenGameplayScene()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

}
