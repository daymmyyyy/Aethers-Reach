using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused = false;

    [Header("Cloud Transition")]

    public CloudTransitionController cloudTransition;
    public GameObject cloudCanvas;

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
        cloudCanvas.SetActive(true);
        StartCoroutine(MainMenuRoutine());
    }

    private IEnumerator MainMenuRoutine()
    {
        // Keep game paused while animation plays
        pauseMenuUI.SetActive(false);

        // Play clouds animation and wait
        if (cloudTransition != null)
            yield return cloudTransition.PlayCloudsInAndWait();

        // Now safely resume game and load Main Menu
        Time.timeScale = 1f;

        if (RelicManager.Instance != null)
            RelicManager.Instance.ResetSessionRelics();

        RelicCurrency.ResetCurrency();

        SceneManager.LoadScene("MainMenu");
    }


}
