using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FallingLeavesScreenFitter : MonoBehaviour
{
    [Header("Camera")]
    public Camera targetCamera;

    [Header("Placement")]
    public float yOffsetAboveScreen = 1f;
    public float zPosition = 0f;

    [Header("Emitter Size")]
    public float baseExtraWidth = 20f;
    public float emitterHeight = 1f;

    [Header("Fast Camera Movement Buffer")]
    public bool useSpeedBasedBuffer = true;
    public float speedBufferMultiplier = 2f;
    public float maxExtraSpeedWidth = 40f;

    [Header("Refill Burst")]
    public bool burstWhenCameraMovesTooFar = true;
    public float burstMoveThreshold = 5f;
    public int burstAmount = 80;

    private ParticleSystem ps;
    private Vector3 lastCameraPosition;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            lastCameraPosition = targetCamera.transform.position;

        FitToCamera(0f);
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        float cameraMoveDistance = Vector3.Distance(
            targetCamera.transform.position,
            lastCameraPosition
        );

        float cameraSpeed = cameraMoveDistance / Mathf.Max(Time.deltaTime, 0.0001f);

        FitToCamera(cameraSpeed);

        if (burstWhenCameraMovesTooFar && cameraMoveDistance >= burstMoveThreshold)
        {
            ps.Emit(burstAmount);
        }

        lastCameraPosition = targetCamera.transform.position;
    }

    private void FitToCamera(float cameraSpeed)
    {
        if (targetCamera == null || ps == null)
            return;

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        float speedExtraWidth = 0f;

        if (useSpeedBasedBuffer)
        {
            speedExtraWidth = cameraSpeed * speedBufferMultiplier;
            speedExtraWidth = Mathf.Clamp(speedExtraWidth, 0f, maxExtraSpeedWidth);
        }

        Vector3 cameraPos = targetCamera.transform.position;

        transform.position = new Vector3(
            cameraPos.x,
            cameraPos.y + targetCamera.orthographicSize + yOffsetAboveScreen,
            zPosition
        );

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;

        shape.scale = new Vector3(
            cameraWidth + baseExtraWidth + speedExtraWidth,
            emitterHeight,
            1f
        );
    }
}