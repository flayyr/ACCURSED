using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MoreMountains.Feedbacks;
using static UnityEngine.Rendering.DebugUI;
using NUnit.Framework;

public class HurtBox : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] LayerMask lm_hitbox;
    [SerializeField] LayerMask lm_buffSource;
    [SerializeField] public BuffTarget buffHurtboxType;

    [Space]
    [SerializeField] float afterHurtInvincibleDuration = 0.2f;
    [SerializeField] float durationUntilDodgeCancellable = 0.5f;

    [SerializeField] List<GameObject> personalHurtBoxes = new List<GameObject>();
    [SerializeField] List<GameObject> personalHitBoxes = new List<GameObject>();

    CharacterMovement cMovement;
    CharacterStatistics cStatistics;
    CharacterManager cManager;
    Collider2D myCollider;

    [SerializeField] bool invincible = false;
    bool parrying = false;
    Vector2 facingDirection;
    Coroutine parryCoroutine;

    float defenseModifier = 1;

    void Start()
    {
        cMovement = GetComponentInParent<CharacterMovement>();
        cStatistics = GetComponentInParent<CharacterStatistics>();
        cManager = GetComponentInParent<CharacterManager>();
        myCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        invincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((lm_hitbox & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && !personalHitBoxes.Contains(collision.gameObject))
        {
            // Identify Hitbox
            var hitBox = collision.gameObject.GetComponent<HitBox>();
            Vector2 direction = hitBox.FindGlobalDirection(transform).normalized;

            if (hitBox.originObject != transform.root.gameObject)
            {
                float hitAlignment = Vector2.Dot(facingDirection, direction) * .5f + .5f;//between 0 and 1, 0 is full accurate
                AttackData attackData = hitBox.GetAttackSO();

                //point on my collider closest to the hitbox, used as the hit/parry vfx position
                Vector2 hitPoint = myCollider.ClosestPoint(collision.transform.position);

                if (parrying && hitAlignment <= attackData.parryLeniency)
                {
                    hitBox.Parried();

                    //midpoint between me and the attacker, used as the parry vfx position
                    Vector2 parryPoint = (transform.root.position + hitBox.originObject.transform.position) * .5f;
                    cManager.GetHitBox().ParrySuccess(parryPoint, hitBox.originObject);//spawns my parry action's hit vfx

                    //Debug.Log("successful parry, accuracy: " + (1f - hitAlignment));
                    //Debug.Log("hitBoxDir: " + direction + " my dir: " + facingDirection + " accuracy: " + (1f - hitAlignment));
                }
                else if (!invincible)
                {
                    InvincibleForSeconds(afterHurtInvincibleDuration);
                    // Use Hitbox info to affect me
                    cManager.Stun(attackData.stunDuration, durationUntilDodgeCancellable);

                    cManager.Launch(direction, attackData.knockbackPower, true);
                    cStatistics.UpdateHealth(-(attackData.attackDamage / defenseModifier));

                    hitBox.Hit(this, hitPoint, transform.root.gameObject);
                }
            }
        }

        if ((lm_buffSource & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && !personalHitBoxes.Contains(collision.gameObject)) {
            var hitBox = collision.gameObject.GetComponent<HitBox>();
            BuffData buffData = hitBox.GetBuffs(this);
            if (buffData != null) {
                cManager.AddBuff(buffData);
            }
        }
    }

    public void Parry(float parryTime, Vector2 direction)
    {
        if (parryCoroutine != null)
            StopCoroutine(parryCoroutine);
        if (parryTime < 0) {
            parrying = false;
            return;
        }

        parryCoroutine = StartCoroutine(ParryTime(parryTime));

        SetFacingDirection(direction);
    }

    public void AddPersonalHitBox(GameObject hitbox) {
        personalHitBoxes.RemoveAll(item => item == null);
        personalHitBoxes.Add(hitbox);
    }

    public void SetFacingDirection(Vector2 direction) {
        facingDirection = direction.normalized;
    }

    public void InvincibleForSeconds(float seconds)
    {
        StartCoroutine(InvisibilityTime(seconds));
    }

    IEnumerator InvisibilityTime(float duration)
    {
        invincible = true;
        yield return new WaitForSeconds(duration);
        invincible = false;
    }

    IEnumerator ParryTime(float duration)
    {
        parrying = true;
        yield return new WaitForSeconds(duration);
        parrying = false;
    }

    public void SetDefenseModifier(float defenseModifier) { this.defenseModifier = defenseModifier; }
}
