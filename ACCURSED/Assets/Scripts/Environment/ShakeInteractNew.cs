using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DirtTrailParticles;

[RequireComponent(typeof(CustomDynamicLit))]
public class ShakeInteractNew : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.5f;
    [SerializeField] private float shakeFrequency = 7f;

    [Header("Layer Interaction")]
    [Tooltip("Turn this ON if this object should shake automatically when something enters its trigger.")]
    [SerializeField] private bool shakeOnTriggerEnter = false;

    [Header("Tag Interaction")]
    [SerializeField] private List<string> TagsToInteract = new List<string>();

    [Tooltip("Only objects on these layers can trigger shake/bend/reset.")]
    [SerializeField] private LayerMask interactionLayers;

    [Header("Bend Offset")]
    //[SerializeField] private bool grassBend = false;
    [SerializeField] private float bendAmountMax = 0f;
    [SerializeField] private float bendLerpDuration = 0.1f;

    [Header("Testing")]
    [SerializeField] private bool allowKeyboardTest = false;
    [SerializeField] private KeyCode testKey = KeyCode.T;

    [Header("Direction")]
    [Tooltip("Use this if the shake/bend direction feels backwards.")]
    [SerializeField] private bool invertShakeDirection = false;

    private float shakeAmt;
    private float currentBend = 0f;
    private float bendTarget = 0f;

    private readonly List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    private Coroutine shakeRoutine;
    private Coroutine resetBendRoutine;

    int _ShakeID;
    int _BendID;

    private void Awake()
    {
        _ShakeID = Shader.PropertyToID("_ShakeAmount");
        _BendID = Shader.PropertyToID("_BendAmount");
        SetupSpriteRenderers();
    }

    private void Update()
    {
        // Optional keyboard testing, useful for checking the shader shake without needing collisions.
        if (allowKeyboardTest && Input.GetKeyDown(testKey))
        {
            Shake();
        }
    }

    private void SetupSpriteRenderers()
    {
        spriteRenderers.Clear();

        // If this object is a tree, also shake the leaves renderer.
        if (TryGetComponent(out TreeScript treeScript))
        {
            if (treeScript.leavesDynamicLit != null)
            {
                SpriteRenderer leavesRenderer =
                    treeScript.leavesDynamicLit.gameObject.GetComponent<SpriteRenderer>();

                if (leavesRenderer != null)
                {
                    spriteRenderers.Add(leavesRenderer);
                }
            }
        }

        // Shake this object's own SpriteRenderer too.
        if (TryGetComponent(out SpriteRenderer ownRenderer))
        {
            spriteRenderers.Add(ownRenderer);
        }

        // Enable the shader keyword needed by the material.
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null && spriteRenderer.material != null)
            {
                spriteRenderer.material.EnableKeyword("_SHAKEINTERACT");
            }
        }
    }

    public void Shake()
    {
        StartShake(0f);
    }

    public void Shake(float direction)
    {
        StartShake(direction);
    }

    public void ShakeFromSource(Vector3 sourcePosition)
    {
        float direction = sourcePosition.x < transform.position.x ? 1f : -1f;

        if (invertShakeDirection)
        {
            direction *= -1f;
        }

        StartShake(direction);
    }

    private void StartShake(float direction)
    {
        if (spriteRenderers.Count == 0)
        {
            SetupSpriteRenderers();
        }

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            SetShakeAmount(0f);
        }

        shakeRoutine = StartCoroutine(ShakeCoroutine(direction));
    }

    private IEnumerator ShakeCoroutine(float forcedDirection)
    {
        float timer = 0f;

        float direction = Mathf.Abs(forcedDirection) > 0.01f
            ? Mathf.Sign(forcedDirection)
            : Random.Range(0, 2) == 0 ? 1f : -1f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            shakeAmt =
                direction *
                shakeMagnitude *
                (shakeDuration - timer) / shakeDuration *
                Mathf.Sin(timer * shakeFrequency);

            SetShakeAmount(shakeAmt);

            yield return null;
        }

        shakeAmt = 0f;
        SetShakeAmount(0);

        shakeRoutine = null;
    }

    //private IEnumerator ResetBend()
    //{
    //    float timer = 0f;
    //    bendTarget = 0f;

    //    while (timer < bendLerpDuration)
    //    {
    //        timer += Time.deltaTime;

    //        currentBend = Mathf.Lerp(currentBend, bendTarget, timer / bendLerpDuration);

    //        SetBendAmount(currentBend);

    //        yield return null;
    //    }

    //    currentBend = 0f;
    //    SetShakeAmount(0f);

    //    resetBendRoutine = null;
    //}

    private void SetShakeAmount(float amount)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null && spriteRenderer.material != null)
            {
                spriteRenderer.material.SetFloat(_ShakeID, amount);
            }
        }
    }

    private void SetBendAmount(float amount)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null && spriteRenderer.material != null)
            {
                spriteRenderer.material.SetFloat(_BendID, amount);
            }
        }
    }

    public void SetBendProgress(float progress, float direction)
    {
        float amount = progress * direction * bendAmountMax;
        if(currentBend != amount)
        {
            currentBend = amount;
            SetBendAmount(currentBend);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!shakeOnTriggerEnter)
            return;

        if (!IsInLayerMask(collision.gameObject.layer, interactionLayers))
            return;

        foreach (string tag in TagsToInteract)
        {
            if (!collision.gameObject.CompareTag(tag))
            {
                return;
            }
        }

        //if (grassBend)
        //{
        //    bendTarget =
        //        Mathf.Sign(collision.transform.position.x - transform.position.x) *
        //        bendAmountMax;

        //    if (invertShakeDirection)
        //    {
        //        bendTarget *= -1f;
        //    }
        //}

        ShakeFromSource(collision.bounds.center);
    }

    private void OnDisable()
    {
        // If the object is culled while bent/shaking, stop everything
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        if (resetBendRoutine != null)
        {
            StopCoroutine(resetBendRoutine);
            resetBendRoutine = null;
        }

        shakeAmt = 0f;
        currentBend = 0f;
        bendTarget = 0f;

        SetShakeAmount(0f);
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (!grassBend)
    //        return;

    //    if (!IsInLayerMask(collision.gameObject.layer, interactionLayers))
    //        return;

    //    // If this object is being disabled by CullManager, do not start a coroutine
    //    if (!gameObject.activeInHierarchy || !isActiveAndEnabled)
    //    {
    //        currentBend = 0f;
    //        bendTarget = 0f;
    //        shakeAmt = 0f;
    //        SetShakeAmount(0f);
    //        return;
    //    }

    //    if (resetBendRoutine != null)
    //    {
    //        StopCoroutine(resetBendRoutine);
    //    }

    //    resetBendRoutine = StartCoroutine(ResetBend());
    //}

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}