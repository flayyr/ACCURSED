using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomDynamicLit))]
public class ShakeInteract : MonoBehaviour
{
    [SerializeField] float shakeDuration=0.5f;
    [SerializeField] float shakeMagnitude=1f;
    [SerializeField] float shakeFrequency=10f;

    float shakeAmt;

    CustomDynamicLit litScript;
    SpriteRenderer spriteRenderer;
    private void Start()
    {
        litScript = GetComponent<CustomDynamicLit>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.material.EnableKeyword("_SHAKEINTERACT");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            Shake();
        }
    }

    [ContextMenu("Shake")]
    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float timer = 0;
        while (timer < shakeDuration)
        {
            timer+=Time.deltaTime;
            shakeAmt = shakeMagnitude * (shakeDuration-timer)/shakeDuration * Mathf.Sin(timer * shakeFrequency);
            spriteRenderer.material.SetFloat("_ShakeAmount", shakeAmt);
            yield return new WaitForEndOfFrame();
        }
        spriteRenderer.material.SetFloat("_ShakeAmount", 0f);
    }
}
