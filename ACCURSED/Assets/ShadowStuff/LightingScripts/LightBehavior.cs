using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    [SerializeField] private float lightRadius;
    [SerializeField] private float lightIntensity;
    [SerializeField] private Color lightColor;
    private Vector3 lightPosition;
    [Header("Parent")]
    [SerializeField] SpriteRenderer parentRenderer;

    [HideInInspector] public CustomLight lightData;

    SpriteRenderer spriteRenderer;
    int _DepthID;

    private void Awake()
    {
        lightData = new CustomLight(lightRadius, lightIntensity, lightColor, lightPosition, parentRenderer);
        spriteRenderer = GetComponent<SpriteRenderer>();
        _DepthID = Shader.PropertyToID("_Depth");
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
        float depth = (parentRenderer == null) ? transform.position.y * -10f : parentRenderer.sortingOrder;
        if (depth != prevDepth)
        {
            prevDepth = depth;
            lightData.lightPosition = new Vector3(transform.position.x, transform.position.y, depth);
            spriteRenderer.material.SetFloat(_DepthID, transform.position.y);
        }
    }

    private void OnValidate()
    {
        lightData = new CustomLight(lightRadius, lightIntensity, lightColor, lightPosition, parentRenderer);
    }


}

public class CustomLight
{
    public float lightRadius;
    public float lightIntensity;
    public Color lightColor;
    [HideInInspector] public Vector3 lightPosition;

    public CustomLight(float lightRadius, float lightIntensity, Color lightColor, Vector3 lightPosition, SpriteRenderer parentRenderer)
    {
        this.lightRadius = lightRadius;
        this.lightIntensity = lightIntensity;
        this.lightColor = lightColor;
        this.lightPosition = lightPosition;
    }

    public CustomLight()
    {
        lightRadius = 0.01f;
        lightIntensity = 0;
        lightColor = Color.black;
        lightPosition = Vector3.zero;
    }
}
