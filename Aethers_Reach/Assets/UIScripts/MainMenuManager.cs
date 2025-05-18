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

        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ResetSessionRelics();
        }

        RelicCurrency.ResetCurrency();  // session-based

        // Force reset PlayerPrefs
        PlayerPrefs.SetInt("RelicsThisSession", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Biome1");
    }


    public void MainMenu()
    {
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

    public void Shop()
    {

        SceneManager.LoadScene("Shop");
    }

    public void DiaryEntries()
    {

        SceneManager.LoadScene("DiaryEntries");
    }

    public void Quit()
    {
        Debug.Log("Quit button pressed");
        Application.Quit();
    }
}
