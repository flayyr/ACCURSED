using UnityEngine;

//[ExecuteAlways]
public class DynamicLit : MonoBehaviour
{
    Transform lightTransform;
    SpriteRenderer spriteRenderer;
    SpriteRenderer shadowRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = Resources.Load<Material>("DynamicLitMat");

        GameObject shadowPrefab = Resources.Load<GameObject>("ShadowPrefab");
        GameObject shadowObj = Instantiate(shadowPrefab, transform);
        shadowObj.transform.localPosition = Vector3.zero;
        shadowRenderer = shadowObj.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        lightTransform = LightManager.instance.FindClosestLight(transform.position);
        Vector2 lightDir = transform.position - lightTransform.position;
        spriteRenderer.sharedMaterial.SetVector("_LightDirection",lightDir);
        shadowRenderer.sharedMaterial.SetVector("_LightDirection", lightDir);
    }
}
