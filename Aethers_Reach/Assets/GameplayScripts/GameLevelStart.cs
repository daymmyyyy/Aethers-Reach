using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelStart : MonoBehaviour
{
    public GameObject topLimit;
    public GameObject instructionsUI;
    private float fadeDuration = 1.5f;

    private bool alreadyTriggered = false;

    private void Awake()
    {
        if (topLimit == null)
        {
            GameObject found = GameObject.Find("TopLimit");
            if (found != null) topLimit = found;
        }

        if (instructionsUI == null)
        {
            GameObject found = GameObject.Find("Instructions");
            if (found != null) instructionsUI = found;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (alreadyTriggered) return;
        if (!collision.collider.CompareTag("Player")) return;
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        
        if (player == null)
            player = collision.collider.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.EnableControls();
        }

        if (topLimit != null)
        {
            topLimit.SetActive(true);
            alreadyTriggered = true;
        }

        if (instructionsUI != null)
        {
            StartCoroutine(FadeOutAndDisable());
            alreadyTriggered = true;
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        CanvasGroup canvasGroup = instructionsUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = instructionsUI.AddComponent<CanvasGroup>();
        }

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
        instructionsUI.SetActive(false);
    }
}
