using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public static LightManager instance;
    public static event Action<LightManager> OnAmbientUpdate;

    [SerializeField] Camera cam;
    [SerializeField] float lightCullBuffer;
    [SerializeField] float cullInterval;
    [Header("Ambient Light")]
    [SerializeField, Range(0,1)] public float ambientLightIntensity;
    [SerializeField] public Color ambientLightColor;
    [SerializeField] public Vector2 ambientLightDirection;

    [HideInInspector] public Vector2 boundsBotLeft;
    [HideInInspector] public Vector2 boundsTopRight;

    private List<LightBehavior> unusedLightBehaviors;
    private List<LightBehavior> lightBehaviors;


    private float cullTimer;

    private void Awake()
    {
        instance = this;
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
        if (cam == null)
        {
            return;
        }

        //only run rest of script every cullInterval seconds
        if (cullTimer > 0)
        {
            cullTimer-=Time.deltaTime;
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
            Vector2 lightPos = lightBehavior.lightData.lightPosition;

            if (lightPos.x > boundsBotLeft.x && lightPos.x<boundsTopRight.x && lightPos.y>boundsBotLeft.y && lightPos.y<boundsTopRight.y)
            {
                addToUsed.Enqueue(lightBehavior);
            }
        }

        Queue<LightBehavior> addToUnused = new Queue<LightBehavior>();

        foreach (LightBehavior lightBehavior in lightBehaviors)
        {
            Vector2 lightPos = lightBehavior.lightData.lightPosition;

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
    }

    private void OnValidate()
    {
        OnAmbientUpdate?.Invoke(this);
    }

    //For Lit objects
    public CustomLight[] FindAffectingLights(Vector3 minBound, Vector3 maxBound)
    {
        CustomLight[] output = new CustomLight[4];
        int count = 0;

        foreach (LightBehavior lightBehavior in lightBehaviors)
        {
            CustomLight light = lightBehavior.lightData;

            float closestX = Mathf.Max(minBound.x, Mathf.Min( light.lightPosition.x, maxBound.x));
            float closestY = Mathf.Max(minBound.y, Mathf.Min(light.lightPosition.y, maxBound.y));
            
            float distSquared = Mathf.Pow( light.lightPosition.x-closestX,2) + Mathf.Pow(light.lightPosition.y-closestY,2);

            if(distSquared< light.lightRadius * light.lightRadius)
            {
                output[count] = light;
                count++;
                if (count == 4)
                {
                    return output;
                }
            }
        }

        //fill the array with empty lights if there are less than 4 affecting lights
        CustomLight newLight = new CustomLight();
        
        for(int i = count; i<4; i++)
        {
            output[i] = newLight;
        }
        return output;
    }

    //For tilemap, returns all loaded lights (up to 16)
    public CustomLight[] GetLoadedLights()
    {
        CustomLight[] output = new CustomLight[16];
        CustomLight newLight = new CustomLight();
        for (int i = 0; i<16; i++)
        {
            if (i < lightBehaviors.Count)
            {
                output[i] = lightBehaviors[i].lightData;
            }
            else
            {
                output[i] = new CustomLight();
            }
        }
        return output;
    }


}
