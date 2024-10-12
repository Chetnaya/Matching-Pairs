using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audio;
    public AudioSource BackgroundMusic;

    private static float vol = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Ensure background music starts playing if it's not muted
        if (!BackgroundMusic.mute && !BackgroundMusic.isPlaying)
        {
            BackgroundMusic.Play();
        }
    }

    public void PlayAudio(int id)
    {
        audioSource.PlayOneShot(audio[id]);
    }

    public void PlayAudio(int id, float vol)
    {
        audioSource.PlayOneShot(audio[id], vol);
    }
}
