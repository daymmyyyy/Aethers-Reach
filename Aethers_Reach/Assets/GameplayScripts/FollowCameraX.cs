using UnityEngine;
using Cinemachine;

public class FollowCameraX : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public float followSpeed = 5f; // adjust in inspector

    private Transform camTransform;

    void Start()
    {
        if (virtualCamera != null)
            camTransform = virtualCamera.VirtualCameraGameObject.transform;
    }

    void LateUpdate()
    {
        if (camTransform != null)
        {
            // Target only the camera's X
            Vector3 targetPos = new Vector3(camTransform.position.x, transform.position.y, transform.position.z);

            // Smooth follow with adjustable speed
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );
        }
    }
}
