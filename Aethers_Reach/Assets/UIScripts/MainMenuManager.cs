using UnityEngine;
using UnityEngine.SceneManagement;  // important!

public class MainMenuManager : MonoBehaviour
{
    public AudioClip menuBGM;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(menuBGM);
    }
    public void StartGame()
    {
        AudioManager.Instance.musicSource.Stop(); // stop Main Menu BGM

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetProgress();
            GameManager.Instance.cameFromMainMenu = true; // ✅ Tell GameManager we are coming from Main Menu
        }

        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ResetSessionRelics();
        }

        RelicCurrency.ResetCurrency();  // session-based

        // Force reset PlayerPrefs
        PlayerPrefs.SetInt("RelicsThisSession", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Biome1");
        Time.timeScale = 1f;
    }


    public void MainMenu()
    {
        AudioManager.Instance.PlayMusic(menuBGM);

        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ResetSessionRelics();

        }

        RelicCurrency.ResetCurrency();  //session-based
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
