using UnityEngine;

[CreateAssetMenu(fileName = "SpawnAttackSO", menuName = "Actions/SpawnAttackSO")]
public class SpawnAttackSO : ActionSO
{
    [Header("Attack Spawning")]
    [SerializeField] SpawnedAttackInstance attackPrefab;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] bool useHitBoxPos;

    public override void GeneralActionTrigger(ref CharacterReference characterRef )
    {
        base.GeneralActionTrigger(ref characterRef);
        if (attackPrefab != null) {
            SpawnedAttackInstance attackInstance = Instantiate(attackPrefab);

            if(useHitBoxPos)
                attackInstance.transform.position = characterRef.hitBox.transform.position;
            else
                attackInstance.transform.position = characterRef.spriteRenderer.transform.position;
            attackInstance.transform.position += (Vector3)spawnOffset;

            attackInstance.SetAttackData(attackData, ref characterRef.hurtBox, characterRef.vfxSpawner);
        }
    }
}
