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

        if (biome1Text != null)
            biome1Text.text = $"Biome 1 High Score: {GameManager.Instance.biome1HighScore:F2} km";

        if (biome2Text != null)
            biome2Text.text = $"Biome 2 High Score: {GameManager.Instance.biome2HighScore:F2} km";

        if (biome3Text != null)
            biome3Text.text = $"Biome 3 High Score: {GameManager.Instance.biome3HighScore:F2} km";

        if (totalHighScoreText != null)
            totalHighScoreText.text = $"Total High Score: {GameManager.Instance.highScore:F2} km";
    }
}
