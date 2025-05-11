using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float totalDistanceTravelled;
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

    public void SaveProgressBeforeSceneChange(float currentSessionDistance)
    {
        totalDistanceTravelled += currentSessionDistance;

        if (currentSessionDistance > highScore)
        {
            highScore = currentSessionDistance;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void ResetProgress()
    {
        totalDistanceTravelled = 0f;
    }

}
