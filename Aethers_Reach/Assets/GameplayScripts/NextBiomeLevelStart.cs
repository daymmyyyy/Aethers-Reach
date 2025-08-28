using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NextBiomeLevelStart : MonoBehaviour
{
    public Text instructionText;
    public float fadeDuration = 1f;
    public float displayTime = 2f;
    public float initialDelay = 1f;
    public GameObject topLimit;

    private void Start()
    {
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        // Initial delay
        yield return new WaitForSeconds(initialDelay);

        // Fade in text
        instructionText.gameObject.SetActive(true);

        // Activate top limit as text starts fading in
        if (topLimit != null)
            topLimit.SetActive(true);

        yield return StartCoroutine(FadeUI(instructionText, 0f, 1f, fadeDuration));

        // Display for a while
        yield return new WaitForSeconds(displayTime);

        // Fade out text
        yield return StartCoroutine(FadeUI(instructionText, 1f, 0f, fadeDuration));
    }


    private IEnumerator FadeUI(Graphic uiElement, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color originalColor = uiElement.color;
        uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, startAlpha);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(startAlpha, endAlpha, t));
            yield return null;
        }

        uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);

        if (endAlpha == 0f)
            uiElement.gameObject.SetActive(false);
    }
}
