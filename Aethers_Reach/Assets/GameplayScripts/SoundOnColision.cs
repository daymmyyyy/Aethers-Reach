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
    private void OnDisable()
    {
        StopSound();
    }

    private void OnDestroy()
    {
        StopSound();
    }

    private void StopSound()
    {
        if (AudioManager.Instance != null && soundToPlay != null)
        {
            AudioManager.Instance.sfxSource.Stop();
        }
    }

}
