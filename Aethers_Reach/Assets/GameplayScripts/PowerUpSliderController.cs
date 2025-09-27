using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpSliderController : MonoBehaviour
{
    public static PowerUpSliderController Instance; // Singleton reference

    private Slider slider;
    private CanvasGroup canvasGroup; // for hiding the slider
    private Coroutine activeTimer;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("PowerUpSliderController requires a Slider component!");
            return;
        }

        // Add CanvasGroup if not present
        canvasGroup = slider.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = slider.gameObject.AddComponent<CanvasGroup>();

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Hide visually but keep active so we can find by tag
        canvasGroup.alpha = 0f;
    }

    public void StartTimer(float duration)
    {
        if (activeTimer != null)
            StopCoroutine(activeTimer);

        activeTimer = StartCoroutine(RunTimer(duration));
    }

    private IEnumerator RunTimer(float duration)
    {
        slider.value = 1f;
        canvasGroup.alpha = 1f; // show slider

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = 1f - Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        slider.value = 0f;
        canvasGroup.alpha = 0f; // hide slider again
        activeTimer = null;
    }
}
