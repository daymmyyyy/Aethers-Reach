using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public Text distanceText;
    public Text highScoreText;
    public Text currencyText;


    private IEnumerator Start()
    {
        while (GameManager.Instance == null) { yield return null; }

        float finalMeters = GameManager.Instance.lastRunDistance;
        float bestMeters = GameManager.Instance.highScore;

        if (distanceText != null)
            distanceText.text = "Distance: " + finalMeters.ToString("F2") + " m";

        if (highScoreText != null)
            highScoreText.text = "Total High Score: " + bestMeters.ToString("F2") + " m";

        int currencyCollected = RelicCurrency.GetSessionCurrency();
        if (currencyText != null)
            currencyText.text = $"{currencyCollected}";
    }

}
