using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource backgroundAudio;

    [SerializeField]
    private AudioSource effectAudio;

    [SerializeField]
    private AudioClip backgroundAudioClips;

    [SerializeField]
    private AudioClip jumpAudioClips;

    [SerializeField]
    private AudioClip attackAudioClips;


    void Start()
    {
        PlayBackgroundAudio();
    }

    // Update is called once per frame
    void Update() { }

    public void PlayJumpAudio()
    {
        effectAudio.PlayOneShot(jumpAudioClips);
    }

    public void PlayAttackAudio()
    {
        effectAudio.PlayOneShot(attackAudioClips);
    }

    public void PlayBackgroundAudio()
    {
        backgroundAudio.clip = backgroundAudioClips;
        backgroundAudio.Play();
    }

    
}
