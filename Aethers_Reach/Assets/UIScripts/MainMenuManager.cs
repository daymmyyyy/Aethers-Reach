using UnityEngine;
using UnityEngine.SceneManagement;  // important!

public class MainMenuManager : MonoBehaviour
{
    public AudioClip menuBGM;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(menuBGM);
        }
    }

    public void StartGame()
    {

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetProgress();
            GameManager.Instance.cameFromMainMenu = true;
        }

        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ResetSessionRelics();
        }

        RelicCurrency.ResetCurrency();  // session-based

        // Force reset PlayerPrefs for session relics
        PlayerPrefs.SetInt("RelicsThisSession", 0);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("Biome1");
    }

    public void MainMenu()
    {
        if (AudioManager.Instance != null)
        {
            // Stop any currently playing scene music/SFX
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.sfxSource.Stop();

            // Play menu BGM
            AudioManager.Instance.PlayMusic(menuBGM);
        }

        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ResetSessionRelics();
        }

        RelicCurrency.ResetCurrency();  // session-based

        SceneManager.LoadScene("MainMenu");
    }
}
