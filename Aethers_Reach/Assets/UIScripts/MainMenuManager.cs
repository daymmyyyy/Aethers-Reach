using UnityEngine;
using UnityEngine.SceneManagement;  // important!

public class MainMenuManager : MonoBehaviour
{
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

        BiomeManager.Instance.SetCurrentBiome(0);
        DiaryManager.Instance.OnBiomeEntered(0);
        SceneManager.LoadScene("Biome1");
    }

    public void MainMenu()
    {
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ResetSessionRelics();
        }

        RelicCurrency.ResetCurrency();  // session-based
        BiomeManager.Instance.SetCurrentBiome(-1);
        SceneManager.LoadScene("MainMenu");
    }
}
