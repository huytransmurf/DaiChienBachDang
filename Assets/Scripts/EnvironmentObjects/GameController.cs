using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject Minigame;
    private NpcMinigame NpcMinigame;

    private bool isGameStarted = false;

    void Start()
    {
        NpcMinigame = FindObjectOfType<NpcMinigame>();
    }

    void Update()
    {
        if (NpcMinigame == null) return;

        if (NpcMinigame.isdonetalk && !isGameStarted)
        {
            StarGame();
            isGameStarted = true;
        }
    }

    public void Home()
    {
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        Minigame.SetActive(false);
        Time.timeScale = 1;
        isGameStarted = false;
        NpcMinigame.isdonetalk = false;
    }

    public void StarGame()
    {
        Minigame.SetActive(true);
        Time.timeScale = 0;
    }
}
