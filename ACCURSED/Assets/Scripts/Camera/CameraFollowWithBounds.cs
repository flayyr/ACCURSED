using UnityEngine;

[DefaultExecutionOrder(100)]
public class CameraFollowWithMapEdges : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("Camera")]
    [Tooltip("Assign the main gameplay camera. This can be this object or a child camera.")]
    [SerializeField] private Camera mainCamera;

    [SerializeField] private float zOffset = -10f;

    [Header("Map Edges")]
    [Tooltip("The actual left edge of the map in world position.")]
    [SerializeField] private float mapLeft = -30f;

    [Tooltip("The actual right edge of the map in world position.")]
    [SerializeField] private float mapRight = 30f;

    [Tooltip("The actual bottom edge of the map in world position.")]
    [SerializeField] private float mapBottom = -20f;

    [Tooltip("The actual top edge of the map in world position.")]
    [SerializeField] private float mapTop = 20f;

    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private float smoothTime = 0.08f;

    private Vector3 velocity;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponentInChildren<Camera>();
        }

        if (target == null)
        {
            FindPlayerTarget();
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();

            if (target == null)
                return;
        }

        if (mainCamera == null)
            return;

        Vector3 currentRigPosition = transform.position;

        float desiredX = followX ? target.position.x : currentRigPosition.x;
        float desiredY = followY ? target.position.y : currentRigPosition.y;

        Vector3 desiredRigPosition = new Vector3(desiredX, desiredY, zOffset);

        desiredRigPosition = ClampRigPositionToMapEdges(desiredRigPosition);

        if (smoothTime <= 0f)
        {
            transform.position = desiredRigPosition;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                currentRigPosition,
                desiredRigPosition,
                ref velocity,
                smoothTime
            );
        }
    }

    private Vector3 ClampRigPositionToMapEdges(Vector3 desiredRigPosition)
    {
        float cameraHalfHeight = mainCamera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        float minCameraCenterX = mapLeft + cameraHalfWidth;
        float maxCameraCenterX = mapRight - cameraHalfWidth;

        float minCameraCenterY = mapBottom + cameraHalfHeight;
        float maxCameraCenterY = mapTop - cameraHalfHeight;

        Vector3 cameraOffsetFromRig = mainCamera.transform.position - transform.position;

        Vector3 desiredCameraCenter = desiredRigPosition + cameraOffsetFromRig;

        float clampedCameraX;
        float clampedCameraY;

        if (minCameraCenterX > maxCameraCenterX)
        {
            clampedCameraX = (mapLeft + mapRight) * 0.5f;
        }
        else
        {
            clampedCameraX = Mathf.Clamp(
                desiredCameraCenter.x,
                minCameraCenterX,
                maxCameraCenterX
            );
        }

        if (minCameraCenterY > maxCameraCenterY)
        {
            clampedCameraY = (mapBottom + mapTop) * 0.5f;
        }
        else
        {
            clampedCameraY = Mathf.Clamp(
                desiredCameraCenter.y,
                minCameraCenterY,
                maxCameraCenterY
            );
        }

        Vector3 clampedCameraCenter = new Vector3(
            clampedCameraX,
            clampedCameraY,
            desiredCameraCenter.z
        );

        Vector3 clampedRigPosition = clampedCameraCenter - cameraOffsetFromRig;
        clampedRigPosition.z = zOffset;

        return clampedRigPosition;
    }

    private void FindPlayerTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetTargetToPlayer()
    {
        FindPlayerTarget();
    }

    public void SetMapEdges(float left, float right, float bottom, float top)
    {
        mapLeft = left;
        mapRight = right;
        mapBottom = bottom;
        mapTop = top;
    }
}