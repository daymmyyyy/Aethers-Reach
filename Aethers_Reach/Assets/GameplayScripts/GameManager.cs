using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scores")]
    public float highScore;           // Highest single run ever
    public float sessionDistance;     // Total distance in current run

    [Header("Per-Biome Distance (this run)")]
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

    // Internal biome start markers
    private float biome1Start;
    private float biome2Start;
    private float biome3Start;

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
        // Total session distance so far (always update to current run distance)
        sessionDistance = currentBiomeDistance;

        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "Biome1":
                if (biome1Start == 0f) biome1Start = currentBiomeDistance;
                biome1Distance = currentBiomeDistance - biome1Start;
                biome1HighScore = Mathf.Max(biome1HighScore, biome1Distance);
                PlayerPrefs.SetFloat("Biome1HighScore", biome1HighScore);
                break;

            case "Biome2":
                if (biome2Start == 0f) biome2Start = currentBiomeDistance;
                biome2Distance = currentBiomeDistance - biome2Start;
                biome2HighScore = Mathf.Max(biome2HighScore, biome2Distance);
                PlayerPrefs.SetFloat("Biome2HighScore", biome2HighScore);
                break;

            case "Biome3":
                if (biome3Start == 0f) biome3Start = currentBiomeDistance;
                biome3Distance = currentBiomeDistance - biome3Start;
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

        // Reset everything for the next run
        sessionDistance = 0f;
        biome1Distance = 0f;
        biome2Distance = 0f;
        biome3Distance = 0f;
        biome1Start = 0f;
        biome2Start = 0f;
        biome3Start = 0f;

        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        sessionDistance = 0f;
        biome1Distance = 0f;
        biome2Distance = 0f;
        biome3Distance = 0f;
        biome1Start = 0f;
        biome2Start = 0f;
        biome3Start = 0f;
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
ore