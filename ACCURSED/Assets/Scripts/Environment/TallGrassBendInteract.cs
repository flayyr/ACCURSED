using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShakeInteractNew))]
public class TallGrassBendInteract : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to bend this object directly. ")]
    [SerializeField] private Transform visualTarget;

    private ShakeInteractNew shakeInteractNew;

    [Header("Player Detection")]
    [Tooltip("Set this to the layer used by the player's body collider.")]
    [SerializeField] private LayerMask playerBodyLayer;

    [Tooltip("If Player Body Mask is empty, this script can still detect objects tagged Player.")]
    [SerializeField] private bool usePlayerTagIfMaskEmpty = true;

    [SerializeField] private string playerTag = "Player";

    [Header("Optional Hitbox Shake")]
    [SerializeField] private bool callShakeWhenHitboxEnters = false;

    [SerializeField] private LayerMask playerHitboxLayer;

    [SerializeField] private float shakeCooldown = 0.1f;

    [Header("Bend Settings")]
    [SerializeField] private float maxBendAngle = 25f;

    [Tooltip("How quickly the grass bends when the player walks onto it.")]
    [SerializeField] private float bendSpeed = 100f;

    [Tooltip("How quickly the grass returns after the player leaves.")]
    [SerializeField] private float recoverSpeed = 90f;

    [Tooltip("How far left/right the player can be while still creating a strong bend.")]
    [SerializeField] private float fullBendXDistance = 0.6f;

    [Tooltip("Small dead zone so the grass does not flip direction rapidly when the player is centered.")]
    [SerializeField] private float sideDeadZone = 0.03f;

    [Tooltip("Minimum visible bend while the player is inside the grass.")]
    [Range(0f, 1f)]
    [SerializeField] private float minimumBendStrength = 0.55f;

    [Header("Splay / Skew Simulation")]
    [Tooltip("Widens the grass while bent. This simulates splaying.")]
    [SerializeField] private float splayAmount = 0.15f;

    [Tooltip("Slightly squashes the grass vertically while bent.")]
    [SerializeField] private float squashAmount = 0.05f;

    [Tooltip("How smoothly the splay/squash follows the bend.")]
    [SerializeField] private float scaleSmoothSpeed = 10f;

    private readonly HashSet<Collider2D> touchingPlayerBodies = new HashSet<Collider2D>();

    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private float currentBendAngle;
    private float targetBendAngle;
    private float lastBendSign = 1f;
    private float nextAllowedShakeTime;

    private void Awake()
    {
        shakeInteractNew = GetComponent<ShakeInteractNew>();

        if (visualTarget == null)
            visualTarget = transform;

        originalLocalRotation = visualTarget.localRotation;
        originalLocalScale = visualTarget.localScale;
    }

    private void Update()
    {
        Collider2D playerCollider = GetClosestTouchingPlayer();

        if (playerCollider != null)
        {
            targetBendAngle = GetBendAngleAwayFromPlayer(playerCollider);
            currentBendAngle = Mathf.MoveTowards(
                currentBendAngle,
                targetBendAngle,
                bendSpeed * Time.deltaTime
            );
        }
        else
        {
            targetBendAngle = 0f;
            currentBendAngle = Mathf.MoveTowards(
                currentBendAngle,
                targetBendAngle,
                recoverSpeed * Time.deltaTime
            );
        }

        ApplyBendVisual();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayerBody(collision))
        {
            touchingPlayerBodies.Add(collision);
        }

        if (callShakeWhenHitboxEnters && IsPlayerHitbox(collision))
        {
            HitFromPlayer(collision.transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsPlayerBody(collision))
        {
            touchingPlayerBodies.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (touchingPlayerBodies.Contains(collision))
        {
            touchingPlayerBodies.Remove(collision);
        }
    }

    public void HitFromPlayer(Vector3 playerPosition)
    {
        if (Time.time < nextAllowedShakeTime)
            return;

        nextAllowedShakeTime = Time.time + shakeCooldown;

        shakeInteractNew.Shake();

        ForceBendAwayFrom(playerPosition);
    }

    public void HitFromPlayer()
    {
        if (Time.time < nextAllowedShakeTime)
            return;

        nextAllowedShakeTime = Time.time + shakeCooldown;

        shakeInteractNew.Shake();
    }

    private void ForceBendAwayFrom(Vector3 sourcePosition)
    {
        float awayX = visualTarget.position.x - sourcePosition.x;

        if (Mathf.Abs(awayX) > sideDeadZone)
        {
            lastBendSign = Mathf.Sign(awayX);
        }

        currentBendAngle = -lastBendSign * maxBendAngle;
    }

    private float GetBendAngleAwayFromPlayer(Collider2D playerCollider)
    {
        float grassX = visualTarget.position.x;
        float playerX = playerCollider.bounds.center.x;

        float awayX = grassX - playerX;

        float bendSign;

        if (Mathf.Abs(awayX) > sideDeadZone)
        {
            bendSign = Mathf.Sign(awayX);
            lastBendSign = bendSign;
        }
        else
        {
            // Keeps the bend direction when the player stands still in the grass.
            bendSign = lastBendSign;
        }

        float closeness = 1f - Mathf.Clamp01(Mathf.Abs(awayX) / fullBendXDistance);
        float bendStrength = Mathf.Lerp(minimumBendStrength, 1f, closeness);

        // Positive Z rotation leans left, negative Z rotation leans right.
        // So invert the sign to make the grass bend away from the player.
        return -bendSign * maxBendAngle * bendStrength;
    }

    private void ApplyBendVisual()
    {
        visualTarget.localRotation = originalLocalRotation * Quaternion.Euler(0f, 0f, currentBendAngle);

        float bend01 = Mathf.InverseLerp(0f, maxBendAngle, Mathf.Abs(currentBendAngle));

        Vector3 targetScale = originalLocalScale;
        targetScale.x = originalLocalScale.x * (1f + splayAmount * bend01);
        targetScale.y = originalLocalScale.y * (1f - squashAmount * bend01);

        visualTarget.localScale = Vector3.Lerp(
            visualTarget.localScale,
            targetScale,
            1f - Mathf.Exp(-scaleSmoothSpeed * Time.deltaTime)
        );
    }

    private Collider2D GetClosestTouchingPlayer()
    {
        touchingPlayerBodies.RemoveWhere(collider => collider == null || !collider.enabled);

        Collider2D closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D playerCollider in touchingPlayerBodies)
        {
            float distance = Mathf.Abs(playerCollider.bounds.center.x - visualTarget.position.x);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = playerCollider;
            }
        }

        return closest;
    }

    private bool IsPlayerBody(Collider2D collision)
    {
        if (playerBodyLayer.value != 0)
        {
            return IsInLayerMask(collision.gameObject.layer, playerBodyLayer);
        }

        if (usePlayerTagIfMaskEmpty)
        {
            return collision.CompareTag(playerTag);
        }

        return false;
    }

    private bool IsPlayerHitbox(Collider2D collision)
    {
        if (playerHitboxLayer.value == 0)
            return false;

        return IsInLayerMask(collision.gameObject.layer, playerHitboxLayer);
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void OnDisable()
    {
        touchingPlayerBodies.Clear();

        if (visualTarget != null)
        {
            visualTarget.localRotation = originalLocalRotation;
            visualTarget.localScale = originalLocalScale;
        }

        currentBendAngle = 0f;
        targetBendAngle = 0f;
    }
}