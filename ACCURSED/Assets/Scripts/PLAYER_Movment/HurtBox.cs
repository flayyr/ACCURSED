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
    [SerializeField] float afterHurtInvincibleDuration = 0.2f;
    [SerializeField] float afterHurtStunDuration = 1.2f;
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
        if ((lm_hitbox & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && !personalHitBoxes.Contains(collision.gameObject) && invincible == false)
        {
            // Identify Hitbox
            var hitBox = collision.gameObject.GetComponent<HitBox>();
            Vector2 direction = hitBox.FindGlobalDirection();

            if (hitBox.originObject != transform.root.gameObject)
            {
                InvincibleForSeconds(afterHurtInvincibleDuration);

                
                combatManager.Stun(afterHurtStunDuration, durationUntilDodgeCancellable);
                

                // Use Hitbox info to affect me
                AttackData attackData = hitBox.GetAttackSO();

                combatManager.Launch(direction, attackData.knockbackPower, true);
                cStatistics.UpdateHealth( -attackData.attackDamage);

                if(hurtFeedback!=null)
                    hurtFeedback.PlayFeedbacks();

                hitBox.Hit();
            }
        }
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
}
