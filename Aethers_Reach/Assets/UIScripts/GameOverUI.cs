using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public Text distanceText;
    public Text highScoreText;

    private IEnumerator Start()
    {
        // Wait until GameManager.Instance is available and valid
        while (GameManager.Instance == null || GameManager.Instance.distanceTravelled == 0f)
        {
            yield return null;
        }

        float finalDistance = GameManager.Instance.distanceTravelled;
        float highScore = GameManager.Instance.highScore;

        if (distanceText != null)
            distanceText.text = "Distance: " + finalDistance.ToString("F2") + " km";

        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore.ToString("F2") + " km";
    }
}
