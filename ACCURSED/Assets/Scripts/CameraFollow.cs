using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Fallback Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("Camera Offset")]
    [SerializeField] private float zOffset = -10f;

    private Transform playerTarget;

    private void Start()
    {
        FindPlayer();

        if (target == null)
        {
            target = playerTarget;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            target = playerTarget;
        }

        if (target != null)
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y,
                zOffset
            );
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("CameraFollow could not find a player with tag: " + playerTag);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void FollowPlayer()
    {
        if (playerTarget == null)
        {
            FindPlayer();
        }

        target = playerTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }
}