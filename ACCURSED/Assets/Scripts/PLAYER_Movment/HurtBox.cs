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
    bool invincible = false;
    bool perfectParry = false;

    [SerializeField] float afterHurtInvincibleDuration = 0.2f;
    [SerializeField] float durationUntilDodgeCancellable = 0.5f;
    [Space]
    [SerializeField] private MMF_Player hurtFeedback;

    [SerializeField] List<GameObject> personalHurtBoxes = new List<GameObject>();
    [SerializeField] List<GameObject> personalHitBoxes = new List<GameObject>();

    CharacterMovement cMovement;
    CharacterStatistics cStatistics;
    CharacterManager combatManager;

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
            Vector2 direction = hitBox.FindGlobalDirection();

            if (hitBox.originObject != transform.root.gameObject)
            {
                if (!invincible)
                {
                    InvincibleForSeconds(afterHurtInvincibleDuration);


                    // Use Hitbox info to affect me
                    AttackData attackData = hitBox.GetAttackSO();

                    combatManager.Stun(attackData.stunDuration, durationUntilDodgeCancellable);

                    combatManager.Launch(direction, attackData.knockbackPower, true);
                    cStatistics.UpdateHealth(-attackData.attackDamage);

                    if (hurtFeedback != null)
                        hurtFeedback.PlayFeedbacks();

                    hitBox.Hit();
                } else if(perfectParry)
                {
                    hitBox.PerfectParried();

                    Debug.Log("perfect parry");
                }
            }
        }
    }

    public void Parry(float parryTime, float perfectParryWindow)
    {
        InvincibleForSeconds(parryTime);
        StartCoroutine(PerfectParryTime(perfectParryWindow));
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

    IEnumerator PerfectParryTime(float duration)
    {
        perfectParry = true;
        yield return new WaitForSeconds(duration);
        perfectParry = false;
    }
}
