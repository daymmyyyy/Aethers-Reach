using UnityEngine;
using UnityEngine.SceneManagement;  // important!

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetProgress();
        }
        SceneManager.LoadScene("Biome1");
    }

    public void MainMenu()
    {
        PlayerPrefs.DeleteKey("RelicsCollected");
        SceneManager.LoadScene("MainMenu");
    }

    public void HighScore()
    {

        SceneManager.LoadScene("HighScore");
    }

    public void Quit()
    {
        Debug.Log("Quit button pressed");
        Application.Quit();
    }
}
