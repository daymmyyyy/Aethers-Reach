using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Global Scores")]
    public float totalDistanceTravelled; // sum of all runs
    public float highScore; // best single run across ALL biomes
    public float sessionDistance;

    [Header("Biome High Scores (best single run per biome)")]
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

            // Load stored scores
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
        // Add to total distance across all sessions
        totalDistanceTravelled += currentBiomeDistance;

        string currentScene = SceneManager.GetActiveScene().name;

        // Update per-biome high score
        switch (currentScene)
        {
            case "Biome1":
                biome1HighScore = Mathf.Max(biome1HighScore, currentBiomeDistance);
                PlayerPrefs.SetFloat("Biome1HighScore", biome1HighScore);
                break;
            case "Biome2":
                biome2HighScore = Mathf.Max(biome2HighScore, currentBiomeDistance);
                PlayerPrefs.SetFloat("Biome2HighScore", biome2HighScore);
                break;
            case "Biome3":
                biome3HighScore = Mathf.Max(biome3HighScore, currentBiomeDistance);
                PlayerPrefs.SetFloat("Biome3HighScore", biome3HighScore);
                break;
        }

        // Update global high score = longest distance in ONE run
        if (currentBiomeDistance > highScore)
        {
            highScore = currentBiomeDistance;
            PlayerPrefs.SetFloat("HighScore", highScore);
        }

        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        totalDistanceTravelled = 0f;
        sessionDistance = 0f;
    }

    public void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        cameFromMainMenu = currentScene == "MainMenu"; // set true if loading from MainMenu

        previousScene = currentScene;
        SceneManager.LoadScene(sceneName);
    }

}
