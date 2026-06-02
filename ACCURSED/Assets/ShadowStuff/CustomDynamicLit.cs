using UnityEditor.Rendering;
using UnityEngine;

//[ExecuteAlways]
public class CustomDynamicLit : MonoBehaviour
{
    [SerializeField] Light testLight;

    Transform lightTransform;
    SpriteRenderer spriteRenderer;
    SpriteRenderer shadowRenderer;

    float depth;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = Resources.Load<Material>("CustomLitMat");

        //GameObject shadowPrefab = Resources.Load<GameObject>("ShadowPrefab");
        //GameObject shadowObj = Instantiate(shadowPrefab, transform);
        //shadowObj.transform.localPosition = Vector3.zero;
        //shadowRenderer = shadowObj.GetComponent<SpriteRenderer>();

        depth = transform.position.y;
        spriteRenderer.sortingOrder = (int)depth;
    }

    private void Update()
    {
        //lightTransform = LightManager.instance.FindClosestLight(transform.position);
        //Vector2 lightDir = transform.position - lightTransform.position;
        //shadowRenderer.sharedMaterial.SetVector("_LightDirection", lightDir);

        depth = transform.position.y;

        spriteRenderer.sharedMaterial.SetVector("_LightPosition", testLight.lightPosition);
        spriteRenderer.sharedMaterial.SetColor("_LightColor", testLight.lightColor);
        spriteRenderer.sharedMaterial.SetFloat("_LightRadius", testLight.lightRadius);
        spriteRenderer.sharedMaterial.SetFloat("_LightIntensity", testLight.lightIntensity);
    }
}
