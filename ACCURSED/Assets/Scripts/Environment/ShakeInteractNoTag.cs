using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

[RequireComponent(typeof(CustomDynamicLit))]
public class ShakeInteractNoTag : MonoBehaviour
{
    [SerializeField] float shakeDuration = 0.35f;
    [SerializeField] float shakeMagnitude = 0.1f;
    [SerializeField] float shakeFrequency = 1f;
    
    [SerializeField] bool shakeByLayer = false;
    [SerializeField] int shakeLayer = 0;
    [Header("Bend Offset")]
    [SerializeField] bool grassBend = false;
    [SerializeField] float bendAmountMax = 0f;
    [SerializeField] float bendLerpDuration = 0.1f;
    
    float shakeAmt;

    float bendOffset = 0f;
    float bendTarget = 0f;

    CustomDynamicLit litScript;
    List<SpriteRenderer> spriteRenderers;
    private void Start()
    {
        litScript = GetComponent<CustomDynamicLit>();

        spriteRenderers = new List<SpriteRenderer>();

        if (TryGetComponent(out TreeScript treeScript))
        {
            spriteRenderers.Add(treeScript.leavesDynamicLit.gameObject.GetComponent<SpriteRenderer>());
        }

        spriteRenderers.Add(GetComponent<SpriteRenderer>());

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.EnableKeyword("_SHAKEINTERACT");
        }
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float timer = 0;
        float direction = Random.Range(0, 2) == 0 ? 1f : -1f;
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            shakeAmt = direction * shakeMagnitude * (shakeDuration - timer) / shakeDuration * Mathf.Sin(timer * shakeFrequency);

            if (grassBend)
            {
                bendOffset = Mathf.Lerp(bendOffset, bendTarget, timer / bendLerpDuration);
            }

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.material.SetFloat("_ShakeAmount", shakeAmt + bendOffset);
            }
            yield return new WaitForEndOfFrame();
        }
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.SetFloat("_ShakeAmount", 0f + bendOffset);
        }
    }

    private IEnumerator ResetBend()
    {
        float timer = 0f;
        bendTarget = 0f;
        while (timer < bendLerpDuration)
        {
            timer += Time.deltaTime;
            bendOffset = Mathf.Lerp(bendOffset, bendTarget, timer / bendLerpDuration);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.material.SetFloat("_ShakeAmount", shakeAmt + bendOffset);
            }

            yield return new WaitForEndOfFrame();
        }
        bendOffset = 0f;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.SetFloat("_ShakeAmount", 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (shakeByLayer && collision.gameObject.layer == shakeLayer)
        {
            if (grassBend)
            {
                bendTarget = Mathf.Sign(collision.transform.position.x - transform.position.x) * bendAmountMax;
            }
            Shake();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (grassBend && collision.gameObject.tag == "Player")
        {
            StartCoroutine(ResetBend());
        }
    }
}
