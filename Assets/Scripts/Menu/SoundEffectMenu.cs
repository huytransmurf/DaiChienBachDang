using UnityEngine;

public class SoundEffectMenu : MonoBehaviour
{
    [SerializeField]
    private AudioSource src;
    [SerializeField]
    private AudioClip sfx1, sfx2, sfx3;

    public void StartBtn()
    {
        src.clip = sfx1;
        src.Play();
    }
}
