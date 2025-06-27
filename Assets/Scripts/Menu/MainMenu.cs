using Assets.Scripts.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Playgame()
    {
        LoadingData.targetScene = "MainScene"; 
        SceneManager.LoadScene("LoadingScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
