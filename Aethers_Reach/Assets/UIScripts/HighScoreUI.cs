using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    public Text highScoreText;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            highScoreText.text = "High Score: " + GameManager.Instance.highScore.ToString("F2") + " km";
        }
    }
}
