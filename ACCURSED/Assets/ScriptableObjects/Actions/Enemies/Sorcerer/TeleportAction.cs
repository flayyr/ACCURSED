using UnityEngine;

[CreateAssetMenu(fileName = "TeleportAction", menuName = "Actions/TeleportAction")]
public class TeleportAction : ActionSO
{
    [Header("Teleport Action")]
    [SerializeField] float teleportDistance;

    public override void GeneralActionTrigger(ref CharacterReference characterRef) {
        base.GeneralActionTrigger(ref characterRef);
        characterRef.cManager.transform.position += (Vector3)Random.onUnitCircle * teleportDistance;
    }
}
