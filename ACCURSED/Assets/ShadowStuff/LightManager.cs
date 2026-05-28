using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//[ExecuteAlways]
public class LightManager : MonoBehaviour
{
    public static LightManager instance;

    [HideInInspector]public Light2D[] lights;

    private void Awake()
    {
        instance = this;
        //lights = new List<Light2D>();
    }

    private void Start()
    {
        lights =  FindObjectsByType<Light2D>(FindObjectsSortMode.None);
    }

    //uncomment if running during edit mode
    /*
    private void Update()
    {
        instance = this;
        lights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
    }*/ 

    public Transform FindClosestLight(Vector3 targetPosition)
    {
        float closestDist = float.MaxValue;
        Transform output = lights[0].transform;
        foreach(Light2D light in lights) {
            float dist = (light.transform.position - targetPosition).magnitude;
            if (dist<closestDist)
            {
                output = light.transform;
                closestDist = dist;
            }
        }
        return output;
    }


}
