using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused = false; 

    public void ResumeGame()
    {
        // Hide the pause menu
        pauseMenuUI.SetActive(false);

        // Resume game time and physics
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        // Show the pause menu
        pauseMenuUI.SetActive(true);

        // Pause game time and physics
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;

        if (RelicManager.Instance != null)
            RelicManager.Instance.ResetSessionRelics();

        RelicCurrency.ResetCurrency();

        SceneManager.LoadScene("MainMenu");
    }
}
