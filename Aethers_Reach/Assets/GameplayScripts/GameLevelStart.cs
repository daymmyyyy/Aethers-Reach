using System.Collections;
using UnityEngine;

public class GameLevelStart : MonoBehaviour
{
    [Header("References")]
    public GameObject topLimit;
    public GameObject UI;

    [Header("Timing")]
    public float delayBeforeFade = 3f;
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        if (topLimit == null)
        {
            GameObject found = GameObject.Find("TopLimit");
            if (found != null) topLimit = found;
        }
    }

    private void Start()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        if (UI != null)
            yield return StartCoroutine(FadeOutAndDisable());

        if (topLimit != null)
            topLimit.SetActive(true);
    }

    private IEnumerator FadeOutAndDisable()
    {
        CanvasGroup canvasGroup = UI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = UI.AddComponent<CanvasGroup>();

        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        UI.SetActive(false);
    }
}
