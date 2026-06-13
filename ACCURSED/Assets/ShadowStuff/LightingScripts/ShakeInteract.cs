using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CustomDynamicLit))]
public class ShakeInteract : MonoBehaviour
{
    [SerializeField] float shakeDuration=0.5f;
    [SerializeField] float shakeMagnitude=1f;
    [SerializeField] float shakeFrequency=10f;

    float shakeAmt;

    CustomDynamicLit litScript;
    List<SpriteRenderer> spriteRenderers;
    private void Start()
    {
        litScript = GetComponent<CustomDynamicLit>();

        spriteRenderers = new List<SpriteRenderer>();

        if(TryGetComponent(out TreeScript treeScript)){
            spriteRenderers.Add(treeScript.leavesDynamicLit.gameObject.GetComponent<SpriteRenderer>());
            if (treeScript.leavesDynamicLit != null)
            {
                Debug.Log("leavesLit" );
            }
        }

        spriteRenderers.Add( GetComponent<SpriteRenderer>());

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.EnableKeyword("_SHAKEINTERACT");
        }
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
}
