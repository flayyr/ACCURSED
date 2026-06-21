using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CustomDynamicLit))]
public class ShakeInteract : MonoBehaviour
{
    [SerializeField] float shakeDuration=0.5f;
    [SerializeField] float shakeMagnitude=1f;
    [SerializeField] float shakeFrequency=10f;
    [SerializeField] bool AllowKeyboardTest;

    float shakeAmt;

    CustomDynamicLit litScript;
    List<SpriteRenderer> spriteRenderers;
    private void Start()
    {
        litScript = GetComponent<CustomDynamicLit>();

        spriteRenderers = new List<SpriteRenderer>();

        if(TryGetComponent(out TreeScript treeScript)){
            spriteRenderers.Add(treeScript.leavesDynamicLit.gameObject.GetComponent<SpriteRenderer>());
        }

        spriteRenderers.Add( GetComponent<SpriteRenderer>());

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.EnableKeyword("_SHAKEINTERACT");
        }
    }

    private void Update()
    {
        if (AllowKeyboardTest)
        {
            if (Input.GetKey(KeyCode.T))
            {
                Shake();
            }
        }
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float timer = 0;
        float direction = Random.Range(0, 2)==0 ? 1f:-1f;
        while (timer < shakeDuration)
        {
            timer+=Time.deltaTime;
            shakeAmt = direction * shakeMagnitude * (shakeDuration-timer)/shakeDuration * Mathf.Sin(timer * shakeFrequency);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.material.SetFloat("_ShakeAmount", shakeAmt);
            }
            yield return new WaitForEndOfFrame();
        }
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.SetFloat("_ShakeAmount", 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Shake();
    }
}
