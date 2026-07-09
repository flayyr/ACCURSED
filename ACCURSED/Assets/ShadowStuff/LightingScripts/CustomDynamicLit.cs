using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.U2D;

//[ExecuteAlways]
[RequireComponent(typeof(ReflectionSprite)), RequireComponent(typeof(DepthSort))]
public class CustomDynamicLit : MonoBehaviour
{
    [Header("Culling")]
    [SerializeField] bool canMove = false;
    [Header("Normal Map")]
    [SerializeField] Sprite normalMap;
    [Header("Ref Transform")]
    [SerializeField] Transform refTransform;
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
    GameObject shadowPrefab;

    CustomLight[] affectingLights;

    DepthSort depthSorter;
    float depth;

    bool visible = true;

    //material ids to replace strings
    int[,] _LightIDs = new int[4,4];
    int _Depth = Shader.PropertyToID("_Depth");

    //MaterialPropertyBlock litMatProperty;
    Material mat;


    private void Awake()
    {
        if (refTransform == null)
        {
            refTransform = transform;
        }

        SetUpIDs();
        SetUpLitMat();
        SetUpSort();

        affectingLights = new CustomLight[4];
    }

    private void SetUpIDs()
    {
        for (int i = 0; i < 4; i++)
        {
            _LightIDs[i,0] = Shader.PropertyToID("_LightPosition" + i);
            _LightIDs[i, 1] = Shader.PropertyToID("_LightHue" + i);
            _LightIDs[i, 2] = Shader.PropertyToID("_LightRadius" + i);
            _LightIDs[i, 3] = Shader.PropertyToID("_LightIntensity" + i);
        }
    }

    public void SetUpLitMat()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = Resources.Load<Material>("CustomLitMat");

        //litMatProperty = new MaterialPropertyBlock();
        //spriteRenderer.GetPropertyBlock(litMatProperty);

        mat = spriteRenderer.material;

        if (useWind)
        {
            spriteRenderer.material.EnableKeyword("_USEWIND");
            mat.SetFloat("_WindStrength", windStrength);
        }
        if (topSway)
        {
            spriteRenderer.material.EnableKeyword("_TOPSWAY");
            mat.SetFloat("_TopSwayStrength", topSwayStrength);
            mat.SetFloat("_SwayOffset", refTransform.position.y * refTransform.position.y + refTransform.position.x);
        }

        spriteRenderer.material.SetTexture("_NormalMap", normalMap == null ? Resources.Load<Texture2D>("DefaultNormal") : normalMap.texture);

        Sprite sprite = spriteRenderer.sprite;
        float baseSpriteHeight = sprite.textureRect.yMin / sprite.texture.height;
        float totalSpriteHeight = (sprite.textureRect.yMax / sprite.texture.height) - baseSpriteHeight;
        mat.SetFloat("_SpriteStartHeight", baseSpriteHeight);
        mat.SetFloat("_SpriteTotalHeight", totalSpriteHeight);
        mat.SetFloat("_TextureSize", 0.0001f * Mathf.Sqrt(sprite.texture.height * sprite.texture.height + sprite.texture.width * sprite.texture.width));

        //spriteRenderer.SetPropertyBlock(litMatProperty);


        //Set up shadows
        if (shadowMat == null)
        {
            return;
        }

        shadowRenderers = new SpriteRenderer[4];
        shadowPrefab = Resources.Load<GameObject>("ShadowPrefab");
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
                ambientShadowRenderer.material.SetFloat("_SwayOffset", refTransform.position.y * refTransform.position.y + refTransform.position.x);
                ambientShadowRenderer.material.SetFloat("_SpriteStartHeight", baseSpriteHeight);
                ambientShadowRenderer.material.SetFloat("_SpriteTotalHeight", totalSpriteHeight);
                ambientShadowRenderer.material.SetFloat("_TopSwayFallOff", spriteRenderer.material.GetFloat("_TopSwayFallOff"));
                ambientShadowRenderer.material.SetFloat("_TopSwaySpeed", spriteRenderer.material.GetFloat("_TopSwaySpeed"));
            }
        }
    }

    private void Start()
    {
        //in Start because instances are assigned in Awake
        if (!canMove)
        {
            CullManager.instance.AddObject(this, refTransform.position);
        }
        
        UpdateAmbientLight(LightManager.instance);
    }

    private void Update()
    {
        //passing light information
        affectingLights = LightManager.instance.FindAffectingLights(spriteRenderer.bounds.min, spriteRenderer.bounds.max);

        if (canMove)
        {
            depth = depthSorter.UpdateSortOrder();
            mat.SetFloat(_Depth, depth);
        }

        if (canMove && useAmbientShadow && shadowMat!=null)
        {
            ambientShadowRenderer.sortingOrder = spriteRenderer.sortingOrder;
        }

        for (int i = 0; i < 4; i++)
        {
            if (affectingLights[i] != null)
            {
                //mat.SetVector(_LightIDs[i, 0], affectingLights[i].lightPosition);
                //mat.SetColor(_LightIDs[i, 1], affectingLights[i].lightColor);
                //mat.SetFloat(_LightIDs[i, 2], affectingLights[i].lightRadius);
                //mat.SetFloat(_LightIDs[i, 3], affectingLights[i].lightIntensity);

                if (useAdditionalShadow && shadowMat != null)
                {
                    CastNewShadow(i);

                    shadowRenderers[i].sortingOrder = spriteRenderer.sortingOrder;
                    Material currShadowMat = shadowRenderers[i].material;
                    currShadowMat.SetVector("_LightDirection", (Vector2)(refTransform.position - affectingLights[i].lightPosition));
                    currShadowMat.SetFloat("_LightIntensity", affectingLights[i].lightIntensity);
                    currShadowMat.SetFloat("_LightRadius", affectingLights[i].lightRadius);
                }
            }
            else
            {
                if (useAdditionalShadow && shadowMat != null)
                {
                    DestroyShadow(i);
                }
            }
        }

        //spriteRenderer.SetPropertyBlock(litMatProperty);


    }

    void CastNewShadow(int index)
    {
        if (shadowRenderers[index] != null)
        {
            return;
        }

        GameObject shadowObj = Instantiate(shadowPrefab, transform);
        shadowObj.transform.localPosition = Vector3.zero;
        SpriteRenderer shadowRenderer = shadowObj.GetComponent<SpriteRenderer>();
        shadowRenderer.material = shadowMat;
        shadowRenderer.sprite = ShadowSprite;
        shadowRenderer.material.SetVector("_ShadowScale", shadowSize);
        shadowRenderer.sortingOrder = spriteRenderer.sortingOrder;
        shadowRenderers[index] = shadowRenderer;
    }

    void DestroyShadow(int index)
    {
        if (shadowRenderers[index] != null)
        {
            Destroy(shadowRenderers[index].gameObject);
        }
    }

    //private void OnValidate()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //    depth = depthSorter.UpdateSortOrder();
    //}

    [ContextMenu("Update Sorting")]
    [ExecuteAlways]
    private void SetUpSort()
    {
        depthSorter = GetComponent<DepthSort>();
        depthSorter.SetUp(spriteRenderer);
    }

    public void SetVisibility(bool visible)
    {
        //spriteRenderer.enabled = visible;
        if (shadowMat != null)
        {
            for (int i = 0; i < shadowRenderers.Length; i++)
            {
                if(shadowRenderers[i] != null)
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

        if (shadowMat != null && useAmbientShadow)
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
