using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("SFX UI")]
    public Slider sfxSlider;
    public Button sfxMuteButton;
    public Sprite sfxMuteIcon;
    public Sprite sfxUnmuteIcon;

    [Header("Music UI")]
    public Slider musicSlider;
    public Button musicMuteButton;
    public Sprite musicMuteIcon;
    public Sprite musicUnmuteIcon;

    private bool isSFXMuted = false;
    private bool isMusicMuted = false;

    void Start()
    {
        // Init sliders
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        musicSlider.value = AudioManager.Instance.GetMusicVolume();

        // Listeners
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        sfxMuteButton.onClick.AddListener(ToggleSFXMute);
        musicMuteButton.onClick.AddListener(ToggleMusicMute);

        UpdateSFXIcon();
        UpdateMusicIcon();
    }

    // SFX
    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        isSFXMuted = value <= 0.01f;
        UpdateSFXIcon();
    }

    private void ToggleSFXMute()
    {
        AudioManager.Instance.ToggleMuteSFX();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume(); // sync
        isSFXMuted = sfxSlider.value <= 0.01f;
        UpdateSFXIcon();
    }

    private void UpdateSFXIcon()
    {
        sfxMuteButton.image.sprite = isSFXMuted ? sfxMuteIcon : sfxUnmuteIcon;
    }

    // Music
    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        isMusicMuted = value <= 0.01f;
        UpdateMusicIcon();
    }

    private void ToggleMusicMute()
    {
        AudioManager.Instance.ToggleMuteMusic();
        musicSlider.value = AudioManager.Instance.GetMusicVolume(); // sync
        isMusicMuted = musicSlider.value <= 0.01f;
        UpdateMusicIcon();
    }

    private void UpdateMusicIcon()
    {
        musicMuteButton.image.sprite = isMusicMuted ? musicMuteIcon : musicUnmuteIcon;
    }
}
