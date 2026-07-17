using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HurtBox : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] LayerMask lm_hitbox;
    [SerializeField] LayerMask lm_hurtbox;

    [SerializeField] bool invinsible = false;
    [SerializeField] float afterHurtInvinsibleDuration = 0.2f;

    [SerializeField] List<GameObject> personalHurtBoxes = new List<GameObject>();
    [SerializeField] List<GameObject> personalHitBoxes = new List<GameObject>();

    CharacterMovement cMovement;
    CharacterStatistics cStatistics;

    void Start()
    {
        cMovement = GetComponentInParent<CharacterMovement>();
        cStatistics = GetComponentInParent<CharacterStatistics>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((lm_hitbox & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && !personalHitBoxes.Contains(collision.gameObject) && invinsible == false)
        {
            // Identify Hitbox
            var hitBox = collision.gameObject.GetComponent<HitBox>();
            Vector2 direction = hitBox.FindGlobalDirection();

            if (hitBox.originObject != transform.root.gameObject)
            {
                InvinsibleForSeconds(afterHurtInvinsibleDuration);

                // Use Hitbox info to affect me
                cMovement.Knockback(direction, hitBox.knockBackPower, true);
                cStatistics.UpdateHealth( -hitBox.damage);

                hitBox.Hit();
            }
        }
    }

    public void InvinsibleForSeconds(float seconds)
    {
        StartCoroutine(InvisibilityTime(seconds));
    }

    IEnumerator InvisibilityTime(float duration)
    {
        invinsible = true;
        yield return new WaitForSeconds(duration);
        invinsible = false;
    }
}
