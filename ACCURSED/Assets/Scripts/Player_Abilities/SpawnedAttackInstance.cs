using UnityEngine;

public class SpawnedAttackInstance : MonoBehaviour
{
    [SerializeField] HitBox[] hitBoxes;
    public void SetAttackData(AttackData data)
    {
        foreach (HitBox hitBox in hitBoxes)
        {
            hitBox.SetAttackData(data);
        }
    }

    public void OnAnimationFinish()
    {
         Destroy(gameObject);
    }
}
