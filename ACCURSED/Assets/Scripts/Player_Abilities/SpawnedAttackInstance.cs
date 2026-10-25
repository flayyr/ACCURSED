using UnityEngine;

public class SpawnedAttackInstance : MonoBehaviour
{
    [SerializeField] HitBox[] hitBoxes;
    public void SetAttackData(AttackData data, ref HurtBox parentHurtbox)
    {
        foreach (HitBox hitBox in hitBoxes)
        {
            hitBox.SetAttackData(data);
            parentHurtbox.AddPersonalHitBox(hitBox.gameObject);
        }
    }

    public void OnAnimationFinish()
    {
         Destroy(gameObject);
    }
}
