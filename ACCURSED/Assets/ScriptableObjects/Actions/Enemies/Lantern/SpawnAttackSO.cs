using UnityEngine;

[CreateAssetMenu(fileName = "SpawnAttackSO", menuName = "Actions/SpawnAttackSO")]
public class SpawnAttackSO : ActionSO
{
    [Header("Attack Spawning")]
    [SerializeField] SpawnedAttackInstance attackPrefab;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] SpawnLocation spawnLocationType;

    public enum SpawnLocation { SpriteRenderer, HitBox, PlayerPos}

    public override void GeneralActionTrigger(ref CharacterReference characterRef )
    {
        base.GeneralActionTrigger(ref characterRef);
        if (attackPrefab != null) {
            SpawnedAttackInstance attackInstance = Instantiate(attackPrefab);

            switch (spawnLocationType) {
                case SpawnLocation.SpriteRenderer:
                    attackInstance.transform.position = characterRef.hitBox.transform.position;
                    break;
                case SpawnLocation.HitBox:
                    attackInstance.transform.position = characterRef.spriteRenderer.transform.position;
                    break;
                case SpawnLocation.PlayerPos:
                    attackInstance.transform.position = PersistentPlayer.Instance.transform.position;
                    break;
            }
            attackInstance.transform.position += (Vector3)spawnOffset;

            attackInstance.SetAttackData(attackData, ref characterRef.hurtBox, characterRef.vfxSpawner);
        }
    }
}
