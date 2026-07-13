using UnityEngine;
using UnityEngine.Tilemaps;

//[ExecuteAlways]
public class TileDynamicLit : MonoBehaviour
{
    [SerializeField] Sprite normalMap;

    TilemapRenderer tilemapRenderer;

    CustomLight[] affectingLights;

    Material mat;

    private void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapRenderer.material.SetTexture("_NormalMap", normalMap.texture);
        mat = tilemapRenderer.material;
    }

    //private void Update()
    //{
    //    //assigns the currently loaded lights into the material
    //    affectingLights = LightManager.instance.GetLoadedLights();

    //    for (int i = 0; i < 16; i++)
    //    {
    //        if (affectingLights[i] != null)
    //        {
    //            mat.SetVector("_LightPosition" + i, affectingLights[i].lightPosition);
    //            mat.SetColor("_LightHue" + i, affectingLights[i].lightColor);
    //            mat.SetFloat("_LightRadius" + i, affectingLights[i].lightRadius);
    //            mat.SetFloat("_LightIntensity" + i, affectingLights[i].lightIntensity);
    //        }
    //    }


    //}
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
    }
}
