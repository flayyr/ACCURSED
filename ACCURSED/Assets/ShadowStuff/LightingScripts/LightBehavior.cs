using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    [SerializeField] private float lightRadius;
    [SerializeField] private float lightIntensity;
    [SerializeField] private Color lightColor;
    private Vector3 lightPosition;
    [Header("Parent")]
    [SerializeField] SpriteRenderer parentRenderer;

    [HideInInspector] public Light lightData;

    private void Awake()
    {
        lightData = new Light(lightRadius, lightIntensity, lightColor, lightPosition, parentRenderer);
    }

    void Update()
    {
        if (parentRenderer == null)
        {
            lightData.lightPosition = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
        else
        {
            lightData.lightPosition = new Vector3(transform.position.x, transform.position.y, parentRenderer.sortingOrder);
        }
    }

    private void OnValidate()
    {
        lightData = new Light(lightRadius, lightIntensity, lightColor, lightPosition, parentRenderer);
    }


}

public class Light
{
    public float lightRadius;
    public float lightIntensity;
    public Color lightColor;
    [HideInInspector] public Vector3 lightPosition;

    public Light(float lightRadius, float lightIntensity, Color lightColor, Vector3 lightPosition, SpriteRenderer parentRenderer)
    {
        this.lightRadius = lightRadius;
        this.lightIntensity = lightIntensity;
        this.lightColor = lightColor;
        this.lightPosition = lightPosition;
    }

    public Light()
    {
        lightRadius = 0;
        lightIntensity = 0;
        lightColor = Color.black;
        lightPosition = Vector3.zero;
    }
}
