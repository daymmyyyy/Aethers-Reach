using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    private float lastMusicVolume = 1f;
    private float lastSFXVolume = 1f;

    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved volumes & mute states
            lastMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            lastSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            isSFXMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

            ApplyMusicVolume();
            ApplySFXVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float volume)
    {
        lastMusicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
        ApplyMusicVolume();
    }

    public void ToggleMuteMusic()
    {
        isMusicMuted = !isMusicMuted;
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMusicVolume();
    }

    public float GetMusicVolume() => isMusicMuted ? 0f : lastMusicVolume;

    private void ApplyMusicVolume()
    {
        musicSource.volume = isMusicMuted ? 0f : lastMusicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        lastSFXVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        ApplySFXVolume();
    }

    public void ToggleMuteSFX()
    {
        isSFXMuted = !isSFXMuted;
        PlayerPrefs.SetInt("SFXMuted", isSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplySFXVolume();
    }

    public float GetSFXVolume() => isSFXMuted ? 0f : lastSFXVolume;

    private void ApplySFXVolume()
    {
        sfxSource.volume = isSFXMuted ? 0f : lastSFXVolume;
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
}
