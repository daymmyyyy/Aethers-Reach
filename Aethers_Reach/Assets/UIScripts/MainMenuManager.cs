using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Cloud Transition")]
    public CloudTransitionController cloudTransition;

    private void Start()
    {
        // Only play CloudsOut automatically if we're in MainMenu
        if (SceneManager.GetActiveScene().name == "MainMenu" &&
            cloudTransition != null)
        {
            cloudTransition.PlayCloudsOut();
        }
        else if (SceneManager.GetActiveScene().name != "MainMenu" && cloudTransition != null)
        {
            // Disable clouds on Game Over scene at start
            cloudTransition.cloudObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        if (cloudTransition != null)
            yield return cloudTransition.PlayCloudsInAndWait(); // wait for CloudsIn animation

        // Then do the rest
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetProgress();
            GameManager.Instance.cameFromMainMenu = true;
        }

        if (RelicManager.Instance != null)
            RelicManager.Instance.ResetSessionRelics();

        RelicCurrency.ResetCurrency();
        PlayerPrefs.SetInt("RelicsThisSession", 0);
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        BiomeManager.Instance.SetCurrentBiome(0);
        DiaryManager.Instance.OnBiomeEntered(0);

        SceneManager.LoadScene("Biome1");
    }

    public void MainMenu()
    {
        StartCoroutine(MainMenuRoutine());
    }

    private IEnumerator MainMenuRoutine()
    {
        if (cloudTransition != null)
            yield return cloudTransition.PlayCloudsInAndWait(); // wait for CloudsIn animation

        if (RelicManager.Instance != null)
            RelicManager.Instance.ResetSessionRelics();

        RelicCurrency.ResetCurrency();
        BiomeManager.Instance.SetCurrentBiome(-1);

        SceneManager.LoadScene("MainMenu");
    }

}
