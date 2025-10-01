using UnityEngine;
using UnityEngine.UI;

public class PowerUpSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float startTime;
    private float duration;
    private bool running;

    void Awake()
    {
        if (slider != null)
            slider.gameObject.SetActive(false); // hide at the start
    }

    public void StartTimer(float duration)
    {
        if (slider == null) return;

        this.duration = duration;
        startTime = Time.time;
        running = true;

        slider.gameObject.SetActive(true); // only show when timer starts
        slider.value = 1f; // start full
    }

    void Update()
    {
        if (!running || slider == null) return;

        float elapsed = Time.time - startTime;
        float progress = Mathf.Clamp01(elapsed / duration);

        // Slider goes from 1 → 0 over the duration
        slider.value = 1f - progress;

        if (elapsed >= duration)
        {
            running = false;
            slider.value = 0f;
            slider.gameObject.SetActive(false); // hide when done
        }
    }

    public float SliderValue()
    {
        return slider != null ? slider.value : 0f;
    }
}
