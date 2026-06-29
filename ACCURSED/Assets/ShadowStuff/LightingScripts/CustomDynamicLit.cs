using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.U2D;

//[ExecuteAlways]
[RequireComponent(typeof(ReflectionSprite))]
public class CustomDynamicLit : MonoBehaviour
{
    [Header("Normal Map")]
    [SerializeField] Sprite normalMap;
    [Header("Ordering")]
    [SerializeField] int sortOrderOffset;
    [Header("Ambient Shadow")]
    [SerializeField] bool useAmbientShadow = true;
    [SerializeField] Sprite ambientShadowSprite;
    [SerializeField] float ambientShadowLength = 0.5f;
    [Header("Additional Shadow")]
    [SerializeField] bool useAdditionalShadow;
    [SerializeField] Material shadowMat;
    [SerializeField] Sprite ShadowSprite;
    [SerializeField] Vector2 shadowSize = Vector2.one;
    [Header("Wind")]
    [SerializeField] public bool useWind;
    [SerializeField, Range(0f, 0.05f)] float windStrength = 0.006f;
    [SerializeField] bool topSway;
    [SerializeField] float topSwayStrength = 0.1f;

    SpriteRenderer spriteRenderer;
    SpriteRenderer[] shadowRenderers;
    SpriteRenderer ambientShadowRenderer;

    Material mat;

    CustomLight[] affectingLights;

    float depth;

    bool visible = true;

    private void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = Resources.Load<Material>("CustomLitMat");
        if (useWind)
        {
            spriteRenderer.material.EnableKeyword("_USEWIND");
            spriteRenderer.material.SetFloat("_WindStrength", windStrength);
        }
        if (topSway)
        {
            spriteRenderer.material.EnableKeyword("_TOPSWAY");
            spriteRenderer.material.SetFloat("_TopSwayStrength", topSwayStrength);
            spriteRenderer.material.SetFloat("_SwayOffset", transform.position.y * transform.position.y + transform.position.x);
        }
        mat = spriteRenderer.material;

        UpdateSortOrder();

        spriteRenderer.material.SetTexture("_NormalMap", normalMap.texture);

        Sprite sprite = spriteRenderer.sprite;
        float baseSpriteHeight = sprite.textureRect.yMin / sprite.texture.height;
        float totalSpriteHeight = (sprite.textureRect.yMax / sprite.texture.height) - baseSpriteHeight;
        spriteRenderer.material.SetFloat("_SpriteStartHeight", baseSpriteHeight);
        spriteRenderer.material.SetFloat("_SpriteTotalHeight", totalSpriteHeight);
        spriteRenderer.material.SetFloat("_TextureSize", 0.0001f * Mathf.Sqrt(sprite.texture.height * sprite.texture.height + sprite.texture.width * sprite.texture.width));

        if (shadowMat == null)
        {
            return;
        }

        shadowRenderers = new SpriteRenderer[4];
        GameObject shadowPrefab = Resources.Load<GameObject>("ShadowPrefab");
        if (useAmbientShadow)
        {
            GameObject shadowObj = Instantiate(shadowPrefab, transform);
            shadowObj.transform.localPosition = Vector3.zero;
            ambientShadowRenderer = shadowObj.GetComponent<SpriteRenderer>();
            ambientShadowRenderer.material = Resources.Load<Material>("SkewShadowMat");
            ambientShadowRenderer.material.SetFloat("_ShadowLength", ambientShadowLength);
            ambientShadowRenderer.sprite = ambientShadowSprite != null ? ambientShadowSprite : spriteRenderer.sprite;
            ambientShadowRenderer.sortingLayerName = "AmbientShadow";
            ambientShadowRenderer.sortingOrder = spriteRenderer.sortingOrder;
            if (topSway)
            {
                ambientShadowRenderer.material.EnableKeyword("_TOPSWAY");
                ambientShadowRenderer.material.SetFloat("_TopSwayStrength", topSwayStrength);
                ambientShadowRenderer.material.SetFloat("_SwayOffset", transform.position.y * transform.position.y + transform.position.x);
                ambientShadowRenderer.material.SetFloat("_SpriteStartHeight", baseSpriteHeight);
                ambientShadowRenderer.material.SetFloat("_SpriteTotalHeight", totalSpriteHeight);
                ambientShadowRenderer.material.SetFloat("_TopSwayFallOff", spriteRenderer.material.GetFloat("_TopSwayFallOff"));
                ambientShadowRenderer.material.SetFloat("_TopSwaySpeed", spriteRenderer.material.GetFloat("_TopSwaySpeed"));
            }
        }


        if (useAdditionalShadow)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject shadowObj = Instantiate(shadowPrefab, transform);
                shadowObj.transform.localPosition = Vector3.zero;
                SpriteRenderer shadowRenderer = shadowObj.GetComponent<SpriteRenderer>();
                shadowRenderer.material = shadowMat;
                shadowRenderer.sprite = ShadowSprite;
                shadowRenderer.material.SetVector("_ShadowScale", shadowSize);
                shadowRenderer.sortingOrder = spriteRenderer.sortingOrder;
                shadowRenderers[i] = shadowRenderer;
            }
        }
    }

    private void Start()
    {
        //in Start because LightManager instance is assigned in Awake
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

        UpdateSortOrder();
        mat.SetFloat("_Depth", depth);

        if (useAmbientShadow && shadowMat!=null)
        {
            ambientShadowRenderer.sortingOrder = spriteRenderer.sortingOrder;
        }

        for (int i = 0; i < 4; i++)
        {
            if (affectingLights[i] != null)
            {
                mat.SetVector("_LightPosition" + i, affectingLights[i].lightPosition);
                mat.SetColor("_LightHue" + i, affectingLights[i].lightColor);
                mat.SetFloat("_LightRadius" + i, affectingLights[i].lightRadius);
                mat.SetFloat("_LightIntensity" + i, affectingLights[i].lightIntensity);

                if (useAdditionalShadow && shadowMat != null && affectingLights[i].lightIntensity!=0f)
                {
                    shadowRenderers[i].sortingOrder = spriteRenderer.sortingOrder;
                    Material currShadowMat = shadowRenderers[i].material;
                    currShadowMat.SetVector("_LightDirection", (Vector2)(transform.position - affectingLights[i].lightPosition));
                    currShadowMat.SetFloat("_LightIntensity", affectingLights[i].lightIntensity);
                    currShadowMat.SetFloat("_LightRadius", affectingLights[i].lightRadius);
                }
            }
        }


    }

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSortOrder();
    }

    [ContextMenu("Execute Function")]
    [ExecuteAlways]
    private void UpdateSortOrder()
    {
        depth = transform.position.y * -10f;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(depth) + sortOrderOffset;
    }

    private void SetVisibility(bool visible)
    {
        //spriteRenderer.enabled = visible;
        if (shadowMat != null)
        {
            for (int i = 0; i < shadowRenderers.Length; i++)
            {
                shadowRenderers[i].enabled = visible;
            }
        }
        //if(ambientShadowRenderer!=null)
        //ambientShadowRenderer.enabled = visible;
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
        mat.SetColor("_AmbientShadowColor", lightManager.ambientShadowColor);

        if (shadowMat != null)
        {
            Material ambientShadowMat = ambientShadowRenderer.material;
            ambientShadowMat.SetFloat("_SkewAmount", lightManager.ambientShadowSkew);
            ambientShadowMat.SetFloat("_ShadowStrength", lightManager.ambientShadowStrength);
            ambientShadowMat.SetFloat("_FlipY", lightManager.ambientShadowFlipY?1f:0f);
        }
    }

    public void CopyValues(CustomDynamicLit other)
    {
        ambientShadowLength = other.ambientShadowLength;
        //shadowMat;
        //ShadowSprite;
        shadowSize = other.shadowSize;
        useWind = other.useWind;
        windStrength = other.windStrength;
        topSway = other.topSway;
        topSwayStrength = other.topSwayStrength;
    }
}
