using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] private Vector3 direction;
    [SerializeField] public Vector3 PostAnimDirection;
    [Space]
    [SerializeField] private PlayerStatistics playerStats;
    [Header("Parried")]
    [SerializeField] private float parryStunDuration = 1f;
    [Header("Destroy on hit")]
    [SerializeField] bool destroyOnHit;

    [Header("VFX")]
    [Tooltip("Spawner for attack hit vfx, auto-found on a sibling if left empty")]
    [SerializeField] private VFXSpawner vfxSpawner;

    [HideInInspector] public GameObject originObject;

    CircleCollider2D c_cc;
    CharacterManager cManager;

    AttackData attackData;
    ActionSO action;

    private void Start()
    {
        c_cc = GetComponent<CircleCollider2D>();
        cManager = GetComponentInParent<CharacterManager>();
        originObject = transform.root.gameObject;

        if (vfxSpawner == null && transform.parent != null)
            vfxSpawner = transform.parent.GetComponentInChildren<VFXSpawner>();
    }

    public Vector3 FindGlobalDirection(Transform targetTransform)
    {
        if (direction == Vector3.zero)
        {
            return (targetTransform.position - transform.position).normalized;
        }
        PostAnimDirection = transform.TransformDirection(direction);
        return PostAnimDirection.normalized;
    }

    public BuffData GetBuffs(HurtBox hurtbox) {
        if (attackData.buffData.type == BuffType.None) return null;
        if (hurtbox.buffHurtboxType != attackData.buffData.target) return null;
        return attackData.buffData;
    }

    //called by HurtBox when this hitbox lands, hitPoint is where it hit, victim is the character that got hit
    public void Hit(HurtBox hurtbox ,Vector3 hitPoint, GameObject victim)
    {
        if (playerStats != null)
        {
            playerStats.UpdateVitality(attackData.vitalityBuildUp);
            playerStats.UpdateRemembranceCharge(attackData.vitalityBuildUp);
        }

        if (vfxSpawner != null)
            vfxSpawner.SpawnAttackHitVFX(action, hitPoint, victim);

        if (destroyOnHit) {
            Destroy(transform.root.gameObject);
        }
    }

    public void Parried()
    {
        if(cManager != null)
            cManager.Stun(parryStunDuration, 0f);

        if (destroyOnHit) {
            Destroy(transform.root.gameObject);
        }
    }

    //called by my own HurtBox when it successfully parries, hitPoint is where the parry vfx goes, attacker is the character that got parried
    public void ParrySuccess(Vector3 hitPoint, GameObject attacker)
    {
        if (vfxSpawner != null)
            vfxSpawner.SpawnAttackHitVFX(action, hitPoint, attacker);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + direction);
        if (c_cc != null)
            Gizmos.DrawWireSphere(transform.position, c_cc.radius);
    }

    public void SetAttackData(AttackData attackData) { this.attackData = attackData; }
    public void SetAction(ActionSO action) { this.action = action; }
    public AttackData GetAttackSO() { return attackData; }
    public void SetPlayerStats(PlayerStatistics playerStats) { this.playerStats = playerStats; }
    public void SetDirection(Vector2 direction) { this.direction = direction; }
    public void SetVFXSpawner(VFXSpawner vfxSpawner) { this.vfxSpawner = vfxSpawner; }
}
