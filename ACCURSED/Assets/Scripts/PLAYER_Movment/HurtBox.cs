using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    [SerializeField] List<GameObject> personalHurtBoxes = new List<GameObject>();
    [SerializeField] List<GameObject> personalHitBoxes = new List<GameObject>();

    CharacterMovement cMovement;
    CharacterStatistics cStatistics;

    PlayerController playerController;

    void Start()
    {
        cMovement = GetComponentInParent<CharacterMovement>();
        cStatistics = GetComponentInParent<CharacterStatistics>();
        playerController = GetComponentInParent<PlayerController>();
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

                if (playerController != null)
                {
                    playerController.SetState( PlayerControlState.Disabled);
                    playerController.SetStateDelayed(PlayerControlState.DodgeOnly, durationUntilDodgeCancellable);
                    playerController.SetStateDelayed(PlayerControlState.Normal, afterHurtStunDuration);

                }

                // Use Hitbox info to affect me
                cMovement.Knockback(direction, hitBox.knockBackPower, true);
                cStatistics.UpdateHealth( -hitBox.damage);

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
