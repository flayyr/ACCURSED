using UnityEngine;

public class DepthSort : MonoBehaviour
{
    [SerializeField] Renderer sortRenderer;
    [SerializeField] bool useParticle;
    [SerializeField] Transform baseTransform;

    float depth;
    int sortOrderOffset;

    private void Awake()
    {
        if (useParticle)
        {
            sortRenderer = GetComponent<ParticleSystemRenderer>();
        }

        if (sortRenderer!=null)
        {
            SetUp(sortRenderer, 0);
        }
    }

    public void SetUp(Renderer renderer, int sortOrderOffset)
    {
        sortRenderer = renderer;
        this.sortOrderOffset = sortOrderOffset;
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
