using MoreMountains.Feedbacks;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] public Vector3 direction;
    [SerializeField] public Vector3 PostAnimDirection;
    [Space]
    [SerializeField] private PlayerStatistics playerStats;
    [SerializeField] private MMF_Player hitFeedback;

    [HideInInspector]public GameObject originObject;
    CircleCollider2D c_cc;

    AttackData attackData;

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
            playerStats.UpdateVitality(attackData.vitalityBuildUp);
            playerStats.UpdateRemembranceCharge(attackData.vitalityBuildUp);
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
        if(GetComponent<CircleCollider2D>()!=null)
        Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);
    }

    public void SetAttackData(AttackData attackData) { this.attackData = attackData; }
    public AttackData GetAttackSO() { return attackData; }
    public void SetPlayerStats(PlayerStatistics playerStats) {  this.playerStats = playerStats; }
    public void SetHitFeedback(MMF_Player hitFeedback) {  this.hitFeedback = hitFeedback; }
    public void SetDirection(Vector2 direction) {  this.direction = direction; }
}
