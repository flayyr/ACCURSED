using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CustomDynamicLit))]
public class ModifiedShakeInteract : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.5f;
    [SerializeField] private float shakeFrequency = 7f;

    [Header("Optional Trigger Shake")]
    [Tooltip("Turn this OFF if another script will call Shake().")]
    [SerializeField] private bool shakeOnTriggerEnter = false;

    [Tooltip("Only these layers can trigger shake if Shake On Trigger Enter is enabled.")]
    [SerializeField] private LayerMask shakeTriggerLayer;

    [Header("Testing")]
    [SerializeField] private bool allowKeyboardTest = false;
    [SerializeField] private KeyCode testKey = KeyCode.T;

    [Header("Direction")]
    [Tooltip("Use this if the shake direction feels backwards.")]
    [SerializeField] private bool invertShakeDirection = false;

    private float shakeAmt;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private Coroutine shakeRoutine;

    private void Awake()
    {
        SetupSpriteRenderers();
    }

    private void Update()
    {
        if (allowKeyboardTest && Input.GetKeyDown(testKey))
        {
            Shake();
        }
    }

    private void SetupSpriteRenderers()
    {
        spriteRenderers.Clear();

        if (TryGetComponent(out TreeScript treeScript))
        {
            if (treeScript.leavesDynamicLit != null)
            {
                SpriteRenderer leavesRenderer = treeScript.leavesDynamicLit.gameObject.GetComponent<SpriteRenderer>();

                if (leavesRenderer != null)
                {
                    spriteRenderers.Add(leavesRenderer);
                }
            }
        }

        if (TryGetComponent(out SpriteRenderer ownRenderer))
        {
            spriteRenderers.Add(ownRenderer);
        }

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
        if (spriteRenderers == null || spriteRenderers.Count == 0)
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

        float direction;

        if (Mathf.Abs(forcedDirection) > 0.01f)
        {
            direction = Mathf.Sign(forcedDirection);
        }
        else
        {
            direction = Random.Range(0, 2) == 0 ? 1f : -1f;
        }

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

        SetShakeAmount(0f);
        shakeRoutine = null;
    }

    private void SetShakeAmount(float amount)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null && spriteRenderer.material != null)
            {
                spriteRenderer.material.SetFloat("_ShakeAmount", amount);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!shakeOnTriggerEnter)
            return;

        if (!IsInLayerMask(collision.gameObject.layer, shakeTriggerLayer))
            return;

        ShakeFromSource(collision.bounds.center);
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}