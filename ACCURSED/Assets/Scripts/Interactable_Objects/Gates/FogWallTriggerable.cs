using UnityEngine;

public class FogWallTriggerable : Triggerable
{
    public override void Trigger(bool activate) {
        base.Trigger(activate);
        gameObject.SetActive(!activate);
    }
}
