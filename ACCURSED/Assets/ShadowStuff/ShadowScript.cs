using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private void Start()
    {
        transform.localPosition = Vector3.zero;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = transform.parent.GetComponent<SpriteRenderer>().sprite;
    }

    private void Update()
    {
        
    }
}
