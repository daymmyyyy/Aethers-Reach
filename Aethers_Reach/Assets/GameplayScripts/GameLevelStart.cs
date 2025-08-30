using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLevelStart : MonoBehaviour
{
    public GameObject topLimit;
    public GameObject UI;

    public Text firstInstruction;
    public Text secondInstruction;
    public Image secondInstructionSprite; // relic icon
    public Text biomeNameText; // assign biome name text in inspector

    public float fadeDuration = 1f;
    public float displayTime = 2f;

    private string biomeKey; // unique key for this biome

    private void Start()
    {
        biomeKey = "Entered_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        firstInstruction.gameObject.SetActive(false);
        secondInstruction.gameObject.SetActive(false);
        secondInstructionSprite.gameObject.SetActive(false);
        biomeNameText.gameObject.SetActive(false);

        if (topLimit != null)
            topLimit.SetActive(false);

        bool showInstructions = false;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.cameFromMainMenu)
            {
                showInstructions = true; // always show if coming from Main Menu
            }
            else if (!PlayerPrefs.HasKey(biomeKey))
            {
                showInstructions = true; // first time in biome
            }

            // mark biome as visited if showing instructions
            if (showInstructions)
            {
                PlayerPrefs.SetInt(biomeKey, 1);
                PlayerPrefs.Save();
            }
        }

        if (showInstructions)
            StartCoroutine(StartSequenceWithInstructions());
        else
            StartCoroutine(StartSequenceBiomeNameOnly());
    }


    private IEnumerator StartSequenceWithInstructions()
    {
        yield return new WaitForSeconds(1f); // delay

        // Fade in first text
        firstInstruction.gameObject.SetActive(true);
        yield return StartCoroutine(FadeUI(firstInstruction, 0f, 1f, fadeDuration));

        // Activate top limit
        if (topLimit != null)
            topLimit.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        // Fade out first text
        yield return StartCoroutine(FadeUI(firstInstruction, 1f, 0f, fadeDuration));

        // Show second text + sprite
        secondInstruction.text = "To unlock portals, collect 5";
        secondInstruction.gameObject.SetActive(true);
        secondInstructionSprite.gameObject.SetActive(true);

        // Fade both together
        yield return StartCoroutine(FadeMultipleUI(new Graphic[] { secondInstruction, secondInstructionSprite }, 0f, 1f, fadeDuration));

        yield return new WaitForSeconds(displayTime);

        // Fade both out together
        yield return StartCoroutine(FadeMultipleUI(new Graphic[] { secondInstruction, secondInstructionSprite }, 1f, 0f, fadeDuration));
    }

    private IEnumerator StartSequenceBiomeNameOnly()
    {
        yield return new WaitForSeconds(1f); // delay

        // Fade in first text
        biomeNameText.gameObject.SetActive(true);
        yield return StartCoroutine(FadeUI(biomeNameText, 0f, 1f, fadeDuration));

        // Activate top limit
        if (topLimit != null)
            topLimit.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        yield return StartCoroutine(FadeUI(biomeNameText, 1f, 0f, fadeDuration));
    }

    private IEnumerator FadeMultipleUI(Graphic[] uiElements, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color[] originalColors = new Color[uiElements.Length];

        for (int i = 0; i < uiElements.Length; i++)
        {
            originalColors[i] = uiElements[i].color;
            uiElements[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, startAlpha);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < uiElements.Length; i++)
            {
                uiElements[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b,
                    Mathf.Lerp(startAlpha, endAlpha, t));
            }
            yield return null;
        }

        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, endAlpha);
            if (endAlpha == 0f)
                uiElements[i].gameObject.SetActive(false);
        }
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
