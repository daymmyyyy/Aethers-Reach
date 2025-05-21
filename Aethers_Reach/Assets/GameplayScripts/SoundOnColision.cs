using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundOnCollision : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip soundToPlay;
    [Range(0f, 1f)] public float volume = 1f;
    public bool playOnce = true;

    private bool hasPlayed = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasPlayed || !playOnce)
        {
            PlaySound();
            hasPlayed = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPlayed || !playOnce)
        {
            PlaySound();
            hasPlayed = true;
        }
    }

    private void PlaySound()
    {
        if (soundToPlay != null)
        {
            audioSource.clip = soundToPlay;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
}
