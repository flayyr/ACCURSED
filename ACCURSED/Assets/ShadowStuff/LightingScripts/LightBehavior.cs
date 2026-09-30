using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    //[SerializeField] private Color lightColor;

    private Vector2 lightPosition;

    [Header("Parent")]
    [SerializeField] Transform refTransform;

    [SerializeField] public GPULight lightData;


    private void Awake()
    {
        //if(refTransform == null) 
            refTransform = transform;
        UpdateDepth();
    }

    float prevDepth;

    void Update()
    {
        //update depth value, may consider moving this out of update
        UpdateDepth();
    }

    private void UpdateDepth()
    {
        float depth = transform.position.y * -10f;
        if (depth != prevDepth)
        {
            prevDepth = depth;
            lightData.position = new Vector2(refTransform.position.x, refTransform.position.y);
            lightData.depth = depth;
        }
    }


}

//public class CustomLight
//{
//    public float lightRadius;
//    public float lightIntensity;
//    public Color lightColor;
//    [HideInInspector] public Vector3 lightPosition;

//    public CustomLight(float lightRadius, float lightIntensity, Color lightColor, Vector3 lightPosition, SpriteRenderer parentRenderer)
//    {
//        this.lightRadius = lightRadius;
//        this.lightIntensity = lightIntensity;
//        this.lightColor = lightColor;
//        this.lightPosition = lightPosition;
//    }

//    public CustomLight()
//    {
//        lightRadius = 0.01f;
//        lightIntensity = 0;
//        lightColor = Color.black;
//        lightPosition = Vector3.zero;
//    }
//}
