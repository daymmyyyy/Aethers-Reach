using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Global Scores")]
    public float totalDistanceTravelled;
    public float highScore; // overall best single run
    public float sessionDistance;

    [Header("Biome High Scores")]
    public float biome1HighScore;
    public float biome2HighScore;
    public float biome3HighScore;

    public float lastRunDistance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load global + biome high scores
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

    public void SaveProgressBeforeSceneChange(float currentSessionDistanceInKm)
    {
        lastRunDistance = currentSessionDistanceInKm;
        totalDistanceTravelled += currentSessionDistanceInKm;

        // Update global high score
        if (currentSessionDistanceInKm > highScore)
        {
            highScore = currentSessionDistanceInKm;
            PlayerPrefs.SetFloat("HighScore", highScore);
        }

        // Update biome high score
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "Biome1":
                biome1HighScore += currentSessionDistanceInKm;
                PlayerPrefs.SetFloat("Biome1HighScore", biome1HighScore);
                break;

            case "Biome2":
                biome2HighScore += currentSessionDistanceInKm;
                PlayerPrefs.SetFloat("Biome2HighScore", biome2HighScore);
                break;

            case "Biome3":
                biome3HighScore += currentSessionDistanceInKm;
                PlayerPrefs.SetFloat("Biome3HighScore", biome3HighScore);
                break;
        }

        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        totalDistanceTravelled = 0f;
        sessionDistance = 0f;
    }
}
