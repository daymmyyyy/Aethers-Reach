using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    [Header("UI References")]
    public Text biome1Text;
    public Text biome2Text;
    public Text biome3Text;
    public Text totalHighScoreText;

    void Start()
    {
        if (GameManager.Instance == null) return;

        float bestMeters = GameManager.Instance.highScore;

        if (biome1Text != null)
            biome1Text.text = $"Skylands: {GameManager.Instance.biome1HighScore:F2}m";

        if (biome2Text != null)
            biome2Text.text = $"Beach: {GameManager.Instance.biome2HighScore:F2}m";

        if (biome3Text != null)
            biome3Text.text = $"Ruins: {GameManager.Instance.biome3HighScore:F2}m";

        if (totalHighScoreText != null)

            totalHighScoreText.text = "Best Single Run: " + bestMeters.ToString("F2") + " m";
    }
}
