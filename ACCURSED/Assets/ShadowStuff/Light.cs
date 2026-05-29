using UnityEngine;

public class Light : MonoBehaviour
{
    public float lightRadius;
    public float lightIntensity;
    public Color lightColor;
    [HideInInspector]public Vector3 lightPosition;
    [Header("Parent")]
    [SerializeField] SpriteRenderer parentRenderer;

    void Update()
    {
        if (parentRenderer == null)
        {
            lightPosition = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
        else
        {
            lightPosition = new Vector3(transform.position.x, transform.position.y, parentRenderer.sortingOrder);
        }
    }
}
