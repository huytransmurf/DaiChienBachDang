using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private PlayerCombat playerCombat;


    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                Pause();
                playerCombat.enabled = false;
            }
            else
            {
                Resume();
                playerCombat.enabled = true;

            }
        }
    }

    public void Home()
    {
        Time.timeScale = 1; // Đảm bảo thời gian trở lại bình thường trước khi chuyển cảnh
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }
}
