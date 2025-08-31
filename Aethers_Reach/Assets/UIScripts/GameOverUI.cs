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

        float finalKm = finalMeters * 0.001f;
        float bestKm = bestMeters * 0.001f;

        if (distanceText != null)
            distanceText.text = "Distance: " + finalKm.ToString("F2") + " km";

        if (highScoreText != null)
            highScoreText.text = "Total High Score: " + bestKm.ToString("F2") + " km";

        int currencyCollected = RelicCurrency.GetSessionCurrency();
        if (currencyText != null)
            currencyText.text = $"{currencyCollected}";
    }

}
