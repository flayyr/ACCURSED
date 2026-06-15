using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Bounds bounds = spriteRenderer.bounds;
        bounds.Expand(/*spriteRenderer.size.y * LightManager.instance.ambientShadowSkew* */ 200f);
        spriteRenderer.localBounds = bounds;
    }
}
