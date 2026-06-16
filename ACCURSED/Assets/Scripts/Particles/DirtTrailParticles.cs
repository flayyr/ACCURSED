using UnityEngine;
using UnityEngine.Tilemaps;

public class DirtTrailParticles : MonoBehaviour
{
    [System.Serializable]
    public class TilemapTriggerLayer
    {
        public Tilemap tilemap;

        [Tooltip("If true, this tilemap can trigger dirt particles when it is the top visible tile.")]
        public bool triggersDirtParticles;
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private ParticleSystem dirtParticles;

    [Header("Tilemap Layers")]
    [Tooltip("Put tilemaps here from TOP layer to BOTTOM layer.")]
    [SerializeField] private TilemapTriggerLayer[] tilemapLayers;

    [Header("Camera To Follow")]
    [SerializeField] private Transform cameraTransform;

    [Header("Distance From Camera")]
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 0f;

    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool keepOriginalZ = true;

    [Header("Movement Settings")]
    [SerializeField] private float minMoveSpeed = 0.1f;
    [SerializeField] private float verticalMovementThreshold = 0.1f;

    [Header("Backward Particle Push")]
    [SerializeField] private float backwardPushStrength = 1.5f;

    [Header("Particle Gravity")]
    [SerializeField] private float gravityWhenMovingHorizontally = 0.6f;
    [SerializeField] private float gravityWhenMovingVertically = 0f;

    private float originalZ;

    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MainModule main;
    private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime;

    private void Awake()
    {
        originalZ = transform.position.z;

        if (dirtParticles == null)
            dirtParticles = GetComponent<ParticleSystem>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        emission = dirtParticles.emission;
        main = dirtParticles.main;
        velocityOverLifetime = dirtParticles.velocityOverLifetime;

        emission.enabled = false;

        main.simulationSpace = ParticleSystemSimulationSpace.World;

        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
    }

    private void Update()
    {
        if (player == null || playerRb == null || dirtParticles == null)
            return;

        Vector2 velocity = playerRb.linearVelocity;

        float speed = velocity.magnitude;
        bool isMoving = speed > minMoveSpeed;
        bool isOnTriggerTile = IsPlayerOnTopTriggerTile();

        if (isMoving && isOnTriggerTile)
        {
            Vector2 moveDirection = velocity.normalized;
            Vector2 oppositeDirection = -moveDirection;

            transform.rotation = Quaternion.identity;

            UpdateParticleDirection(opositeDirection: oppositeDirection);
            UpdateParticleGravity(velocity);

            emission.enabled = true;

            if (!dirtParticles.isPlaying)
                dirtParticles.Play();
        }
        else
        {
            emission.enabled = false;
        }
    }

    private void LateUpdate()
    {
        FollowCamera();
    }

    private void FollowCamera()
    {
        if (cameraTransform == null)
            return;

        Vector3 newPosition = transform.position;

        if (followX)
            newPosition.x = cameraTransform.position.x + xOffset;

        if (followY)
            newPosition.y = cameraTransform.position.y + yOffset;

        if (keepOriginalZ)
            newPosition.z = originalZ;
        else
            newPosition.z = cameraTransform.position.z;

        transform.position = newPosition;
    }

    private void UpdateParticleDirection(Vector2 opositeDirection)
    {
        Vector2 particleVelocity = opositeDirection * backwardPushStrength;

        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(particleVelocity.x);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(particleVelocity.y);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f);
    }

    private void UpdateParticleGravity(Vector2 velocity)
    {
        bool isMovingUpOrDown = Mathf.Abs(velocity.y) > verticalMovementThreshold;

        if (isMovingUpOrDown)
        {
            main.gravityModifier = gravityWhenMovingVertically;
        }
        else
        {
            main.gravityModifier = gravityWhenMovingHorizontally;
        }
    }

    private bool IsPlayerOnTopTriggerTile()
    {
        if (tilemapLayers == null || tilemapLayers.Length == 0)
            return false;

        foreach (TilemapTriggerLayer layer in tilemapLayers)
        {
            if (layer == null || layer.tilemap == null)
                continue;

            Vector3Int cellPosition = layer.tilemap.WorldToCell(player.position);
            TileBase tile = layer.tilemap.GetTile(cellPosition);

            // This is the topmost tile found under the player.
            // It decides whether dirt particles should play.
            if (tile != null)
            {
                return layer.triggersDirtParticles;
            }
        }

        return false;
    }
}