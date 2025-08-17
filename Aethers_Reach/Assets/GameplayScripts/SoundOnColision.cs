using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip soundToPlay;
    [Range(0f, 1f)] public float volume = 1f;
    public bool playOnce = true;

    private bool hasPlayed = false;

    void Start()
    {
        // If this sound is intended as background music
        if (soundToPlay != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.musicSource.volume = volume;
            AudioManager.Instance.PlayMusic(soundToPlay);
        }
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
        if (soundToPlay != null && AudioManager.Instance != null)
        {
            // Temporarily set the volume
            AudioManager.Instance.sfxSource.volume = volume;
            AudioManager.Instance.PlaySFX(soundToPlay);

            // Restore
            AudioManager.Instance.sfxSource.volume = 0.3f;
        }
    }
}
