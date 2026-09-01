using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Bounds bounds = spriteRenderer.bounds;
        bounds.Expand(/*spriteRenderer.size.y * LightManager.instance.ambientShadowSkew* */ float.MaxValue);
        spriteRenderer.localBounds = bounds;
    }

    public void SetShadowOffset(Vector3 targetPosition)
    {
        Vector2 offset = targetPosition - transform.position;
        spriteRenderer.material.SetVector("_PositionOffset", offset);
    }
}
