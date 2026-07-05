using UnityEngine;

public class SpriteSinking : MonoBehaviour
{
    [SerializeField] LayerMask landCollider;
    [SerializeField] float maxDist =10f;

    Transform originTransform;
    Vector2 nearestPoint = new Vector2(999f,999f);

    Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private void Start()
    {
        originTransform = transform;
    }

    void Update()
    {
        Debug.Log( DistFromLand());
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
