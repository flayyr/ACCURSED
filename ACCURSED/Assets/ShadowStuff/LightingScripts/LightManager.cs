using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[StructLayout(LayoutKind.Sequential)]
public struct GPULight
{
    public Vector2 position;
    public float depth;
    public float radius;
    public float intensity;
    public Vector3 color;

    public GPULight(Vector2 position, float depth,float radius, float intensity, Color color)
    {
        this.position = position;
        this.depth = depth;
        this.radius = radius;
        this.intensity = intensity;
        this.color = new Vector3(color.r,color.g,color.b);
    }
}

public class LightManager : MonoBehaviour
{
    public static LightManager instance;
    public static event Action<LightManager> OnAmbientUpdate;
    private static int MAX_LIGHTS = 16;

    [SerializeField] public bool useCircleShadow;
    [Space]

    [SerializeField] Camera cam;
    [SerializeField] RenderTexture lightTexture;
    [SerializeField] Material lightMaterial;

    [Space]
    [SerializeField] float lightCullBuffer;
    [SerializeField] float cullInterval;
    [Header("Ambient Light")]
    [SerializeField, Range(0,1)] public float ambientLightIntensity;
    [SerializeField] public Color ambientLightColor;
    [SerializeField] public Color ambientShadowColor;
    [SerializeField] public float ambientShadowSkew;
    [SerializeField] public bool ambientShadowFlipY;
    [SerializeField, Range(0,1)] public float ambientShadowStrength=0.1f;

    //[SerializeField] Material[] materialRefs;

    [HideInInspector] public Vector2 boundsBotLeft;
    [HideInInspector] public Vector2 boundsTopRight;

    private List<LightBehavior> unusedLightBehaviors;
    private List<LightBehavior> lightBehaviors;
    private GPULight[] visibleLights;


    private float cullTimer;

    private GraphicsBuffer lightBuffer;

    private void Awake()
    {
        instance = this;
        lightBuffer = new GraphicsBuffer(
            GraphicsBuffer.Target.Structured,
            MAX_LIGHTS,
            Marshal.SizeOf<GPULight>());
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        unusedLightBehaviors = new List<LightBehavior>();
        LightBehavior[] lightBehaviorsArray =  FindObjectsByType<LightBehavior>(FindObjectsSortMode.None);
        lightBehaviors = new List<LightBehavior>(lightBehaviorsArray);
        cullTimer = 0;
        OnAmbientUpdate?.Invoke(this);
    }

    private void Update()
    {
        {
            if (cam == null)
            {
                return;
            }

            //only run rest of script every cullInterval seconds
            if (cullTimer > 0)
            {
                cullTimer -= Time.deltaTime;
                return;
            }
            cullTimer = cullInterval;

            //calculate camera bounding box
            boundsBotLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            boundsTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            boundsBotLeft -= Vector2.one * lightCullBuffer;
            boundsTopRight += Vector2.one * lightCullBuffer;



            //badly written code that disables/enables light based on camera bounds
            Queue<LightBehavior> addToUsed = new Queue<LightBehavior>();

            foreach (LightBehavior lightBehavior in unusedLightBehaviors)
            {
                Vector2 lightPos = lightBehavior.lightData.position;

                if (lightPos.x > boundsBotLeft.x && lightPos.x < boundsTopRight.x && lightPos.y > boundsBotLeft.y && lightPos.y < boundsTopRight.y)
                {
                    addToUsed.Enqueue(lightBehavior);
                }
            }

            Queue<LightBehavior> addToUnused = new Queue<LightBehavior>();

            foreach (LightBehavior lightBehavior in lightBehaviors)
            {
                Vector2 lightPos = lightBehavior.lightData.position;

                if (lightPos.x < boundsBotLeft.x || lightPos.x > boundsTopRight.x || lightPos.y < boundsBotLeft.y || lightPos.y > boundsTopRight.y)
                {
                    addToUnused.Enqueue(lightBehavior);
                }
            }

            while (addToUsed.Count > 0)
            {
                LightBehavior curr = addToUsed.Dequeue();
                curr.enabled = true;
                lightBehaviors.Add(curr);
                unusedLightBehaviors.Remove(curr);

            }
            while (addToUnused.Count > 0)
            {
                LightBehavior curr = addToUnused.Dequeue();
                curr.enabled = false;
                unusedLightBehaviors.Add(curr);
                lightBehaviors.Remove(curr);
            }

            visibleLights = new GPULight[lightBehaviors.Count];
            for (int i = 0; i < visibleLights.Length; i++)
            {
                visibleLights[i] = lightBehaviors[i].lightData;
            }
        }




        lightBuffer.SetData(visibleLights);

        lightMaterial.SetBuffer("_Lights", lightBuffer);

        lightMaterial.SetInt("_LightCount", visibleLights.Length);

        boundsBotLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        boundsTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        lightMaterial.SetVector("_CameraMin", boundsBotLeft);

        lightMaterial.SetVector("_CameraSize", boundsTopRight - boundsBotLeft);

        Graphics.Blit(null, lightTexture, lightMaterial);


    }

    private void OnValidate()
    {
        OnAmbientUpdate?.Invoke(this);
    }

    //For Lit objects
    public GPULight[] FindAffectingLights(Vector3 minBound, Vector3 maxBound)
    {
        Queue<GPULight> tempQueue = new Queue<GPULight>();
        int count = 0;

        foreach (LightBehavior lightBehavior in lightBehaviors)
        {
            GPULight light = lightBehavior.lightData;

            float closestX = Mathf.Max(minBound.x, Mathf.Min(light.position.x, maxBound.x));
            float closestY = Mathf.Max(minBound.y, Mathf.Min(light.position.y, maxBound.y));

            float distSquared = Mathf.Pow(light.position.x - closestX, 2) + Mathf.Pow(light.position.y - closestY, 2);

            if (distSquared < light.radius * light.radius)
            {
                tempQueue.Enqueue( light);
                count++;
            }
        }

        GPULight[] output = new GPULight[count];

        for (int i = 0; i<count; i++)
        {
            output[i] = tempQueue.Dequeue();
        }

        return output;
    }

    //For tilemap, returns all loaded lights (up to 16)
    //public CustomLight[] GetLoadedLights()
    //{
    //    CustomLight[] output = new CustomLight[16];
    //    CustomLight newLight = new CustomLight();
    //    //for (int i = 0; i<16; i++)
    //    //{
    //    //    output[i] = new CustomLight();
    //    //    continue;

    //    //    //if (i < lightBehaviors.Count)
    //    //    //{
    //    //    //    output[i] = lightBehaviors[i].lightData;
    //    //    //}
    //    //    //else
    //    //    //{
    //    //    //    output[i] = new CustomLight();
    //    //    //}
    //    //}
    //    return output;
    //}


}
