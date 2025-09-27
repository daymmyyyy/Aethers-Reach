using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InvincibilityPowerUp : MonoBehaviour
{
    public float invincibilityDuration = 2f;
    private float transparentAlpha = 0.3f;
    private float originalAlpha = 1f;

    private Slider timerSlider;

    void Awake()
    {
        GameObject sliderObj = GameObject.FindGameObjectWithTag("PowerUpSlider");
        if (sliderObj != null)
        {
            timerSlider = sliderObj.GetComponent<Slider>();
            timerSlider.gameObject.SetActive(false); // hidden at start
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            StartCoroutine(TemporaryDisableColliders());
        }
    }

    IEnumerator TemporaryDisableColliders()
    {
        Collider2D[] obstacleColliders = DisableTaggedColliders("Obstacle");
        SetAlpha("Obstacle", transparentAlpha);

        if (timerSlider != null)
        {
            timerSlider.value = 1f; // full bar
            timerSlider.gameObject.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            elapsed += Time.deltaTime;

            if (timerSlider != null)
                timerSlider.value = 1f - (elapsed / invincibilityDuration);

            yield return null;
        }

        EnableColliders(obstacleColliders);
        SetAlpha("Obstacle", originalAlpha);
        SetAlpha("Ground", originalAlpha);

        if (timerSlider != null)
            timerSlider.gameObject.SetActive(false);
    }

    Collider2D[] DisableTaggedColliders(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        Collider2D[] disabled = new Collider2D[taggedObjects.Length];

        for (int i = 0; i < taggedObjects.Length; i++)
        {
            Collider2D col = taggedObjects[i].GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
                disabled[i] = col;
            }
        }

        return disabled;
    }

    void EnableColliders(Collider2D[] colliders)
    {
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = true;
        }
    }

    void SetAlpha(string tag, float alpha)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in taggedObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }
    }
}
