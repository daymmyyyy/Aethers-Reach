using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private float lastMusicVolume = 1f;
    private float lastSFXVolume = 1f;

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

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume * sfxSource.volume);
    }

    // Music
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        if (volume > 0f) lastMusicVolume = volume;
    }

    public void ToggleMuteMusic()
    {
        if (musicSource.volume > 0f)
            musicSource.volume = 0f;
        else
            musicSource.volume = lastMusicVolume;
    }

    public float GetMusicVolume() => musicSource.volume;

    // SFX
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        if (volume > 0f) lastSFXVolume = volume;
    }

    public void ToggleMuteSFX()
    {
        if (sfxSource.volume > 0f)
            sfxSource.volume = 0f;
        else
            sfxSource.volume = lastSFXVolume;
    }

    public float GetSFXVolume() => sfxSource.volume;
}
