using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    [Tooltip("World Y offset applied after the vfx is spawned and rotated (can be keyframed in animations)")]
    [SerializeField] public float yOffset;

    //called by CharacterAnimator (animation event) during the wind animation
    public void SpawnWindVFX(ActionSO action) {
        if (action != null)
            SpawnVFX(action.windFeedback, transform.position);
    }

    //called by CharacterAnimator (animation event) during the attack animation
    public void SpawnAttackVFX(ActionSO action) {
        if (action != null)
            SpawnVFX(action.attackFeedback, transform.position);
    }

    //called by HitBox when it lands on a HurtBox (or parries), hitPoint is where it hit
    //otherEntities are the other characters involved, the vfx sorts in front of whichever one is most in front (including me)
    public void SpawnAttackHitVFX(ActionSO action, Vector3 hitPoint, params GameObject[] otherEntities) {
        if (action != null)
            SpawnVFX(action.attackHitFeedback, hitPoint, otherEntities);
    }

    void SpawnVFX(GameObject vfxPrefab, Vector3 position, params GameObject[] otherEntities) {
        if (vfxPrefab == null) return;//null feedback on the actionSO means no vfx

        Transform vfxTransform = Instantiate(vfxPrefab).transform;
        vfxTransform.position = position;

        if (vfxTransform.TryGetComponent(out VFX vfx) && vfx.doNotRotate)
        {
            vfx.SetDirection(GetDirection());
        }
        else
        {
            vfxTransform.rotation = transform.rotation;
        }

        vfxTransform.position += Vector3.up * yOffset;

        SortInFrontOfEntities(vfxTransform.gameObject, otherEntities);
    }

    //puts every particle system in the vfx 1 sorting order in front of the most in front entity
    void SortInFrontOfEntities(GameObject vfxObject, GameObject[] otherEntities)
    {
        Renderer frontRenderer = GetFrontRenderer(transform.root.gameObject);
        foreach (GameObject entity in otherEntities)
        {
            Renderer entityRenderer = GetFrontRenderer(entity);
            if (entityRenderer != null && (frontRenderer == null || entityRenderer.sortingOrder > frontRenderer.sortingOrder))
                frontRenderer = entityRenderer;
        }
        if (frontRenderer == null) return;

        ParticleSystemRenderer[] particleRenderers = vfxObject.GetComponentsInChildren<ParticleSystemRenderer>();
        if (particleRenderers.Length == 0) return;

        //keep the particle systems' order relative to each other, with the lowest one sitting 1 in front of the entity
        int lowestOrder = int.MaxValue;
        foreach (ParticleSystemRenderer particleRenderer in particleRenderers)
            lowestOrder = Mathf.Min(lowestOrder, particleRenderer.sortingOrder);

        foreach (ParticleSystemRenderer particleRenderer in particleRenderers)
        {
            particleRenderer.sortingLayerID = frontRenderer.sortingLayerID;
            particleRenderer.sortingOrder = frontRenderer.sortingOrder + 1 + (particleRenderer.sortingOrder - lowestOrder);
        }
    }

    //the most in front renderer driven by a DepthSort on this entity (ignores things like the shadow)
    Renderer GetFrontRenderer(GameObject entity)
    {
        if (entity == null) return null;

        Renderer frontRenderer = null;
        foreach (DepthSort depthSort in entity.GetComponentsInChildren<DepthSort>())
        {
            Renderer sortRenderer = depthSort.GetRenderer();
            if (sortRenderer != null && (frontRenderer == null || sortRenderer.sortingOrder > frontRenderer.sortingOrder))
                frontRenderer = sortRenderer;
        }
        return frontRenderer;
    }

    int GetDirection()//return 0-3 for up,right,down,left
    {
        Vector2 dir = -transform.up;
        if (Mathf.Abs(dir.x) > 0.3)//could be 0 instead of .3, its there in case we fuck up in the future
        {
            return Mathf.Sign(dir.x) == 1? 1 : 3;
        }
        return Mathf.Sign(dir.y)==1?0:2;
    }
}
