using UnityEngine;

public class ReflectionSprite : MonoBehaviour
{
    GameObject reflectionPrefab;
    SpriteRenderer spriteRenderer;
    [SerializeField] float reflectionYScale = 0.7f;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        reflectionPrefab = Resources.Load<GameObject>("ReflectionsPrefab");
        SpriteRenderer reflectionRenderer = Instantiate(reflectionPrefab, transform).GetComponent<SpriteRenderer>();
        reflectionRenderer.transform.localPosition = Vector3.zero;
        reflectionRenderer.sprite = spriteRenderer.sprite;
        reflectionRenderer.flipY = true;
        reflectionRenderer.sortingOrder = spriteRenderer.sortingOrder;
        reflectionRenderer.transform.localScale = new Vector3(1, reflectionYScale, 1);
    }
}
