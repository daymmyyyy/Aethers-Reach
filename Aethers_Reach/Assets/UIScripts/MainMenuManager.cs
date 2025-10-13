using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Transition Settings")]
    public Animator cloudAnimator; // Assign in Inspector
    [SerializeField] private string cloudsInTrigger = "CloudsIn";
    [SerializeField] private string cloudsOutTrigger = "CloudsOut";

    private static bool hasPlayedCloudOut = false;

    private void Start()
    {
        // When first entering main menu, play clouds out once
        if (!hasPlayedCloudOut && cloudAnimator != null)
        {
            cloudAnimator.SetTrigger(cloudsOutTrigger);
            hasPlayedCloudOut = true;
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
        if (cloudAnimator != null)
        {
            StartCoroutine(CloudsInThenLoadMainMenu());
        }
        else
        {
            LoadMainMenuImmediate();
        }
    }

    private IEnumerator CloudsInThenLoadMainMenu()
    {
        cloudAnimator.SetTrigger(cloudsInTrigger);

        // Wait until animation finishes
        yield return new WaitUntil(() =>
            cloudAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
            !cloudAnimator.IsInTransition(0));

        LoadMainMenuImmediate();
    }

    private void LoadMainMenuImmediate()
    {
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ResetSessionRelics();
        }

        RelicCurrency.ResetCurrency();
        BiomeManager.Instance.SetCurrentBiome(-1);

        SceneManager.LoadScene("MainMenu");
    }
}
