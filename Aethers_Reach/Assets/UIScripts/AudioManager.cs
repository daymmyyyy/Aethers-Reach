using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("UI (optional)")]
    public Slider musicSlider;
    public Image musicIcon;
    public Sprite musicMuteIcon;
    public Sprite musicUnmuteIcon;

    public Slider sfxSlider;
    public Image sfxIcon;
    public Sprite sfxMuteIcon;
    public Sprite sfxUnmuteIcon;

    [Header("Defaults")]
    [Range(0f, 1f)] public float defaultMusicVolume = 1f;
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;

    private float musicVolume;
    private float sfxVolume;
    private bool musicMuted = false;
    private bool sfxMuted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved preferences
        musicVolume = PlayerPrefs.GetFloat("MUSIC_VOLUME", defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", defaultSFXVolume);
        musicMuted = PlayerPrefs.GetInt("MUSIC_MUTED", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SFX_MUTED", 0) == 1;

        ApplyVolumes();
        UpdateIcons();
    }

    private void OnEnable()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolumeFromSlider);
            musicSlider.value = GetMusicVolume();
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolumeFromSlider);
            sfxSlider.value = GetSFXVolume();
        }

        UpdateIcons();
    }

    #region Public API

    // Play music
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = GetMusicVolume();
        musicSource.Play();
    }

    // Play SFX with current volume
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, GetSFXVolume());
    }

    public void ToggleMusicMute()
    {
        musicMuted = !musicMuted;
        PlayerPrefs.SetInt("MUSIC_MUTED", musicMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolumes();
        UpdateIcons();
        if (musicSlider != null) musicSlider.value = GetMusicVolume();
    }

    public void ToggleSFXMute()
    {
        sfxMuted = !sfxMuted;
        PlayerPrefs.SetInt("SFX_MUTED", sfxMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolumes();
        UpdateIcons();
        if (sfxSlider != null) sfxSlider.value = GetSFXVolume();
    }

    // Set volume from slider
    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("MUSIC_VOLUME", musicVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
        UpdateIcons();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SFX_VOLUME", sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
        UpdateIcons();
    }

    // Get effective volume
    public float GetMusicVolume() => musicMuted ? 0f : musicVolume;
    public float GetSFXVolume() => sfxMuted ? 0f : sfxVolume;

    #endregion

    #region Private Helpers

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = GetMusicVolume();
        if (sfxSource != null)
            sfxSource.volume = GetSFXVolume(); // ensures PlayOneShot uses correct volume
    }

    private void UpdateIcons()
    {
        if (musicIcon != null)
            musicIcon.sprite = GetMusicVolume() <= 0.01f ? musicMuteIcon : musicUnmuteIcon;

        if (sfxIcon != null)
            sfxIcon.sprite = GetSFXVolume() <= 0.01f ? sfxMuteIcon : sfxUnmuteIcon;
    }

    private void SetMusicVolumeFromSlider(float value) => SetMusicVolume(value);
    private void SetSFXVolumeFromSlider(float value) => SetSFXVolume(value);

    #endregion
}
