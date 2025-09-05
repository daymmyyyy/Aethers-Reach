using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("UI")]
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

    [Header("Scene Music Clips")]
    public AudioClip menuMusic;
    public AudioClip biome1Music;
    public AudioClip biome2Music;
    public AudioClip biome3Music;

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

        // Automatically unmute if volume > 0
        if (musicVolume > 0f) musicMuted = false;
        if (sfxVolume > 0f) sfxMuted = false;

        ApplyVolumes();

        SceneManager.sceneLoaded += OnSceneLoaded;

        AttachSliders();
    }


    private void AttachSliders()
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);
        RefreshUI();
    }

    public void PlaySceneMusic(string sceneName)
    {
        AudioClip clip = null;

        if (sceneName == "MainMenu") clip = menuMusic;
        else if (sceneName == "Biome1") clip = biome1Music;
        else if (sceneName == "Biome2") clip = biome2Music;
        else if (sceneName == "Biome3") clip = biome3Music;

        if (clip != null)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.volume = GetMusicVolume();
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }

    #region Public API
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = GetMusicVolume();
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, GetSFXVolume());
    }

    public void ToggleSFXMute()
    {
        sfxMuted = !sfxMuted;
        PlayerPrefs.SetInt("SFX_MUTED", sfxMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolumes();
        RefreshUI();
    }

    public void ToggleMusicMute()
    {
        musicMuted = !musicMuted;
        PlayerPrefs.SetInt("MUSIC_MUTED", musicMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolumes();
        RefreshUI();
    }

    private void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SFX_VOLUME", sfxVolume);
        PlayerPrefs.Save();

        // Automatically unmute if slider > 0
        if (sfxVolume > 0f && sfxMuted) sfxMuted = false;

        ApplyVolumes();
        UpdateIcons();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("MUSIC_VOLUME", musicVolume);
        PlayerPrefs.Save();

        // Automatically unmute if slider > 0
        if (musicVolume > 0f && musicMuted) musicMuted = false;

        ApplyVolumes();
        UpdateIcons();
    }

    public float GetMusicVolume() => musicMuted ? 0f : musicVolume;
    public float GetSFXVolume() => sfxMuted ? 0f : sfxVolume;
    #endregion

    #region Helpers
    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = GetMusicVolume();
        if (sfxSource != null)
            sfxSource.volume = GetSFXVolume();
    }

    public void RefreshUI()
    {
        if (musicSlider != null) musicSlider.value = GetMusicVolume();
        if (sfxSlider != null) sfxSlider.value = GetSFXVolume();
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        if (musicIcon != null)
            musicIcon.sprite = GetMusicVolume() <= 0.01f ? musicMuteIcon : musicUnmuteIcon;

        if (sfxIcon != null)
            sfxIcon.sprite = GetSFXVolume() <= 0.01f ? sfxMuteIcon : sfxUnmuteIcon;
    }

    public void AttachSliders(Slider musicSlider, Slider sfxSlider)
    {
        // Remove old listeners
        if (this.musicSlider != null) this.musicSlider.onValueChanged.RemoveAllListeners();
        if (this.sfxSlider != null) this.sfxSlider.onValueChanged.RemoveAllListeners();

        // Assign new references
        this.musicSlider = musicSlider;
        this.sfxSlider = sfxSlider;

        // Add listeners
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolumeFromSlider);
            musicSlider.value = GetMusicVolume(); // sync value
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolumeFromSlider);
            sfxSlider.value = GetSFXVolume(); // sync value
        }

        UpdateIcons();
    }

    private void SetMusicVolumeFromSlider(float value) => SetMusicVolume(value);
    private void SetSFXVolumeFromSlider(float value) => SetSFXVolume(value);
    #endregion
}
