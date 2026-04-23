using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [Header("Audio Setup")]
    [Tooltip("AudioSource that will play the music.")]
    public AudioSource audioSource;

    [Tooltip("Music clip to be played.")]
    public AudioClip musicClip;

    [Header("Playback Settings")]
    [Tooltip("Time in seconds to wait AFTER the music finishes.")]
    public float playbackDelay;

    [Tooltip("If true, music will play once at the start.")]
    public bool playOnStart;

    void Start()
    {
        audioSource.clip = musicClip;

        if (playOnStart)
        {
            StartCoroutine(MusicLoop());
        }
    }

    IEnumerator MusicLoop()
    {
        while (true)
        {
            // Play the music
            audioSource.Play();

            // Wait until the clip finishes
            yield return new WaitForSeconds(audioSource.clip.length);

            // Wait your extra delay
            yield return new WaitForSeconds(playbackDelay);
        }
    }

    public void PlayMusic()
    {
        StartCoroutine(MusicLoop());
    }
}