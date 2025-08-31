using UnityEngine;
using Cinemachine;
using System.Collections;

public class WindGustCameraZoom : MonoBehaviour
{
    [Header("Camera Settings")]
    public float zoomAmount = 5f;      // How much to zoom out
    public float zoomDuration = 0.5f;  // How long each zoom transition takes
    public float holdTime = 1f;        // How long to hold the zoom-out before zooming back in

    private CinemachineVirtualCamera virtualCamera;
    private float originalSize;
    private bool isZooming = false;

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
        if (collision.CompareTag("Player") && !isZooming && virtualCamera != null)
        {
            StartCoroutine(ZoomCoroutine());
        }
    }

    private IEnumerator ZoomCoroutine()
    {
        isZooming = true;
        float elapsed = 0f;
        float targetSize = originalSize + zoomAmount;
        float startSize = virtualCamera.m_Lens.OrthographicSize;

        // Zoom out
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / zoomDuration);
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = targetSize;

        // Hold zoom
        yield return new WaitForSeconds(holdTime);

        // Zoom back in
        elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(targetSize, originalSize, elapsed / zoomDuration);
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = originalSize;

        isZooming = false;
    }
}
