using UnityEngine;
using Cinemachine;
using System.Collections;

public class WindGustCameraZoom : MonoBehaviour
{
    [Header("Camera Settings")]
    public float zoomAmount = 5f;       // How much to zoom out
    public float zoomDuration = 0.5f;   // How long each zoom transition takes
    public float holdTime = 1f;         // How long to hold the zoom-out before zooming back in
    public float maxZoomSize = 8f;      // Maximum camera size allowed

    private CinemachineVirtualCamera virtualCamera;
    private float originalSize;
    private bool isZooming = false;
    private Coroutine zoomCoroutine;
    private float currentHoldTime;

    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (virtualCamera != null)
            originalSize = virtualCamera.m_Lens.OrthographicSize;
        else
            Debug.LogWarning("No CinemachineVirtualCamera found in the scene!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && virtualCamera != null)
        {
            if (!isZooming)
            {
                zoomCoroutine = StartCoroutine(ZoomCoroutine());
            }
            else
            {
                // Reset hold timer if already zoomed
                currentHoldTime = 0f;
            }
        }
    }

    private IEnumerator ZoomCoroutine()
    {
        isZooming = true;
        currentHoldTime = 0f;

        float targetSize = Mathf.Min(originalSize + zoomAmount, maxZoomSize);
        float startSize = virtualCamera.m_Lens.OrthographicSize;

        // Zoom out
        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / zoomDuration);
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = targetSize;

        // Hold zoom until holdTime is reached
        while (currentHoldTime < holdTime)
        {
            currentHoldTime += Time.deltaTime;
            yield return null;
        }

        // Zoom back in
        elapsed = 0f;
        startSize = virtualCamera.m_Lens.OrthographicSize;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, originalSize, elapsed / zoomDuration);
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = originalSize;

        isZooming = false;
    }
}
