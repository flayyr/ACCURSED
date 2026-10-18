using UnityEngine;

// Placeholder Placeholder Placeholder
[CreateAssetMenu(fileName = "NewFable", menuName = "Actions/Fable/BaseFable")]
public class FableSO : AbilitySO
{
    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        // Keep the existing AbilitySO behavior. Add Fable-specific effects later.
        base.PlayerActionTrigger(ref playerRef);
    }
}
