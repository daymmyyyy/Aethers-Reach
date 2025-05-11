using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused = false; 

    void Update()
    {
        // Toggle pause when the player presses the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

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
}
