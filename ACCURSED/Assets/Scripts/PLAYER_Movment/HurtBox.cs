using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MoreMountains.Feedbacks;

public class HurtBox : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] LayerMask lm_hitbox;
    [SerializeField] LayerMask lm_hurtbox;

    [Space]
    [SerializeField] float afterHurtInvincibleDuration = 0.2f;
    [SerializeField] float durationUntilDodgeCancellable = 0.5f;
    [Space]
    [SerializeField] private MMF_Player hurtFeedback;

    [SerializeField] List<GameObject> personalHurtBoxes = new List<GameObject>();
    [SerializeField] List<GameObject> personalHitBoxes = new List<GameObject>();

    CharacterMovement cMovement;
    CharacterStatistics cStatistics;
    CharacterManager combatManager;
    
    bool invincible = false;
    bool parrying = false;
    Vector2 parryDirection;
    Coroutine parryCoroutine;

    void Start()
    {
        cMovement = GetComponentInParent<CharacterMovement>();
        cStatistics = GetComponentInParent<CharacterStatistics>();
        combatManager = GetComponentInParent<CharacterManager>();
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
                float parryAccuracy = Vector2.Dot(parryDirection, direction)*.5f + .5f;//between 0 and 1, 0 is full accurate
                AttackData attackData = hitBox.GetAttackSO();

                if (parrying && parryAccuracy<=attackData.parryLeniency)
                {
                    hitBox.Parried();

                    Debug.Log("successful parry, accuracy: "+(1f-parryAccuracy));
                }
                else if (!invincible)
                {
                    InvincibleForSeconds(afterHurtInvincibleDuration);


                    // Use Hitbox info to affect me
                    combatManager.Stun(attackData.stunDuration, durationUntilDodgeCancellable);

                    combatManager.Launch(direction, attackData.knockbackPower, true);
                    cStatistics.UpdateHealth(-attackData.attackDamage);

                    if (hurtFeedback != null)
                        hurtFeedback.PlayFeedbacks();

                    hitBox.Hit();
                }
            }
        }
    }

    public void Parry(float parryTime, Vector2 direction)
    {
        if(parryCoroutine != null)
            StopCoroutine(parryCoroutine);

        parryCoroutine = StartCoroutine(ParryTime(parryTime));

        parryDirection = direction.normalized;
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
}
