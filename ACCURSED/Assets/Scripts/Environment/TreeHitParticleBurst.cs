using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TreeHitParticleBurst : MonoBehaviour
{
    [Header("Hit Detection")]
    [Tooltip("Layer used by the player's attack hitbox objects.")]
    [SerializeField] private LayerMask hitboxLayer;

    [Tooltip("The tree's trigger collider that receives hits.")]
    [SerializeField] private Collider2D treeHitCollider;

    [Tooltip("Prevents one attack from triggering too many bursts instantly.")]
    [SerializeField] private float hitCooldown = 0.1f;

    [Header("Particle Burst")]
    [SerializeField] private ParticleSystem hitParticles;

    [Tooltip("How many particles to emit when the tree is hit.")]
    [SerializeField] private int burstAmount = 5;

    [Tooltip("Temporary max particles during the hit burst.")]
    [SerializeField] private int burstMaxParticles = 20;

    [Tooltip("How long before Max Particles returns to normal.")]
    [SerializeField] private float maxParticleResetDelay = 0.5f;

    [Tooltip("Move the particle system to the hit point before bursting.")]
    [SerializeField] private bool burstAtHitPoint = false   ;

    [SerializeField] private Vector3 particleOffset;

    private Rigidbody2D rb;
    private float nextAllowedHitTime;
    private int originalMaxParticles;
    private Coroutine resetMaxParticlesCoroutine;

    private void Reset()
    {
        AutoSetup();
    }

    private void Awake()
    {
        AutoSetup();

        if (hitParticles == null)
            hitParticles = GetComponentInChildren<ParticleSystem>();

        if (hitParticles != null)
        {
            ParticleSystem.MainModule main = hitParticles.main;
            originalMaxParticles = main.maxParticles;
        }
    }

    private void AutoSetup()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (treeHitCollider == null)
            treeHitCollider = GetComponent<Collider2D>();

        if (treeHitCollider == null)
            treeHitCollider = gameObject.AddComponent<BoxCollider2D>();

        treeHitCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryBurstFromHit(collision);
    }

    private void TryBurstFromHit(Collider2D collision)
    {
        if (Time.time < nextAllowedHitTime)
            return;

        bool isHitboxLayer =
            (hitboxLayer.value & (1 << collision.gameObject.layer)) != 0;

        if (!isHitboxLayer)
            return;

        nextAllowedHitTime = Time.time + hitCooldown;

        EmitParticleBurst(collision);
    }

    private void EmitParticleBurst(Collider2D hitboxCollider)
    {
        if (hitParticles == null)
        {
            Debug.LogWarning($"{name} was hit, but no Particle System is assigned.");
            return;
        }

        if (burstAtHitPoint)
        {
            Vector2 hitPoint = hitboxCollider.ClosestPoint(transform.position);
            hitParticles.transform.position = (Vector3)hitPoint + particleOffset;
        }

        TemporarilyIncreaseMaxParticles();

        hitParticles.Emit(burstAmount);
    }

    private void TemporarilyIncreaseMaxParticles()
    {
        ParticleSystem.MainModule main = hitParticles.main;

        int neededMaxParticles = Mathf.Max(burstMaxParticles, burstAmount);

        main.maxParticles = neededMaxParticles;

        if (resetMaxParticlesCoroutine != null)
            StopCoroutine(resetMaxParticlesCoroutine);

        resetMaxParticlesCoroutine = StartCoroutine(ResetMaxParticlesAfterDelay());
    }

    private IEnumerator ResetMaxParticlesAfterDelay()
    {
        yield return new WaitForSeconds(maxParticleResetDelay);

        if (hitParticles != null)
        {
            ParticleSystem.MainModule main = hitParticles.main;
            main.maxParticles = originalMaxParticles;
        }

        resetMaxParticlesCoroutine = null;
    }
}