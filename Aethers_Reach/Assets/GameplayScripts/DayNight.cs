using UnityEngine;

[System.Serializable]
public class DarkenObject
{
    public SpriteRenderer spriteRenderer;
    [Range(0f, 1f)]
    public float maxAlpha = 0.6f; // maximum darkness at night
}

public class DayNight : MonoBehaviour
{
    [Header("Objects to Darken")]
    public DarkenObject[] objectsToDarken;

    [Header("Cycle Durations")]
    public float dayDuration = 30f;
    public float nightFadeDuration = 2f;
    public float nightHoldDuration = 30f;
    public float dayFadeDuration = 5f;

    [Header("Night VFX")]
    public GameObject[] VFX; // assign multiple VFX
    public float fadeDuration = 2f;

    private enum Phase { Day, NightFadeIn, NightHold, DayFadeIn }
    private Phase currentPhase = Phase.Day;

    private float timer = 0f;
    private CanvasGroup[] starsCanvasGroups; // one per VFX

    void Start()
    {
        // Add CanvasGroups to all stars for fading
        starsCanvasGroups = new CanvasGroup[VFX.Length];
        for (int i = 0; i < VFX.Length; i++)
        {
            if (VFX[i] != null)
            {
                CanvasGroup cg = VFX[i].GetComponent<CanvasGroup>();
                if (!cg) cg = VFX[i].AddComponent<CanvasGroup>();
                cg.alpha = 0f;
                starsCanvasGroups[i] = cg;
                VFX[i].SetActive(false); // hide at start
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case Phase.Day:
                UpdateAlpha(0f);
                SetStarsActive(false);
                if (timer >= dayDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.NightFadeIn;
                    SetStarsActive(true);
                }
                break;

            case Phase.NightFadeIn:
                float tFade = Mathf.Clamp01(timer / nightFadeDuration);
                UpdateAlpha(tFade);
                UpdateStars(tFade);
                if (timer >= nightFadeDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.NightHold;
                }
                break;

            case Phase.NightHold:
                UpdateAlpha(1f);
                UpdateStars(1f);
                if (timer >= nightHoldDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.DayFadeIn;
                }
                break;

            case Phase.DayFadeIn:
                float tDay = Mathf.Clamp01(timer / dayFadeDuration);
                UpdateAlpha(1f - tDay);
                UpdateStars(1f - tDay);
                if (timer >= dayFadeDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.Day;
                    SetStarsActive(false);
                }
                break;
        }
    }

    void UpdateAlpha(float t)
    {
        foreach (var obj in objectsToDarken)
        {
            Color c = obj.spriteRenderer.color;
            c.a = Mathf.Lerp(0f, obj.maxAlpha, t);
            obj.spriteRenderer.color = c;
        }
    }

    void UpdateStars(float t)
    {
        if (starsCanvasGroups == null) return;

        for (int i = 0; i < starsCanvasGroups.Length; i++)
        {
            if (starsCanvasGroups[i] != null)
                starsCanvasGroups[i].alpha = t;
        }
    }

    void SetStarsActive(bool active)
    {
        foreach (var star in VFX)
        {
            if (star != null)
                star.SetActive(active);
        }
    }
}
