using UnityEngine;

public class DepthSort : MonoBehaviour
{
    [SerializeField] int sortOrderOffset;
    [SerializeField] bool useParticle;
    [SerializeField] Transform baseTransform;

    Renderer sortRenderer;
    float depth;
    

    private void Awake()
    {
        if (useParticle)
        {
            sortRenderer = GetComponent<ParticleSystemRenderer>();
        }

        if (sortRenderer!=null)
        {
            SetUp(sortRenderer);
        }
    }

    public void SetUp(Renderer renderer)
    {
        sortRenderer = renderer;
        if (baseTransform == null)
        {
            baseTransform = transform;
        }

        UpdateSortOrder();
    }

    public float UpdateSortOrder()
    {
        depth = baseTransform.position.y * -10f;
        sortRenderer.sortingOrder = Mathf.RoundToInt(depth) + sortOrderOffset;
        return depth;
    }
}
