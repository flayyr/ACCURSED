using UnityEngine;

public class DepthSort : MonoBehaviour
{
    [SerializeField] Renderer sortRenderer;

    [SerializeField] int sortOrderOffset;
    [SerializeField] bool useParticle;
    [SerializeField] Transform baseTransform;
    [SerializeField] bool selfUpdate;
    
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

    private void Update()
    {
        if (selfUpdate)
        {
            UpdateSortOrder();
        }
    }

    public float SetUp(Renderer renderer)
    {
        sortRenderer = renderer;
        if (baseTransform == null)
        {
            baseTransform = transform;
        }

        return UpdateSortOrder();
    }

    public float SetUp(Renderer renderer, Transform refTransform)
    {
        sortRenderer = renderer;
        baseTransform = refTransform;

        return UpdateSortOrder();
    }

    public float UpdateSortOrder()
    {
        depth = baseTransform.position.y * -10f + sortOrderOffset;
        sortRenderer.sortingOrder = Mathf.RoundToInt(depth);
        return depth;
    }
}
