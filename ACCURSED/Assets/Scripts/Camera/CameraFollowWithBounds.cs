using UnityEngine;

[DefaultExecutionOrder(100)]
public class CameraFollowWithBounds : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zOffset = -10f;

    [Header("Bounds")]
    [Tooltip("A Collider2D that represents the area the camera is allowed to show.")]
    [SerializeField] private Collider2D cameraBounds;

    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private float smoothTime = 0.08f;

    private Vector3 velocity;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
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

        Vector3 currentPosition = transform.position;

        float desiredX = followX ? target.position.x : currentPosition.x;
        float desiredY = followY ? target.position.y : currentPosition.y;

        Vector3 desiredPosition = new Vector3(desiredX, desiredY, zOffset);

        desiredPosition = ClampCameraToBounds(desiredPosition);

        if (smoothTime <= 0f)
        {
            transform.position = desiredPosition;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                currentPosition,
                desiredPosition,
                ref velocity,
                smoothTime
            );
        }
    }

    private Vector3 ClampCameraToBounds(Vector3 desiredPosition)
    {
        if (cameraBounds == null || mainCamera == null)
            return desiredPosition;

        Bounds bounds = cameraBounds.bounds;

        float cameraHalfHeight = mainCamera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        float minX = bounds.min.x + cameraHalfWidth;
        float maxX = bounds.max.x - cameraHalfWidth;

        float minY = bounds.min.y + cameraHalfHeight;
        float maxY = bounds.max.y - cameraHalfHeight;

        float clampedX;
        float clampedY;

        if (minX > maxX)
        {
            clampedX = bounds.center.x;
        }
        else
        {
            clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        if (minY > maxY)
        {
            clampedY = bounds.center.y;
        }
        else
        {
            clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        return new Vector3(clampedX, clampedY, desiredPosition.z);
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
}