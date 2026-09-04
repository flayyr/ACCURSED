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
    [Header("Parried")]
    [SerializeField] private float perfectParryStunDuration =1f;

    [HideInInspector]public GameObject originObject;

    CircleCollider2D c_cc;
    CharacterManager cManager;

    AttackData attackData;

    private void Start()
    {
        c_cc = GetComponent<CircleCollider2D>();
        cManager = GetComponentInParent<CharacterManager>();
        originObject = transform.root.gameObject;
    }

    public Vector3 FindGlobalDirection(Transform targetTransform)
    {
        if(direction == Vector3.zero) {
            return (targetTransform.position - transform.position).normalized;
        }
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

    public void Parried()
    {
        cManager.Stun(perfectParryStunDuration, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + direction);
        if(c_cc!=null)
        Gizmos.DrawWireSphere(transform.position, c_cc.radius);
    }

    public void SetAttackData(AttackData attackData) { this.attackData = attackData; }
    public AttackData GetAttackSO() { return attackData; }
    public void SetPlayerStats(PlayerStatistics playerStats) {  this.playerStats = playerStats; }
    public void SetHitFeedback(MMF_Player hitFeedback) {  this.hitFeedback = hitFeedback; }
    public void SetDirection(Vector2 direction) {  this.direction = direction; }
}
