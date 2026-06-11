using UnityEngine;

public class FollowCameraWithOffset : MonoBehaviour
{
    [Header("Camera To Follow")]
    public Transform cameraTransform;

    [Header("Distance From Camera")]
    public float xOffset = 0f;
    public float yOffset = 0f;

    [Header("Follow Settings")]
    public bool followX = true;
    public bool followY = true;
    public bool keepOriginalZ = true;

    private float originalZ;

    private void Start()
    {
        originalZ = transform.position.z;

        // Automatically use the main camera if none is assigned
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        Vector3 newPosition = transform.position;

        if (followX)
        {
            newPosition.x = cameraTransform.position.x + xOffset;
        }

        if (followY)
        {
            newPosition.y = cameraTransform.position.y + yOffset;
        }

        if (keepOriginalZ)
        {
            newPosition.z = originalZ;
        }
        else
        {
            newPosition.z = cameraTransform.position.z;
        }

        transform.position = newPosition;
    }
}