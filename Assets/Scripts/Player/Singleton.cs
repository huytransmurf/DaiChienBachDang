using UnityEngine;

public class Singleton : MonoBehaviour
{
    private static Singleton _instance;
    public bool dialogActive = false;
    public static Singleton Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
