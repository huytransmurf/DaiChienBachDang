using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool bossDefeated = false;
    public bool hasTalkedToNpc = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
}
