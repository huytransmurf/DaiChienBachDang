using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool bossDefeated = false;
    public bool hasTalkedToNpc = false;
    public PlayerData playerData = new PlayerData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}