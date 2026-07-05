using UnityEngine;

public class SpriteSinking : MonoBehaviour
{
    [SerializeField] LayerMask landCollider;
    [SerializeField] float maxDist =10f;
    [SerializeField] float spriteBaseY = 0;
    [SerializeField] float maxSinkAmount = 1f;
    [SerializeField] WaterTrail waterTrail;

    Transform originTransform;
    Vector2 nearestPoint = new Vector2(999f,999f);

    Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    float sinkAmount;

    private void Start()
    {
        if(originTransform== null)
            originTransform = transform;
    }

    void Update()
    {
        if (waterTrail.inWater)
        {
            float dist = DistFromLand();
            sinkAmount = (dist / maxDist) * maxSinkAmount;
            originTransform.localPosition = new Vector3(originTransform.localPosition.x, spriteBaseY-sinkAmount, 0);
        }
        else
        {
            if(sinkAmount > 0)
            {
                sinkAmount = 0;
                originTransform.localPosition = new Vector3(originTransform.localPosition.x, spriteBaseY, 0);
            }
        }
        
    }

    private float DistFromLand()
    {
        float nearestDist = ((Vector2)originTransform.position - nearestPoint).magnitude;

        for(int i=0; i<directions.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(originTransform.position, directions[i], maxDist, landCollider);
            if(hit.collider != null)
            {
                if (hit.distance < nearestDist)
                {
                    nearestDist = hit.distance;
                    nearestPoint = hit.point;
                }
            }
        }

        return Mathf.Min( nearestDist, maxDist);
    }
}
