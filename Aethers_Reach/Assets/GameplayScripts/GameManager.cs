using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float totalDistanceTravelled;
    public float highScore;
    public float sessionDistance;


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

    public float lastRunDistance;

    public void SaveProgressBeforeSceneChange(float currentSessionDistanceInKm)
    {
        lastRunDistance = currentSessionDistanceInKm;
        totalDistanceTravelled += currentSessionDistanceInKm;

        if (currentSessionDistanceInKm > highScore)
        {
            highScore = currentSessionDistanceInKm;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }


    public void ResetProgress()
    {
        totalDistanceTravelled = 0f;
        sessionDistance = 0f;
    }


}
