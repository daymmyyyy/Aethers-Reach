using UnityEngine;
using UnityEngine.UI;

public class SettingsOpen : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Image musicIcon;
    public Image sfxIcon;

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            // Attach sliders and icons
            AudioManager.Instance.AttachSliders(musicSlider, sfxSlider);
            AudioManager.Instance.musicIcon = musicIcon;
            AudioManager.Instance.sfxIcon = sfxIcon;

            AudioManager.Instance.RefreshUI();
        }
    }
}
