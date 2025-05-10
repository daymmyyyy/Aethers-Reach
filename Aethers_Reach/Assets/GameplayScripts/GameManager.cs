using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float distanceTravelled;
    public float highScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load high score if stored previously
            highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveScore(float distance)
    {
        distanceTravelled = distance;

        if (distance > highScore)
        {
            highScore = distance;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
}
