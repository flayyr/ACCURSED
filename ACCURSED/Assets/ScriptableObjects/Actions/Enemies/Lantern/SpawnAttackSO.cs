using UnityEngine;

[CreateAssetMenu(fileName = "SpawnAttackSO", menuName = "Actions/SpawnAttackSO")]
public class SpawnAttackSO : ActionSO
{
    [Header("Attack Spawning")]
    [SerializeField] SpawnedAttackInstance attackPrefab;

    public override void GeneralActionTrigger(ref CharacterReference characterRef )
    {
        base.GeneralActionTrigger(ref characterRef);
        SpawnedAttackInstance attackInstance = Instantiate(attackPrefab);
        attackInstance.transform.position = characterRef.spriteRenderer.transform.position;
        attackInstance.SetAttackData(attackData);
    }
}
