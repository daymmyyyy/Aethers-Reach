using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused = false;

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);

        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.musicSource.clip != null)
                AudioManager.Instance.musicSource.UnPause();
            if (AudioManager.Instance.sfxSource.clip != null)
                AudioManager.Instance.sfxSource.UnPause();
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.musicSource.clip != null)
                AudioManager.Instance.musicSource.Pause();
            if (AudioManager.Instance.sfxSource.clip != null)
                AudioManager.Instance.sfxSource.Pause();
        }

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        if (RelicManager.Instance != null)
            RelicManager.Instance.ResetSessionRelics();

        RelicCurrency.ResetCurrency();

        if (AudioManager.Instance != null)
        {
            // Stop all scene audio and apply saved volume to AudioManager
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.sfxSource.Stop();
        }

        SceneManager.LoadScene("MainMenu");
    }
}
