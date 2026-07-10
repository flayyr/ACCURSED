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
