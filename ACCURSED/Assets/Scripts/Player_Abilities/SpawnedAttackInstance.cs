using UnityEngine;

public class SpawnedAttackInstance : MonoBehaviour
{
    [SerializeField] HitBox[] hitBoxes;
    public void SetAttackData(AttackData data, ref HurtBox parentHurtbox, VFXSpawner vfxSpawner)
    {
        foreach (HitBox hitBox in hitBoxes)
        {
            hitBox.SetAttackData(data);
            hitBox.SetVFXSpawner(vfxSpawner);
            parentHurtbox.AddPersonalHitBox(hitBox.gameObject);
        }
    }

    public void OnAnimationFinish()
    {
         Destroy(gameObject);
    }
}
