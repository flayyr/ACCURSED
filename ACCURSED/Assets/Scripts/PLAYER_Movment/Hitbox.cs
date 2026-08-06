using MoreMountains.Feedbacks;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] public Gradient damageColor;
    [SerializeField] public float damage;
    [SerializeField] public float knockBackPower;
    [SerializeField] public Vector3 direction;
    [SerializeField] public Vector3 PostAnimDirection;
    [Space]
    [SerializeField] private PlayerStatistics playerStats;
    [SerializeField] private MMF_Player hitFeedback;

    [HideInInspector]public GameObject originObject;
    CircleCollider2D c_cc;

    private void Start()
    {
        c_cc = GetComponent<CircleCollider2D>();
    }

    public Vector3 FindGlobalDirection()
    {
        PostAnimDirection = transform.TransformDirection(direction);
        return PostAnimDirection.normalized;
    }

    public void Hit()
    {
        if (playerStats != null)
        {
            playerStats.UpdateVitality(1);
        }
        if (hitFeedback != null)
        {
            hitFeedback.PlayFeedbacks();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + direction);
        Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);
    }
}
