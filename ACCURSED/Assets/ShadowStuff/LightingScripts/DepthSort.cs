using UnityEngine;

public class DepthSort : MonoBehaviour
{
    [SerializeField] Renderer sortRenderer;

    [SerializeField] int sortOrderOffset;
    [SerializeField] bool useParticle;
    [SerializeField] Transform baseTransform;
    [SerializeField] bool selfUpdate;
    
    float depth;

    bool isSetUp;

    private void Start()
    {
        if (useParticle)
        {
            sortRenderer = GetComponent<ParticleSystemRenderer>();
        }

        if (!isSetUp && sortRenderer!=null)
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

        isSetUp = true;

        return UpdateSortOrder();
    }

    public float SetUp(Renderer renderer, Transform refTransform)
    {
        sortRenderer = renderer;
        baseTransform = refTransform;

        isSetUp = true;

        return UpdateSortOrder();
    }

    public float UpdateSortOrder()
    {
        depth = baseTransform.position.y * -10f + sortOrderOffset;
        sortRenderer.sortingOrder = Mathf.RoundToInt(depth);
        return depth;
    }
}
