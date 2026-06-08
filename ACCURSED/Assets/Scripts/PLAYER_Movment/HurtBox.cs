using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HurtBox : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] LayerMask lm_hitbox;
    [SerializeField] LayerMask lm_hurtbox;

    [SerializeField] bool invinsible = false;

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
                invinsible = true;
                StartCoroutine("InvisibilityTime");

                // Use Hitbox info to affect me
                cMovement.Knockback(direction, hitBox.knockBackPower, true);
                cStatistics.currentHealth -= hitBox.damage;
            }
        }
    }

    IEnumerator InvisibilityTime()
    {
        yield return new WaitForSeconds(0.1f);
        invinsible = false;
    }
}
