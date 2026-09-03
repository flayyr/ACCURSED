using UnityEngine;

public class SpriteUpdater : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    SpriteRenderer spriteRenderer;
    Sprite lastSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTargetRenderer(SpriteRenderer targetSpriteRenderer)
    {
        this.targetSpriteRenderer = targetSpriteRenderer;
        lastSprite = targetSpriteRenderer.sprite;
    }

    void Update()
    {
        if (targetSpriteRenderer != null && lastSprite != targetSpriteRenderer.sprite)
        {
            lastSprite = targetSpriteRenderer.sprite;
            spriteRenderer.sprite = lastSprite;
        }
    }
}
