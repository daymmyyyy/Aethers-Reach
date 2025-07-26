using UnityEngine;
using Cinemachine;

[ExecuteAlways]
[AddComponentMenu("Cinemachine/Extensions/Clamp Y Position")]
public class CameraClampY : CinemachineExtension
{
    public Transform topLimit;
    public Transform bottomLimit;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body) return;
        if (topLimit == null || bottomLimit == null) return;

        float camHalfHeight = state.Lens.OrthographicSize;
        float minY = bottomLimit.position.y + camHalfHeight;
        float maxY = topLimit.position.y - camHalfHeight;

        Vector3 pos = state.RawPosition;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        state.RawPosition = pos;
    }
}
