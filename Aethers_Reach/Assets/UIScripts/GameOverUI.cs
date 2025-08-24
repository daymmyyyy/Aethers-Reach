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
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        float finalDistance = GameManager.Instance.lastRunDistance;
        float highScore = GameManager.Instance.highScore;

        if (distanceText != null)
            distanceText.text = "Distance: " + finalDistance.ToString("F2") + " km";

        if (highScoreText != null)
            highScoreText.text = "Total High Score: " + highScore.ToString("F2") + " km";

        int currencyCollected = RelicCurrency.GetSessionCurrency();
        if (currencyText != null)
            currencyText.text = $"Relic Currency: {currencyCollected}";

    }


}
