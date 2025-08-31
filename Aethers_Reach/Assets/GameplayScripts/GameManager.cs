using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scores")]
    public float highScore;           // Highest single run ever
    public float sessionDistance;     // Total distance in current run

    [Header("Per-Biome Distance")]
    public float biome1Distance;
    public float biome2Distance;
    public float biome3Distance;

    [Header("Biome High Scores")]
    public float biome1HighScore;
    public float biome2HighScore;
    public float biome3HighScore;

    public float lastRunDistance;
    public string previousScene;
    public bool cameFromMainMenu = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load stored high scores
            highScore = PlayerPrefs.GetFloat("HighScore", 0f);
            biome1HighScore = PlayerPrefs.GetFloat("Biome1HighScore", 0f);
            biome2HighScore = PlayerPrefs.GetFloat("Biome2HighScore", 0f);
            biome3HighScore = PlayerPrefs.GetFloat("Biome3HighScore", 0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveProgressBeforeSceneChange(float currentBiomeDistance)
    {
        sessionDistance += currentBiomeDistance;

        string currentScene = SceneManager.GetActiveScene().name;

        // Save per-biome distance separately
        switch (currentScene)
        {
            case "Biome1":
                biome1Distance += currentBiomeDistance - biome2Distance - biome3Distance;
                biome1HighScore = Mathf.Max(biome1HighScore, biome1Distance);
                PlayerPrefs.SetFloat("Biome1HighScore", biome1HighScore);
                break;
            case "Biome2":
                biome2Distance += currentBiomeDistance - biome1Distance;
                biome2HighScore = Mathf.Max(biome2HighScore, biome2Distance);
                PlayerPrefs.SetFloat("Biome2HighScore", biome2HighScore);
                break;
            case "Biome3":
                biome3Distance += currentBiomeDistance - biome2Distance - biome1Distance;
                biome3HighScore = Mathf.Max(biome3HighScore, biome3Distance);
                PlayerPrefs.SetFloat("Biome3HighScore", biome3HighScore);
                break;
        }

        PlayerPrefs.Save();
    }

    public void OnGameOver()
    {
        lastRunDistance = sessionDistance;

        // Save high score if beaten
        if (sessionDistance > highScore)
        {
            highScore = sessionDistance;
            PlayerPrefs.SetFloat("HighScore", highScore);
        }

        // Reset session and per-biome distances for next run
        sessionDistance = 0f;
        biome1Distance = 0f;
        biome2Distance = 0f;
        biome3Distance = 0f;

        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        sessionDistance = 0f;
        biome1Distance = 0f;
        biome2Distance = 0f;
        biome3Distance = 0f;
        cameFromMainMenu = true;
    }

    public void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        cameFromMainMenu = currentScene == "MainMenu";

        previousScene = currentScene;
        SceneManager.LoadScene(sceneName);
    }
}
