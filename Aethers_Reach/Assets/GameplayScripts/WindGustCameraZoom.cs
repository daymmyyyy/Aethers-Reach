using UnityEngine;
using Cinemachine;
using System.Collections;

public class WindGustCameraZoom : MonoBehaviour
{
    [Header("Camera Settings")]
    public float zoomAmount = 5f;      // How much to zoom out
    public float zoomDuration = 0.5f;  // Zoom transition duration
    public float holdTime = 1f;        // How long to hold zoom-out
    public float maxZoomOut = 8f;      // Max allowed zoom-out

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;
    private float originalSize;
    private bool isZooming = false;

    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            originalSize = virtualCamera.m_Lens.OrthographicSize;
            transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                // Slightly soften the follow so player isn't perfectly centered
                transposer.m_XDamping = 1f;
                transposer.m_YDamping = 1f;
            }
        }
        else
        {
            Debug.LogWarning("No CinemachineVirtualCamera found in the scene!");
        }
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

        float targetSize = Mathf.Min(originalSize + zoomAmount, originalSize + maxZoomOut);
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsed = 0f;

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
