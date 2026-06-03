using UnityEditor.Rendering;
using UnityEngine;

//[ExecuteAlways]
public class CustomDynamicLit : MonoBehaviour
{
    [SerializeField] Sprite normalMap;
    [SerializeField] bool useAdditionalShadow;
    [SerializeField] Material shadowMat;
    [SerializeField] Sprite ShadowSprite;
    [SerializeField] float ambientShadowStrength;

    SpriteRenderer spriteRenderer;
    SpriteRenderer[] shadowRenderers;

    Material mat;

    Light[] affectingLights;

    float depth;

    bool visible = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = Resources.Load<Material>("CustomLitMat");

        depth = transform.position.y;
        spriteRenderer.sortingOrder = (int)depth;

        spriteRenderer.material.SetTexture("_NormalMap", normalMap.texture);

        mat = spriteRenderer.material;

        shadowRenderers = new SpriteRenderer[5];
        GameObject shadowPrefab = Resources.Load<GameObject>("ShadowPrefab");
        for (int i = 0; i < 1 || useAdditionalShadow && i<5; i++)
        {
            GameObject shadowObj = Instantiate(shadowPrefab, transform);
            shadowObj.transform.localPosition = Vector3.zero;
            SpriteRenderer shadowRenderer = shadowObj.GetComponent<SpriteRenderer>();
            shadowRenderer.material = shadowMat;
            shadowRenderer.sprite = ShadowSprite;
            shadowRenderers[i] = shadowRenderer;
        }
    }

    private void Start()
    {
        UpdateAmbientLight(LightManager.instance);
    }

    private void Update()
    {
        //Hide and unhide based on camera position
        Vector2 position = transform.position;
        Vector2 boundsBotLeft = LightManager.instance.boundsBotLeft;
        Vector2 boundsTopRight = LightManager.instance.boundsTopRight;

        if (position.x < boundsBotLeft.x || position.x > boundsTopRight.x || position.y < boundsBotLeft.y || position.y > boundsTopRight.y)
        {
            if(visible)
            {
                SetVisibility(false);
                visible = false;
            }
            return;
        }
        if (!visible)
        {
            SetVisibility(true);
            visible = true;
        }


        //passing light information
        affectingLights = LightManager.instance.FindAffectingLights(spriteRenderer.bounds.min, spriteRenderer.bounds.max);

        depth = transform.position.y;
        spriteRenderer.sortingOrder = (int)depth;
        mat.SetFloat("_Depth", depth);

        for (int i = 0; i < 4; i++)
        {
            if (affectingLights[i] != null)
            {
                mat.SetVector("_LightPosition" + i, affectingLights[i].lightPosition);
                mat.SetColor("_LightHue" + i, affectingLights[i].lightColor);
                mat.SetFloat("_LightRadius" + i, affectingLights[i].lightRadius);
                mat.SetFloat("_LightIntensity" + i, affectingLights[i].lightIntensity);

                if (useAdditionalShadow)
                {
                    Material currShadowMat = shadowRenderers[i+1].material;
                    currShadowMat.SetVector("_LightDirection", (Vector2)(transform.position - affectingLights[i].lightPosition));
                    currShadowMat.SetFloat("_LightIntensity", affectingLights[i].lightIntensity);
                    currShadowMat.SetFloat("_LightRadius", affectingLights[i].lightRadius);
                }
            }
        }


    }

    private void SetVisibility(bool visible)
    {
        spriteRenderer.enabled = visible;
        for (int i = 0; i < shadowRenderers.Length; i++)
        {
            shadowRenderers[i].enabled = visible;
        }
    }

    private void OnEnable()
    {
        LightManager.OnAmbientUpdate += UpdateAmbientLight;
    }

    private void OnDisable()
    {
        LightManager.OnAmbientUpdate -= UpdateAmbientLight;
    }

    public void UpdateAmbientLight(LightManager lightManager)
    {
        mat.SetFloat("_AmbientLightIntensity", lightManager.ambientLightIntensity);
        mat.SetColor("_AmbientLightColor", lightManager.ambientLightColor);

        Material ambientShadowMat = shadowRenderers[0].material;
        ambientShadowMat.SetVector("_LightDirection", lightManager.ambientLightDirection.normalized);
        ambientShadowMat.SetFloat("_LightRadius", ambientShadowStrength+1);
        ambientShadowMat.SetFloat("_LightIntensity", lightManager.ambientLightIntensity);
    }
}
